using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileTactics
{
    class GUI
    {
        public void draw(SpriteBatch s, Main m)
        {
            Vector2 vec = new Vector2(0, 1620);
            s.Draw(Main.Textures["UI"], m.camera.ScreenToWorld(1620, 0));
            //Vector2 t = new Vector2(vec.X * mat.m11 + vec.Y * mat.m12, vec.X * mat * m21 + vec.Y * mat * 22);
        }
    }
}
