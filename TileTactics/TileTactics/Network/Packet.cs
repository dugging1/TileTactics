using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileTactics.Network {
	public abstract class Packet {
		public abstract int ID { get; }
		static Type[] PacketIds = new Type[4];

		public static void packetInit() {
			PacketIds[0] = typeof(TilePacket);
			TilePacket.id = 0;
			PacketIds[1] = typeof(MovePacket);
			MovePacket.id = 1;
			PacketIds[2] = typeof(AttackPacket);
			AttackPacket.id = 2;
			PacketIds[3] = typeof(TradePacket);
			TradePacket.id = 3;
		}

		public abstract byte[] toByte();

		public static Packet fromByte(int id, byte[] data) {
			return (Packet)Activator.CreateInstance(PacketIds[id], data);
		}

		/// <summary>
		/// Update tile packet (Recieved on client only)
		/// </summary>
		public class TilePacket : Packet {
			public static int id;
			public override int ID { get { return id; } }

			public Vector2 Pos;
			public bool hasChar;
			public Unit u;
			
			public TilePacket(byte[] data) {
				//Tile Pos, Empty Tile?, AP, HP, name len, name
				int startCounter = 0;
				int x = BitConverter.ToInt32(data, startCounter);
				startCounter += sizeof(int);
				int y = BitConverter.ToInt32(data, startCounter);
				Pos = new Vector2(x, y);
				startCounter += sizeof(int);
				hasChar = BitConverter.ToBoolean(data, startCounter);
				startCounter += sizeof(bool);
				if (hasChar) {
					int AP = BitConverter.ToInt32(data, startCounter);
					startCounter += sizeof(int);
					int HP = BitConverter.ToInt32(data, startCounter);
					startCounter += sizeof(int);
					int len = BitConverter.ToInt32(data, startCounter);
					startCounter += sizeof(int);
					string name = BitConverter.ToString(data, startCounter, len);
					u = new Unit(name, AP, HP);
				}
			}

			public TilePacket(Vector2 Pos) {
				this.Pos = Pos;
				hasChar = false;
				u = null;
			}

			public TilePacket(Vector2 Pos, Unit u) {
				this.Pos = Pos;
				hasChar = true;
				this.u = u;
			}

			public override byte[] toByte() {
				List<byte> ret = new List<byte>();
				ret.AddRange(BitConverter.GetBytes(id));
				ret.AddRange(BitConverter.GetBytes(Pos.X));
				ret.AddRange(BitConverter.GetBytes(Pos.Y));
				ret.AddRange(BitConverter.GetBytes(hasChar));
				if (hasChar) {
					ret.AddRange(BitConverter.GetBytes(u.AP));
					ret.AddRange(BitConverter.GetBytes(u.HP));
					ret.AddRange(BitConverter.GetBytes(u.Username.Length));
					for (int i = 0; i < u.Username.Length; i++) {
						ret.AddRange(BitConverter.GetBytes(u.Username.ToCharArray()[i]));
					}
				}
				return ret.ToArray();
			}
		}

		/// <summary>
		/// Move character packet (Recieved on server only; use TilePacket to update other clients)
		/// </summary>
		public class MovePacket : Packet {
			public static int id;
			public override int ID {
				get { return id; }
			}

			public Vector2 from;
			public Vector2 to;

			public MovePacket(byte[] data) {
				int[] iData = new int[4];
				for (int i = 0; i < 4; i++) {
					iData[i] = BitConverter.ToInt32(data, sizeof(int)*i);
				}
				from = new Vector2(iData[0], iData[1]);
				to = new Vector2(iData[2], iData[3]);
			}

			public MovePacket(Vector2 From, Vector2 To) {
				from = From;
				to = To;
			}

			public override byte[] toByte() {
				List<byte> ret = new List<byte>();

				ret.AddRange(BitConverter.GetBytes(id));
				ret.AddRange(BitConverter.GetBytes((int)from.X));
				ret.AddRange(BitConverter.GetBytes((int)from.Y));
				ret.AddRange(BitConverter.GetBytes((int)to.X));
				ret.AddRange(BitConverter.GetBytes((int)to.Y));

				return ret.ToArray();
			}
		}

		/// <summary>
		/// Attack initiated at 'from' atacking 'to'
		/// </summary>
		public class AttackPacket : Packet {
			public static int id;

			public override int ID {
				get { return id; }
			}

			public Vector2 from;
			public Vector2 to;

			public AttackPacket(byte[] data) {
				int[] iData = new int[4];
				for (int i = 0; i < 4; i++) {
					iData[i] = BitConverter.ToInt32(data, sizeof(int)*i);
				}
				from = new Vector2(iData[0], iData[1]);
				to = new Vector2(iData[2], iData[3]);
			}

			public AttackPacket(Vector2 From, Vector2 To) {
				from = From;
				to = To;
			}

			public override byte[] toByte() {
				List<byte> ret = new List<byte>();

				ret.AddRange(BitConverter.GetBytes(id));
				ret.AddRange(BitConverter.GetBytes((int)from.X));
				ret.AddRange(BitConverter.GetBytes((int)from.Y));
				ret.AddRange(BitConverter.GetBytes((int)to.X));
				ret.AddRange(BitConverter.GetBytes((int)to.Y));

				return ret.ToArray();
			}
		}

		/// <summary>
		/// Trade AP from 'from' to 'to'
		/// </summary>
		public class TradePacket:Packet {
			public static int id;

			public override int ID {
				get { return id; }
			}

			public Vector2 from;
			public Vector2 to;

			public TradePacket(byte[] data) {
				int[] iData = new int[4];
				for (int i = 0; i < 4; i++) {
					iData[i] = BitConverter.ToInt32(data, sizeof(int)*i);
				}
				from = new Vector2(iData[0], iData[1]);
				to = new Vector2(iData[2], iData[3]);
			}

			public TradePacket(Vector2 From, Vector2 To) {
				from = From;
				to = To;
			}

			public override byte[] toByte() {
				List<byte> ret = new List<byte>();

				ret.AddRange(BitConverter.GetBytes(id));
				ret.AddRange(BitConverter.GetBytes((int)from.X));
				ret.AddRange(BitConverter.GetBytes((int)from.Y));
				ret.AddRange(BitConverter.GetBytes((int)to.X));
				ret.AddRange(BitConverter.GetBytes((int)to.Y));

				return ret.ToArray();
			}
		}
	}
}
