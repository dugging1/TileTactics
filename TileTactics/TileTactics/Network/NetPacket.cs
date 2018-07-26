using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TileTactics.Network {
	public struct NetPacket {
		public Packet p;
		public IPEndPoint addr;

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
			string[] ipParts = (ip.Split('.')).ToArray();
			long IP = 0;
			for (int i = 0; i < 4; i++) {
				IP += Convert.ToInt64(ipParts[i]) << 8*i;
			}
			return IP;
		}
	}
}
