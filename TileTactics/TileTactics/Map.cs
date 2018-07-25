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
        public Vector2 TileSelected;

        public void setData(int x, int y, Unit value) {
            mapData[y * 70 + x] = value;
        }

        public void dailyAPBoost()
        {
            foreach (Unit User in mapData)
            {
                if (User != null)
                {
                    User.AP = User.AP + 1;
                }
            }
        }

        public void draw(SpriteBatch s, Main m)
        {
            for (int x = 0; x < 70; x++)
            {
                for (int y = 0; y < 70; y++)
                {
                    Vector2 TempVector = m.camera.ScreenToWorld(m.inputHandler.MousePos) / 64;
                    if (Math.Floor(TempVector.X) == x && Math.Floor(TempVector.Y) == y)
                    {
                        s.Draw(Main.Textures["TileSelected"], new Vector2((x * 64), (y * 64)));
                        if (m.inputHandler.isMBtnPressed(0))
                        {
                            TileSelected = new Vector2(x, y);
                        }
                    }
                    else
                    {
                        s.Draw(Main.Textures["Tile"], new Vector2((x * 64), (y * 64)));
                    }
                    
                    if (getData(x,y) != null)
                    {
                        getData(x, y).draw(s, new Vector2((x * 64), (y * 64)));
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

        public bool Attack(Vector2 CurrentLocation, Vector2 EndLocation)
        {
            if (getData((int)CurrentLocation.X, (int)CurrentLocation.Y).AP >= 1 && getData((int)EndLocation.X, (int)EndLocation.Y) != null)
            {
                if (getData((int)EndLocation.X, (int)EndLocation.Y).HP == 1)
                {
                    setData((int)EndLocation.X, (int)EndLocation.Y, null);
                    //THIS IS NOT FINISHED - UPDATE TO HANDLE MOVING PLAYERS TO DEAD LIST!
                }
                else
                {
                    getData((int)EndLocation.X, (int)EndLocation.Y).HP = getData((int)EndLocation.X, (int)EndLocation.Y).HP - 1;
                }
                getData((int)CurrentLocation.X, (int)CurrentLocation.Y).AP = getData((int)CurrentLocation.X, (int)CurrentLocation.Y).AP - 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ShootAP(Vector2 CurrentLocation, Vector2 EndLocation)
        {
            if (getData((int)CurrentLocation.X, (int)CurrentLocation.Y).AP >= 1 && getData((int)EndLocation.X, (int)EndLocation.Y) != null)
            {
                getData((int)CurrentLocation.X, (int)CurrentLocation.Y).AP = getData((int)CurrentLocation.X, (int)CurrentLocation.Y).AP - 1;
                getData((int)EndLocation.X, (int)EndLocation.Y).AP = getData((int)EndLocation.X, (int)EndLocation.Y).AP + 1;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
