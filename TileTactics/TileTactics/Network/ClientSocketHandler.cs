using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TileTactics.Network {
	public class ClientSocketHandler {
		public static ManualResetEvent allDone = new ManualResetEvent(false);
		public static Socket clientSocket;
		
		public static void connect(long ip, int port) {
			IPEndPoint addr = new IPEndPoint(ip, port);
			clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			clientSocket.Connect(addr);

			Thread t = new Thread(new ThreadStart(listen));
			t.Start();
		}

		private static void listen() {
			while (true) {
				allDone.Reset();

				StateObject state = new StateObject();
				state.workSocket = clientSocket;
				clientSocket.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReadCB), state);

				allDone.WaitOne();
			}
		}

		private static void ReadCB(IAsyncResult ar) {
			Console.WriteLine("Starting recieve bytes.");
			StateObject state = (StateObject)ar.AsyncState;

			int bytesRead = 0;

			try {
				bytesRead = clientSocket.EndReceive(ar);
			} catch (SocketException) {
				clientSocket.Close();
				return; //End program
			}

			if (bytesRead > 0) {
				if (state.messageLength == -1) { 
					//Start of new message
					state.messageLength = BitConverter.ToInt32(state.buffer, 0);
					if (state.buffer.Length >= state.messageLength-state.recieved) {
						state.content.AddRange(state.buffer.Skip(sizeof(int)).Take(state.messageLength-state.recieved));
						byte[] packet = state.content.ToArray();
						StateObject s = new StateObject() { workSocket = state.workSocket, content = new List<byte>(packet), messageLength = state.messageLength, recieved = state.messageLength};
						handleData(s);
						int offset = sizeof(int)+state.messageLength-state.recieved;
						state.buffer = new byte[StateObject.bufferSize];
						state.buffer.Skip(offset).ToArray().CopyTo(state.buffer, 0);
						state.messageLength = -1;
						state.recieved = 0;
						clientSocket.BeginReceive(state.buffer, StateObject.bufferSize-offset, offset, 0, new AsyncCallback(ReadCB), state);
					} else {
						state.content.AddRange(state.buffer.Skip(sizeof(int)));
						state.recieved += StateObject.bufferSize-sizeof(int);
						clientSocket.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReadCB), state);
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
						clientSocket.BeginReceive(state.buffer, StateObject.bufferSize-offset, offset, 0, new AsyncCallback(ReadCB), state);
					} else {
						state.content.AddRange(state.buffer);
						state.recieved += StateObject.bufferSize;
						clientSocket.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReadCB), state);
					}
				}
			}
		}

		private static void handleData(StateObject state) {
			byte[] data = state.content.ToArray();
			Console.WriteLine("Handling data of length: "+data.Length);
			Packet p = Packet.fromByte(BitConverter.ToInt32(data, 0), data.Skip(sizeof(int)).ToArray());
			Client.RecievedPacket.Enqueue(new NetPacket(state.workSocket.RemoteEndPoint as IPEndPoint, p));
		}

		public static void Send(Packet p) {
			byte[] data = p.toByte();
			int len = data.Length;
			List<byte> send = new List<byte>();
			send.AddRange(BitConverter.GetBytes(len));
			send.AddRange(data);
			data = send.ToArray();
			clientSocket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(sendCB), p);
		}

		public static void SendSync(Packet p) {
			byte[] data = p.toByte();
			int len = data.Length;
			List<byte> send = new List<byte>();
			send.AddRange(BitConverter.GetBytes(len));
			send.AddRange(data);
			data = send.ToArray();
			clientSocket.Send(data);
		}

		private static void sendCB(IAsyncResult ar) {
			clientSocket.EndSend(ar);
			Console.WriteLine("Sent packet: "+ar.AsyncState.GetType().ToString());
		}
	}
}
