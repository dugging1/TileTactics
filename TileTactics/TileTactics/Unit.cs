using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileTactics
{
    public class Unit
    {
        public int AP;
        public int HP;
        public Texture2D Avatar;
        public string Username;

        public Unit(string name) {
            HP = 3;
            Avatar = Main.Textures["Avatar"];
            Username = name;
        }

		public Unit(string name, int AP, int HP) {
			Username = name;
			this.AP = AP;
			this.HP = HP;
			Avatar = Main.Textures["Avatar"];
		}

        public Unit(Unit Clone)
        {
            AP = Clone.AP;
            HP = Clone.HP;
            Avatar = Clone.Avatar;
            Username = Clone.Username;
        }

        public void draw(SpriteBatch s, Vector2 Origin)
        {
            Vector2 AvatarOffset = new Vector2(16, 16);
            Vector2 APBannerOffset = new Vector2(16,50);
            Vector2 HeartOffset1 = new Vector2(16, 5);
            Vector2 HeartOffset2 = new Vector2(28, 5);
            Vector2 HeartOffset3 = new Vector2(40, 5);

            if (AP != 0)
            {
                s.Draw(Avatar, Origin + AvatarOffset);
            }
            else
            {
                s.Draw(Main.Textures["OffAvatar"], Origin + AvatarOffset);
            }

            s.Draw(Main.Textures["APBanner"], Origin + APBannerOffset);

            if (HP >= 1)
            {
                s.Draw(Main.Textures["Heart"], Origin+HeartOffset1);
            }
            if (HP >= 2)
            {
                s.Draw(Main.Textures["Heart"], Origin + HeartOffset2);
            }
            if (HP >= 3)
            {
                s.Draw(Main.Textures["Heart"], Origin + HeartOffset3);
            }
        }
    }
}
