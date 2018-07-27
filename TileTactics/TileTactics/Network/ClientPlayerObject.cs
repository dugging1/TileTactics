using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileTactics.Network {
	public class ClientPlayerObject {
		public string username;
		public bool online;
		public bool alive;

		public ClientPlayerObject(string username, bool online, bool alive) {
			this.username = username;
			this.online = online;
			this.alive = alive;
		}
	}
}
