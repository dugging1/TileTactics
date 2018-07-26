using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static TileTactics.Network.Packet;

namespace TileTactics.Network {
		public class Server {
		public static ConcurrentQueue<NetPacket> RecievedPacket = new ConcurrentQueue<NetPacket>();
		public static ConcurrentQueue<NetPacket> ToSendPacket = new ConcurrentQueue<NetPacket>();

		private Main m;

		public Server(string ip, int port, Main m) {
			this.m = m;
			long IP = NetPacket.stringToLongIP(ip);

			Thread t = new Thread(new ThreadStart(() => ServerSocketHandler.StartListening(IP, port)));
			t.Start();
		}

		public void update() {
			handleRecievePacket();
			handleSendPacket();
		}

		private delegate void handlePacket(Server s, Packet p);

		private handlePacket[] packetHandlers = new handlePacket[] {
			(Server s, Packet p) => s.handleTilePacket((TilePacket)p),
			(Server s, Packet p) => s.handleMovePacket((MovePacket)p),
			(Server s, Packet p) => s.handleAttackPacket((AttackPacket)p),
			(Server s, Packet p) => s.handleTradePacket((TradePacket)p)
		};

		private void handleTradePacket(TradePacket p) {
			//Trade packet recieved server side
		}

		private void handleAttackPacket(AttackPacket p) {
			//Attack packet recieved server side
		}

		private void handleMovePacket(MovePacket p) {
			//Move packet recieved server side
		}

		private void handleTilePacket(TilePacket p) {
			//Tile update arriving from client (shouldn't ever happen)
			throw new Exception("Tile update arrived from client.");
		}

		private void handleRecievePacket() {
			NetPacket p;
			bool rec = RecievedPacket.TryDequeue(out p);
			if (rec) {
				packetHandlers[p.p.ID](this, p.p);
			}
		}

		private void handleSendPacket() {
			NetPacket p;
			bool rec = ToSendPacket.TryDequeue(out p);
			if (rec) {
				ServerSocketHandler.Send(p.addr, p.p);
			}
		}
	}
}
