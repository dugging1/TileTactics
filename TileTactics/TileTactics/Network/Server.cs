using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TileTactics.Network {
	public struct NetPacket {
		Packet p;
		IPEndPoint addr;

		public NetPacket(IPEndPoint addr, Packet p) {
			this.addr = addr;
			this.p = p;
		}

		public NetPacket(long ip, int port, Packet p) {
			addr = new IPEndPoint(ip, port);
			this.p = p;
		}

		public NetPacket(string ip, int port, Packet p) {
			addr = new IPEndPoint(NetPacket.stringToLongIP(ip), port);
			this.p = p;
		}

		public static long stringToLongIP(string ip) {
			string[] ipParts = (string[])(ip.Split('.')).Reverse();
			long IP = 0;
			for (int i = 0; i < 4; i++) {
				IP += Convert.ToInt32(ipParts[i]) << 8*i;
			}
			return IP;
		}
	}

	public class Server {
		public static ConcurrentQueue<NetPacket> RecievedPacket = new ConcurrentQueue<NetPacket>();
		public static ConcurrentQueue<NetPacket> ToSendPacket = new ConcurrentQueue<NetPacket>();

		public Server(string ip, int port) {
			long IP = NetPacket.stringToLongIP(ip);

			Thread t = new Thread(new ThreadStart(() => ServerSocketHandler.StartListening(IP, port)));
			t.Start();
		}

		public void update() {

		}

		private void handleRecievePacket() {

		}

		private void handleSendPacket() {

		}
	}
}
