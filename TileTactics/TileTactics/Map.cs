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

        public bool MoveCheck(Vector2 Origin, Vector2 Endpoint) //checks radi of 1 tile around Origin
        {
            return (Origin - Endpoint).Length() <= Math.Pow(2, 0.5f) && getData((int)Endpoint.X, (int)Endpoint.Y) == null;
        }

        public bool AimCheck(Vector2 Origin, Vector2 Endpoint) //checks radi of 3 tiles around Origin
        {
            if (((Origin - Endpoint).Length() <= Math.Pow(18, 0.5f) && getData((int)Endpoint.X, (int)Endpoint.Y) == null))
                if (Endpoint != Origin + new Vector2(0, 4) && Endpoint != Origin + new Vector2(4, 0) && Endpoint != Origin + new Vector2(-4, 0) && Endpoint != Origin + new Vector2(0, -4))
                    return true;
            return false;
        }

        public bool MoveUnit(Vector2 CurrentLocation,Vector2 NewLocation)
        {
            if (MoveCheck(CurrentLocation,NewLocation) && getData((int)CurrentLocation.X, (int)CurrentLocation.Y).AP >= 1)
            {
                Unit Temp = getData((int)CurrentLocation.X, (int)CurrentLocation.Y);
                Temp.AP = Temp.AP - 1;
                setData((int)NewLocation.X, (int)NewLocation.Y, new Unit(Temp));
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
