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
        public int AC;
        public int HP;
        public Texture2D Avatar;
        public string Username;

        public Unit(Main m, string name) {
            HP = 3;
            Avatar = m.Textures["Avatar"];
            Username = name;
        }

        public void draw(SpriteBatch s, Vector2 Origin)
        {
            s.Draw(Avatar, Origin);
        }
    }
}
