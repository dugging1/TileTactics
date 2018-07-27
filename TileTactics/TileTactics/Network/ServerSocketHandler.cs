using System;
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

		public static void addClient(Socket s) {
			lock (clients) {
				clients.Add(s);
			}
		}

		public static Socket getClient(IPEndPoint addr) {
			Socket ret = null;
			lock (clients) {
				for (int i = 0; i < clients.Count; i++) {
					if(clients[i].RemoteEndPoint as IPEndPoint == addr) {
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
			StateObject state = (StateObject)ar.AsyncState;
			Socket handler = state.workSocket;
			int bytesRead = 0;

			try {
				bytesRead = handler.EndReceive(ar);
			}catch(SocketException e) {
				removeClient(handler.RemoteEndPoint as IPEndPoint);
				return;
			}

			if(bytesRead > 0) {
				if(state.messageLength == -1) { //Start of message
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
				}				
			}
		}

		private static void handleData(StateObject state) {
			byte[] data = state.content.ToArray();
			Packet p = Packet.fromByte(BitConverter.ToInt32(data, 0), data.Skip(sizeof(int)).ToArray());
			IPEndPoint clientAddr = state.workSocket.RemoteEndPoint as IPEndPoint;
			Server.RecievedPacket.Enqueue(new NetPacket(clientAddr, p));
		}

		public static void Send(Socket handler, Packet p) {
			byte[] data = p.toByte();
			handler.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCB), handler);
		}

		public static void Send(IPEndPoint addr, Packet p) {
			Send(getClient(addr), p);
		}

		private static void SendCB(IAsyncResult ar) {
			Socket handler = (Socket)ar.AsyncState;
			handler.EndSend(ar);
		}
	}
}
