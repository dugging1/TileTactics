﻿using System;
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

				clientSocket.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReadCB), state);

				allDone.WaitOne();
			}
		}

		private static void ReadCB(IAsyncResult ar) {
			StateObject state = (StateObject)ar.AsyncState;

			int bytesRead = clientSocket.EndReceive(ar);

			if (bytesRead > 0) {
				if (state.messageLength == -1) { //Start of message
					state.messageLength = BitConverter.ToInt32(state.buffer, 0);
					state.content.AddRange(state.buffer.Skip(sizeof(int)));
					state.recieved += bytesRead-sizeof(int);

					if (state.recieved == state.messageLength) {
						//All data recieved; Handle data;
						handleData(state);
					} else {
						//Get more data
						clientSocket.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReadCB), state);
					}
				}
			}
		}

		private static void handleData(StateObject state) {
			byte[] data = state.content.ToArray();
			Packet p = Packet.fromByte(BitConverter.ToInt32(data, 0), data.Skip(sizeof(int)).ToArray());
			//TODO: Put packet in recieved queue
		}

		public static void Send(Packet p) {
			byte[] data = p.toByte();
			clientSocket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(sendCB), null);
		}

		private static void sendCB(IAsyncResult ar) {
			clientSocket.EndSend(ar);
		}
	}
}