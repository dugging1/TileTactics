using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileTactics.Network;
using static TileTactics.Network.Packet;

namespace TileTactics {
	public class Map {
		private Unit[] mapData = new Unit[70*70];
		public Unit getData(int x, int y) { return mapData[y * 70 + x]; }
		public Vector2 TileSelected = new Vector2(-1);

		public void setData(int x, int y, Unit value) {
			mapData[y * 70 + x] = value;
		}

		public void dailyAPBoost() {
			foreach (Unit User in mapData) {
				if (User != null) {
					User.AP = User.AP + 1;
				}
			}
		}

		public void AddRandomUnit(string Username) {
			bool locationFound = false;
			Random R = new Random();

			int RX;
			int RY;

			while (locationFound != true) {
				RX = R.Next(0, 69);
				RY = R.Next(0, 69);
				if (getData(RX, RY) == null) {
					setData(RX, RY, new Unit(Username));
					locationFound = true;
				}
			}
		}

		public void draw(SpriteBatch s, Main m) {
			for (int x = 0; x < 70; x++) {
				for (int y = 0; y < 70; y++) {
					if (m.gui.MainMenuOpen != true) {
						if (TileSelected == new Vector2(x, y)) {
							s.Draw(Main.Textures["TileSelected"], new Vector2((x * 64), (y * 64)));
						} else {
							s.Draw(Main.Textures["Tile"], new Vector2((x * 64), (y * 64)));
						}
						if (getData(x, y) != null) {
							getData(x, y).draw(s, new Vector2((x * 64), (y * 64)));
						}
					}
				}
			}
		}

		public bool MoveCheck(Vector2 Origin, Vector2 Endpoint) //checks square radi of 1 tile around Origin
		{
			return (Origin - Endpoint).Length() <= Math.Pow(2, 0.5f) && getData((int)Endpoint.X, (int)Endpoint.Y) == null;
		}

		public bool AimCheck(Vector2 Origin, Vector2 Endpoint) //checks square radi of 3 tiles around Origin
		{
			if (((Origin - Endpoint).Length() <= Math.Pow(18, 0.5f) && getData((int)Endpoint.X, (int)Endpoint.Y) == null))
				if (Endpoint != Origin + new Vector2(0, 4) && Endpoint != Origin + new Vector2(4, 0) && Endpoint != Origin + new Vector2(-4, 0) && Endpoint != Origin + new Vector2(0, -4))
					return true;
			return false;
		}

		public bool MoveUnit(Vector2 CurrentLocation, Vector2 NewLocation) {
			if (MoveCheck(CurrentLocation, NewLocation) && getData((int)CurrentLocation.X, (int)CurrentLocation.Y).AP >= 1) {
				Client.ToSendPacket.Enqueue(new NetPacket(null, new MovePacket(CurrentLocation, NewLocation)));
				return true;
			} else {
				return false;
			}
		}

		public bool Attack(Vector2 CurrentLocation, Vector2 EndLocation) {
			if (getData((int)CurrentLocation.X, (int)CurrentLocation.Y).AP >= 1 && getData((int)EndLocation.X, (int)EndLocation.Y) != null) {
				Client.ToSendPacket.Enqueue(new NetPacket(null, new AttackPacket(CurrentLocation, EndLocation)));
				return true;
			} else {
				return false;
			}
		}

		public bool ShootAP(Vector2 CurrentLocation, Vector2 EndLocation) {
			if (getData((int)CurrentLocation.X, (int)CurrentLocation.Y).AP >= 1 && getData((int)EndLocation.X, (int)EndLocation.Y) != null) {
				Client.ToSendPacket.Enqueue(new NetPacket(null, new TradePacket(CurrentLocation, EndLocation)));
				return true;
			} else {
				return false;
			}
		}
	}
}