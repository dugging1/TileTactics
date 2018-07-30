using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TileTactics.Network;
using static TileTactics.Network.Packet;

namespace TileTactics {
	enum MMTextFieldSelected { IPAddress, Port, UserName, Password, None }

	public class GUI {
		public bool InfoOpen = false;
		public bool ClickedInGUI = false;
		public bool MainMenuOpen = true;

		public string[] data = new string[5] { "", "", "", "", "" }; //In Enum order w/ Avatar as last element

		private Main m;
		private MMTextFieldSelected selected = MMTextFieldSelected.None;

		private SpriteFont font;

		public GUI(Main M) {
			m = M;
			font = m.Content.Load<SpriteFont>("SF");
		}

		// Mx = Mouse X, EntX = Top Left Entity X, Wx = Entity Width
		public bool InBounds(float mx, float my, float entx, float enty, float wx, float wy) {
			if (mx > entx && mx < entx + wx)
				if (my > enty && my < enty + wy)
					return true;
			return false;
		}

		Vector2[] MMTextFields = new Vector2[] { new Vector2(170, 104), new Vector2(170, 160),
												 new Vector2(170, 234), new Vector2(170, 307)}; //Correct for camera
		Vector2 MMTextFieldSize = new Vector2(168, 21);//Correct for camera

