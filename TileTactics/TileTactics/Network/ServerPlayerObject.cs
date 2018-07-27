using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileTactics.Network {
	public class ServerPlayerObject {
		public long ip;
		public int port;
		public string username;
		public string password;
		public bool online;
		public bool alive;

		public ServerPlayerObject(long ip, int port, string username, string password, bool online, bool alive) {
			this.ip = ip;
			this.port = port;
			this.username = username;
			this.password = password;
			this.online = online;
			this.alive = alive;
		}
	}
}
