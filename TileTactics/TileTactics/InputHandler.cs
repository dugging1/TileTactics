using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileTactics {
	public class InputHandler {
		KeyboardState curState;
		KeyboardState lastState;
		public Vector2 MousePos;
		MouseState curMState;
		MouseState lastMState;
		public int MWheelPos { get { return curMState.ScrollWheelValue; } }
		public int deltaMWheelPos { get { return MWheelPos-lastMState.ScrollWheelValue; } }
		
		public bool isKeyDown(Keys k) { //First frame of key down
			if (curState == null) return false;
			if (lastState == null) return curState.IsKeyDown(k);
			return curState.IsKeyDown(k) && !lastState.IsKeyDown(k);
		}

		public bool isKeyPressed(Keys k) {
			return curState.IsKeyDown(k);
		}

		public bool isKeyUp(Keys k) { //First frame of key up
			if (curState == null) return false;
			if (lastState == null) return curState.IsKeyUp(k);
			return curState.IsKeyUp(k) && !lastState.IsKeyUp(k);
		}

		public bool isMBtnDown(int btn) {
			switch (btn) {
				case 0:
					return curMState.LeftButton == ButtonState.Pressed && !(lastMState.LeftButton == ButtonState.Pressed);
				case 1:
					return curMState.RightButton == ButtonState.Pressed && !(lastMState.RightButton == ButtonState.Pressed);
				case 2:
					return curMState.MiddleButton == ButtonState.Pressed && !(lastMState.MiddleButton == ButtonState.Pressed);
				default:
					return false;
			}
		}

		public bool isMBtnPressed(int btn) {
			switch (btn) {
				case 0:
					return curMState.LeftButton == ButtonState.Pressed;
				case 1:
					return curMState.RightButton == ButtonState.Pressed;
				case 2:
					return curMState.MiddleButton == ButtonState.Pressed;
				default:
					return false;
			}
		}

		public bool isMBtnUp(int btn) {
			switch (btn) {
				case 0:
					return curMState.LeftButton == ButtonState.Released && !(lastMState.LeftButton == ButtonState.Released);
				case 1:
					return curMState.RightButton == ButtonState.Released && !(lastMState.RightButton == ButtonState.Released);
				case 2:
					return curMState.MiddleButton == ButtonState.Released && !(lastMState.MiddleButton == ButtonState.Released);
				default:
					return false;
			}
		}

		public void update() {
			lastState = curState;
			curState = Keyboard.GetState();
			MousePos = Mouse.GetState().Position.ToVector2();
			lastMState = curMState;
			curMState = Mouse.GetState();
		}
	}
}
