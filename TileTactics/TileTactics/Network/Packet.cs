using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileTactics.Network {
	public abstract class Packet {
		static Type[] PacketIds = new Type[8];

		public static void packetInit() {
			PacketIds[0] = typeof(TilePacket);
		}

		public abstract byte[] toByte();
		public static Packet fromByte(int id, byte[] data) {
			return (Packet)Activator.CreateInstance(PacketIds[id], data);
		}

		public class TilePacket : Packet {
			Vector2 Pos;
			bool hasChar;
			Unit u;
			
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
	}
}