		public void draw(SpriteBatch s, Main M) {
			if (m == null) m = M;

			if (m.gameState == GameState.MainMenu) {

				Vector2 MMBackground = m.camera.ScreenToWorld(new Vector2(0, 0));

				Vector2 MMAvatar1 = m.camera.ScreenToWorld(new Vector2(539, 127));
				Vector2 MMAvatarFrame = m.camera.ScreenToWorld(new Vector2(539, 127));

				Vector2 MMButton = m.camera.ScreenToWorld(new Vector2(45, 500));

				s.Draw(Main.Textures["MMBackground"], MMBackground, null, null, new Vector2(0, 0), 0, new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), Color.White);

				Vector2 mPos = m.camera.ScreenToWorld(m.inputHandler.MousePos);

                for (int i = 0; i < MMTextFields.Length; i++) {
                    if ((int)selected != i) {
                        if (InBounds(mPos.X, mPos.Y, m.camera.ScreenToWorld(MMTextFields[i]).X, m.camera.ScreenToWorld(MMTextFields[i]).Y, MMTextFieldSize.X/m.camera.Zoom, MMTextFieldSize.Y/m.camera.Zoom)) {
                            s.Draw(Main.Textures["MMTextOn"], m.camera.ScreenToWorld(MMTextFields[i]), null, null, new Vector2(0, 0), 0, new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), Color.White);
                        } else {
                            s.Draw(Main.Textures["MMTextOff"], m.camera.ScreenToWorld(MMTextFields[i]), null, null, new Vector2(0, 0), 0, new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), Color.White);
                        }
					} else {
						s.Draw(Main.Textures["MMTextSelected"], m.camera.ScreenToWorld(MMTextFields[i]), null, null, new Vector2(0, 0), 0, new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), Color.White);
					}
					s.DrawString(font, data[i], m.camera.ScreenToWorld(MMTextFields[i] + new Vector2(1)), Color.Black, 0, new Vector2(0, 0), new Vector2(0.5f / m.camera.Zoom, 0.5f / m.camera.Zoom), SpriteEffects.None, 1f);
				}

				s.Draw(Main.Textures["Avatar"], MMAvatar1, null, null, new Vector2(0, 0), 0, new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), Color.White);
				s.Draw(Main.Textures["MMAvatarSelecter"], MMAvatarFrame, null, null, new Vector2(0, 0), 0, new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), Color.White);


				if (data[0] != "" && data[1] != "" && data[2] != "" && data[3] != "") {
					if (InBounds(m.inputHandler.MousePos.X, m.inputHandler.MousePos.Y, 45, 500, 86, 25)) {
						s.Draw(Main.Textures["MMConnectHover"], MMButton, null, null, new Vector2(0, 0), 0, new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), Color.White);
					} else {
						s.Draw(Main.Textures["MMConnect"], MMButton, null, null, new Vector2(0, 0), 0, new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), Color.White);
					}
				} else {
					s.Draw(Main.Textures["MMConnectDisabled"], MMButton, null, null, new Vector2(0, 0), 0, new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), Color.White);
				}
			}

			if (InfoOpen == true) {
				Vector2 vec1 = new Vector2(630, 0); //this makes me feel ill
				Vector2 Backgroundloc = m.camera.ScreenToWorld(vec1);
				Vector2 vec2 = new Vector2(630+20, 0+30);
				Vector2 InfoTitleOffsetV = m.camera.ScreenToWorld(vec2);
				Vector2 vec3 = new Vector2(630 + 20, 0 + 90);
				Vector2 SelectedTitleV = m.camera.ScreenToWorld(vec3);
				Vector2 vec4 = new Vector2(630 + 20, 0 + 115);
				Vector2 SelectedTitleLocV = m.camera.ScreenToWorld(vec4);
				Vector2 vec5 = new Vector2(630 + 20, 0 + 140);
				Vector2 SelectedTitleStaV = m.camera.ScreenToWorld(vec5);

				Vector2 vec6 = new Vector2(630 + 20, 0 + 175);
				Vector2 SelectedTitleStaNameV = m.camera.ScreenToWorld(vec6);
				Vector2 vec7 = new Vector2(630 + 20, 0 + 200);
				Vector2 SelectedTitleStaHPV = m.camera.ScreenToWorld(vec7);
				Vector2 vec8 = new Vector2(630 + 20, 0 + 225);
				Vector2 SelectedTitleStaAPV = m.camera.ScreenToWorld(vec8);
				Vector2 vec9 = new Vector2(630 + 20, 0 + 250);
				Vector2 SelectedTitleStaAVaV = m.camera.ScreenToWorld(vec9);

				Vector2 vec10 = new Vector2(630 + 20, 0 + 280);
				Vector2 SelectedTitleStaPicV = m.camera.ScreenToWorld(vec10);

				s.Draw(Main.Textures["UI"], Backgroundloc, null, null, new Vector2(0, 0), 0, new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), Color.White);
				s.DrawString(Main.Fonts["UIFont"], "Information Panel [I to Hide]\nClick To Scan Tile...", InfoTitleOffsetV, Color.White, 0, new Vector2(0, 0), new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), SpriteEffects.None, 1f);

				if (ClickedInGUI) {
					s.DrawString(Main.Fonts["UIFont"], "Seltected Tile:", SelectedTitleV, Color.Gray, 0, new Vector2(0, 0), new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), SpriteEffects.None, 1f);
					s.DrawString(Main.Fonts["UIFont"], "Location: x: "+m.map.TileSelected.X+" y: "+m.map.TileSelected.Y, SelectedTitleLocV, Color.Gray, 0, new Vector2(0, 0), new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), SpriteEffects.None, 1f);
					if (m.map.getData((int)m.map.TileSelected.X, (int)m.map.TileSelected.Y) == null) {
						s.DrawString(Main.Fonts["UIFont"], "Tile Status: Empty", SelectedTitleStaV, Color.Gray, 0, new Vector2(0, 0), new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), SpriteEffects.None, 1f);
					} else {
						Unit UnitData = m.map.getData((int)m.map.TileSelected.X, (int)m.map.TileSelected.Y);
						s.DrawString(Main.Fonts["UIFont"], "Tile Status: Occupied", SelectedTitleStaV, Color.Green, 0, new Vector2(0, 0), new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), SpriteEffects.None, 1f);
						s.DrawString(Main.Fonts["UIFont"], "Unit Name: "+UnitData.Username, SelectedTitleStaNameV, Color.LightSeaGreen, 0, new Vector2(0, 0), new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), SpriteEffects.None, 1f);
						s.DrawString(Main.Fonts["UIFont"], "Unit HP: " + UnitData.HP, SelectedTitleStaHPV, Color.LightSeaGreen, 0, new Vector2(0, 0), new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), SpriteEffects.None, 1f);
						s.DrawString(Main.Fonts["UIFont"], "Unit AP: " + UnitData.AP, SelectedTitleStaAPV, Color.LightSeaGreen, 0, new Vector2(0, 0), new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), SpriteEffects.None, 1f);
						s.DrawString(Main.Fonts["UIFont"], "Unit Avatar:", SelectedTitleStaAVaV, Color.LightSeaGreen, 0, new Vector2(0, 0), new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), SpriteEffects.None, 1f);
						s.Draw(UnitData.Avatar, SelectedTitleStaPicV, null, null, new Vector2(0, 0), 0, new Vector2(4 / m.camera.Zoom, 4 / m.camera.Zoom), Color.White);
					}
				}
			}
		}

		private const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private Stopwatch sw = new Stopwatch();
		public void update() {
			if (!sw.IsRunning)
				sw.Start();
			if (m.gameState == GameState.Map) {
				if (m.inputHandler.isKeyDown(Keys.I)) {
					InfoOpen = !InfoOpen;
					ClickedInGUI = false;
				}
				if (m.inputHandler.isMBtnDown(0) && InfoOpen == true) {
					ClickedInGUI = true;
				}
			} else if (m.gameState == GameState.MainMenu) {
				if (m.inputHandler.isMBtnDown(0)) {
					Vector2 mPos = m.camera.ScreenToWorld(m.inputHandler.MousePos);
					for (int i = 0; i < MMTextFields.Length; i++) {
						if (InBounds(mPos.X, mPos.Y, m.camera.ScreenToWorld(MMTextFields[i]).X, m.camera.ScreenToWorld(MMTextFields[i]).Y, MMTextFieldSize.X/m.camera.Zoom, MMTextFieldSize.Y/m.camera.Zoom)) {
							selected = (MMTextFieldSelected)i;
							break;
						}
					}
					if (data[0] != "" && data[1] != "" && data[2] != "" && data[3] != "")
						if (InBounds(m.inputHandler.MousePos.X, m.inputHandler.MousePos.Y, 45, 500, 86, 25))
							handleConnect();
				}
				if (selected != MMTextFieldSelected.None) {
					#region Typing
					if (sw.ElapsedMilliseconds >= 100) {
						bool reset = false;
						foreach (Keys k in Enum.GetValues(typeof(Keys)).Cast<Keys>()) {
							if (m.inputHandler.isKeyPressed(k)) {
								if (validChars.Contains(k.ToString()) || k.ToString().ToLower().Contains("numpad") || (k.ToString().ToLower().Contains('d') && k.ToString().Length == 2)) {
									if (k.ToString().Contains("NumPad")) {
										data[(int)selected] += k.ToString().Substring(6, 1);
										reset = true;
									} else {
										if (m.inputHandler.isKeyPressed(Keys.LeftShift) || m.inputHandler.isKeyPressed(Keys.RightShift)) {
											data[(int)selected] += k.ToString();
											reset = true;
										} else {
											if (k.ToString().Length > 1) {
												if (Char.IsDigit(k.ToString().ToCharArray()[1])) {
													data[(int)selected] += k.ToString().ToCharArray()[1];
													reset = true;
												}
											} else {
												data[(int)selected] += k.ToString().ToLower();
												reset = true;
											}
										}
									}
								} else if (k == Keys.Back) {
									if (data[(int)selected].Length > 0) {
										data[(int)selected] = data[(int)selected].Substring(0, data[(int)selected].Length-1);
										reset = true;
									}
								} else if (k == Keys.OemPeriod) {
									if (m.inputHandler.isKeyPressed(k)) {
										data[(int)selected] += '.';
										reset = true;
									}
								}
							}
                            if (data[(int)selected].Length > 19) {
                                data[(int)selected] = data[(int)selected].Substring(0, 19);
                            }
						}
						if (reset) {
							sw.Restart();
						}
					}
					#endregion
				}
			}
		}

		private void handleConnect() {
			//TODO: Input validation
			if (m.isServer) {
				m.server = new Server(data[(int)MMTextFieldSelected.IPAddress], Convert.ToInt32(data[(int)MMTextFieldSelected.Port]), m);
			} else {
				m.client = new Client(data[(int)MMTextFieldSelected.IPAddress], Convert.ToInt32(data[(int)MMTextFieldSelected.Port]), m);
				IPEndPoint localAddr = ClientSocketHandler.clientSocket.LocalEndPoint as IPEndPoint;
				PlayerPacket p = new PlayerPacket(localAddr, data[(int)MMTextFieldSelected.UserName], data[(int)MMTextFieldSelected.Password], PlayerStatus.Connecting, true, true);
				Client.ToSendPacket.Enqueue(new NetPacket(null, p));
			}
		}
	}
}