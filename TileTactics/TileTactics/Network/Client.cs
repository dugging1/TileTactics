using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TileTactics.Network.Packet;

namespace TileTactics.Network {
	public class Client {
		public static ConcurrentQueue<NetPacket> RecievedPacket = new ConcurrentQueue<NetPacket>();
		public static ConcurrentQueue<NetPacket> ToSendPacket = new ConcurrentQueue<NetPacket>();
		public static ConcurrentQueue<byte[]> ToProcess = new ConcurrentQueue<byte[]>();

		Main m;
		public ConcurrentDictionary<string, ClientPlayerObject> players = new ConcurrentDictionary<string, ClientPlayerObject>();
		public bool isSelfAlive {
			get {
				ClientPlayerObject p;
				players.TryGetValue(m.gui.data[(int)MMTextFieldSelected.UserName], out p);
				if (p != null)
					return p.alive;
				return false;
			}
		}

		public Client(string ip, int port, Main M) {
			m = M;
			ClientSocketHandler.connect(NetPacket.stringToLongIP(ip), port);
		}

		public void update() {
			NetPacket p;
			RecievedPacket.TryPeek(out p);
			if (p.p != null)
				Console.WriteLine("Handling a recieved packet: "+p.p.GetType().ToString());
			handleRecieve();

			ToSendPacket.TryPeek(out p);
			if (p.p != null)
				Console.WriteLine("Handling a send packet: "+p.p.GetType().ToString());
			handleSend();
		}

		private delegate void handlePacket(Client c, Packet p);
		private handlePacket[] packetHandlers = new handlePacket[] {
			(Client c, Packet p) => c.handleTilePacket((TilePacket)p),
			(Client c, Packet p) => c.handleMovePacket((MovePacket)p),
			(Client c, Packet p) => c.handleAttackPacket((AttackPacket)p),
			(Client c, Packet p) => c.handleTradePacket((TradePacket)p),
			(Client c, Packet p) => c.handlePlayerPacket((PlayerPacket)p)
		};

		private void handlePlayerPacket(PlayerPacket p) {
			ClientPlayerObject player = new ClientPlayerObject(p.username, p.online, p.alive);
			players.AddOrUpdate(p.username, player, (string k, ClientPlayerObject v) => player);
		}

		private void handleTradePacket(TradePacket p) {
			//Trade packet received client side (shouldn't ever happen)
			throw new Exception("Trade packet recieved client side.");
		}

		private void handleAttackPacket(AttackPacket p) {
			//Attack packet received client side (shouldn't ever happen)
			throw new Exception("Attack packet recieved client side.");
		}

		private void handleMovePacket(MovePacket p) {
			//Move packet received client side (shouldn't ever happen)
			throw new Exception("Move packet recieved client side.");
		}

		private void handleTilePacket(TilePacket p) {
			//Update tile packet recieved on client side.
			if (p.hasChar) {
				m.map.setData((int)p.Pos.X, (int)p.Pos.Y, p.u);
			} else {
				m.map.setData((int)p.Pos.X, (int)p.Pos.Y, null);
			}
		}

		private void handleRecieve() {
			NetPacket p;
			bool rec = RecievedPacket.TryDequeue(out p);
			if (rec) {
				packetHandlers[p.p.ID](this, p.p);
			}
		}

		private void handleSend() {
			NetPacket p;
			bool rec = ToSendPacket.TryDequeue(out p);
			if (rec) {
				ClientSocketHandler.Send(p.p);
			}
		}
	}
}
