﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TileTactics.Network {
	public class ServerSocketHandler {
		public static ManualResetEvent allDone = new ManualResetEvent(false);
		private static List<Socket> clients = new List<Socket>();
		public static List<IPEndPoint> Addresses = new List<IPEndPoint>();

		public static void addClient(Socket s) {
			lock (clients) {
				clients.Add(s);
			}
			lock (Addresses) {
				Addresses.Add(s.RemoteEndPoint as IPEndPoint);
			}
		}

		public static Socket getClient(IPEndPoint addr) {
			Socket ret = null;
			lock (clients) {
				for (int i = 0; i < clients.Count; i++) {
					IPEndPoint t = clients[i].RemoteEndPoint as IPEndPoint;
					if (t.Equals(addr)) {
						ret = clients[i];
					}
				}
			}
			return ret;
		}

		public static void removeClient(IPEndPoint addr) {
			lock (clients) {
				for (int i = 0; i < clients.Count; i++) {
					if (clients[i].RemoteEndPoint as IPEndPoint == addr) {
						lock (Addresses) {
							Addresses.Remove(clients[i].RemoteEndPoint as IPEndPoint);
						}
						if (clients[i].Connected) {
							clients[i].Shutdown(SocketShutdown.Both);
							clients[i].Close();
						}
						clients.RemoveAt(i);
						break;
					}
				}
			}
		}

		public static void StartListening(long ip, int port) {
			Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			IPEndPoint localEndPoint = new IPEndPoint(ip, port);

			try {
				listener.Bind(localEndPoint);
				listener.Listen(32);

				while (true) {
					allDone.Reset();

					Console.WriteLine("Awaiting connection...");
					listener.BeginAccept(new AsyncCallback(AcceptCB), listener); //listener sent as 'state' w/ type of object

					allDone.WaitOne();
				}
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
			}
		}

		private static void AcceptCB(IAsyncResult ar) {
			allDone.Set();

			Socket listener = (Socket)ar.AsyncState;
			Socket handler = listener.EndAccept(ar);
			addClient(handler);
			Console.WriteLine("Accepted connection from: "+(handler.RemoteEndPoint as IPEndPoint).Address.ToString());

			StateObject state = new StateObject();
			state.workSocket = handler;
			handler.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReadCB), state);
		}

		private static void ReadCB(IAsyncResult ar) {
			Console.WriteLine("Starting recieve bytes.");
			StateObject state = (StateObject)ar.AsyncState;
			Socket handler = state.workSocket;
			int bytesRead = 0;

			try {
				bytesRead = handler.EndReceive(ar);
			}catch(SocketException) {
				removeClient(handler.RemoteEndPoint as IPEndPoint);
				return;
			}

			if(bytesRead > 0) {
				if (state.messageLength == -1) {
					//Start of new message
					state.messageLength = BitConverter.ToInt32(state.buffer, 0);
					if (state.buffer.Length >= state.messageLength-state.recieved) {
						state.content.AddRange(state.buffer.Skip(sizeof(int)).Take(state.messageLength-state.recieved));
						byte[] packet = state.content.ToArray();
						StateObject s = new StateObject() { workSocket = state.workSocket, content = new List<byte>(packet), messageLength = state.messageLength, recieved = state.messageLength };
						handleData(s);
						int offset = sizeof(int)+state.messageLength-state.recieved;
						state.buffer = new byte[StateObject.bufferSize];
						state.buffer.Skip(offset).ToArray().CopyTo(state.buffer, 0);
						state.messageLength = -1;
						state.recieved = 0;
						handler.BeginReceive(state.buffer, StateObject.bufferSize-offset, offset, 0, new AsyncCallback(ReadCB), state);
					} else {
						state.content.AddRange(state.buffer.Skip(sizeof(int)));
						state.recieved += StateObject.bufferSize-sizeof(int);
						handler.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReadCB), state);
					}
				} else {
					if (state.buffer.Length >= state.messageLength-state.recieved) {
						state.content.AddRange(state.buffer.Take(state.messageLength-state.recieved));
						byte[] packet = state.content.ToArray();
						StateObject s = new StateObject() { workSocket = state.workSocket, content = new List<byte>(packet), messageLength = state.messageLength, recieved = state.messageLength };
						handleData(s);
						int offset = state.messageLength-state.recieved;
						state.buffer = new byte[StateObject.bufferSize];
						state.buffer.Skip(offset).ToArray().CopyTo(state.buffer, 0);
						state.messageLength = -1;
						state.recieved = 0;
						handler.BeginReceive(state.buffer, StateObject.bufferSize-offset, offset, 0, new AsyncCallback(ReadCB), state);
					} else {
						state.content.AddRange(state.buffer);
						state.recieved += StateObject.bufferSize;
						handler.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReadCB), state);
					}
				}
				/*if(state.messageLength == -1) { //Start of message
					state.messageLength = BitConverter.ToInt32(state.buffer, 0);
					state.content.AddRange(state.buffer.Skip(sizeof(int)));
					state.recieved += bytesRead-sizeof(int);

					if(state.recieved == state.messageLength) {
						//All data recieved; Handle data;
						handleData(state);
					} else {
						//Get more data
						handler.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReadCB), state);
					}
				}*/
			}
		}

		private static void handleData(StateObject state) {
			byte[] data = state.content.ToArray();
			Packet p = Packet.fromByte(BitConverter.ToInt32(data, 0), data.Skip(sizeof(int)).ToArray());
			IPEndPoint clientAddr = state.workSocket.RemoteEndPoint as IPEndPoint;
			Server.RecievedPacket.Enqueue(new NetPacket(clientAddr, p));
			Console.WriteLine("Enqueued packet.");
		}

		public static void Send(Socket handler, Packet p) {
			byte[] data = p.toByte();
			int len = data.Length;
			List<byte> send = new List<byte>();
			send.AddRange(BitConverter.GetBytes(len));
			send.AddRange(data);
			data = send.ToArray();
			handler.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCB), handler);
		}

		public static void Send(IPEndPoint addr, Packet p) {
			Send(getClient(addr), p);
		}

		private static void SendCB(IAsyncResult ar) {
			Socket handler = (Socket)ar.AsyncState;
			handler.EndSend(ar);
			Console.WriteLine("Sent packet to: "+(handler.RemoteEndPoint as IPEndPoint).Address.ToString());
		}
	}
}
