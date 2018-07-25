using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TileTactics.Network {
	public class ServerSocketHandler {
		TcpListener serverSocket;
		List<TcpClient> clients = new List<TcpClient>();

		public ServerSocketHandler(long ip, int port) {
			serverSocket = new TcpListener(new IPEndPoint(ip, port));
			
		}

		private void updateLoop() {

		}
	}
}
