using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileTactics
{
    public class Map
    {
        private Unit[] mapData = new Unit[70*70];
        public Unit getData(int x, int y) { return mapData[y * 70 + x]; }

        public void setData(int x, int y, Unit value) {
            mapData[y * 70 + x] = value;
        }


        public void draw(SpriteBatch s, Main m)
        {
            for (int x = 0; x < 70; x++)
            {
                for (int y = 0; y < 70; y++)
                {
                    if (getData(x,y) == null)
                    {
                        s.Draw(m.Textures["Tile"], new Vector2((x * 64), (y * 64)));
                    }
                    else
                    {
                        getData(x,y).draw(s, new Vector2((x * 64), (y * 64)));
                    }
                }
            }
        }

        public bool MoveCheck(Vector2 Origin, Vector2 Endpoint)
        {
            if (Origin.X == Endpoint.X && (Origin.Y == Endpoint.Y - 1 || Origin.Y == Endpoint.Y + 1))
                return true;
            else if (Origin.Y == Endpoint.Y && (Origin.X == Endpoint.X - 1 || Origin.X == Endpoint.X + 1))
                return true;
            else if (((Origin.X == Endpoint.X + 1) || (Origin.X == Endpoint.X - 1)) && ((Origin.Y == Endpoint.Y + 1) || Origin.Y == Endpoint.Y - 1))
                return true;
            else
                return false;
        }
    }
}
