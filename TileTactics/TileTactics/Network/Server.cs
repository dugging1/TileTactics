using Microsoft.Xna.Framework;
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

		private delegate void handlePacket(Server s, NetPacket p);

		private handlePacket[] packetHandlers = new handlePacket[] {
			(Server s, NetPacket p) => s.handleTilePacket(p),
			(Server s, NetPacket p) => s.handleMovePacket((MovePacket)p.p),
			(Server s, NetPacket p) => s.handleAttackPacket((AttackPacket)p.p),
			(Server s, NetPacket p) => s.handleTradePacket((TradePacket)p.p)
		};

		private void handleTradePacket(TradePacket p) {
			//Trade packet recieved server side
			if (m.map.getData((int)p.from.X, (int)p.from.Y) == null) return;
			if (m.map.getData((int)p.to.X, (int)p.to.Y) == null) return;
			if (m.map.getData((int)p.from.X, (int)p.from.Y).AP == 0) return;

			Unit u = m.map.getData((int)p.from.X, (int)p.from.Y);
			u.AP--;
			m.map.setData((int)p.from.X, (int)p.from.Y, u);
			u = m.map.getData((int)p.to.X, (int)p.to.Y);
			u.AP++;
			m.map.setData((int)p.to.X, (int)p.to.Y, u);
			updateTile(p.from);
			updateTile(p.to);
		}

		private void handleAttackPacket(AttackPacket p) {
			//Attack packet recieved server side
			if (m.map.getData((int)p.from.X, (int)p.from.Y) == null) return;
			if (m.map.getData((int)p.to.X, (int)p.to.Y) == null) return;
			if (m.map.getData((int)p.from.X, (int)p.from.Y).AP == 0) return;

			Unit u = m.map.getData((int)p.from.X, (int)p.from.Y);
			u.AP--;
			m.map.setData((int)p.from.X, (int)p.from.Y, u);

			u = m.map.getData((int)p.to.X, (int)p.to.Y);
			u.HP--; //TODO: Handle death
			m.map.setData((int)p.to.X, (int)p.to.Y, u);
			updateTile(p.from);
			updateTile(p.to);
		}

		private void handleMovePacket(MovePacket p) {
			//Move packet recieved server side
			if (m.map.getData((int)p.from.X, (int)p.from.Y) == null) return;
			if (m.map.getData((int)p.to.X, (int)p.to.Y) != null) return;
			if (m.map.getData((int)p.from.X, (int)p.from.Y).AP == 0) return;

			Unit u = m.map.getData((int)p.from.X, (int)p.from.Y);
			u.AP--;
			m.map.setData((int)p.to.X, (int)p.to.Y, u);
			updateTile(p.from);
			updateTile(p.to);
		}

		private void handleTilePacket(NetPacket p) {
			//Tile update arriving from client (shouldn't ever happen)
			throw new Exception("Tile update arrived from client.");
		}

		private void updateTile(Vector2 vec) {
			lock (ServerSocketHandler.Addresses) {
				for (int i = 0; i < ServerSocketHandler.Addresses.Count; i++) {
					ToSendPacket.Enqueue(new NetPacket(ServerSocketHandler.Addresses[i], new TilePacket(vec, m.map.getData((int)vec.X, (int)vec.Y))));
				}
			}
		}

		private void handleRecievePacket() {
			NetPacket p;
			bool rec = RecievedPacket.TryDequeue(out p);
			if (rec) {
				packetHandlers[p.p.ID](this, p);
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
