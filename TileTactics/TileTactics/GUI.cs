﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileTactics
{
    public class GUI
    {
        public bool InfoOpen = false;
        public bool ClickedInGUI = false;

        public void draw(SpriteBatch s, Main m)
        {
            if (m.inputHandler.isKeyDown(Keys.I))
            {
                if (InfoOpen == true)
                {
                    InfoOpen = false;
                    ClickedInGUI = false;
                }
                else
                {
                    InfoOpen = true;
                }
            }
            if (m.inputHandler.isMBtnDown(0) && InfoOpen == true)
            {
                ClickedInGUI = true;
            }
            if (InfoOpen == true)
            {
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
                s.DrawString(Main.Fonts["UIFont"], "Information Panel [I to Hide]\nClick To Scan Tile...", InfoTitleOffsetV, Color.White, 0, new Vector2(0,0), new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom),SpriteEffects.None,1f);

                if (ClickedInGUI)
                {
                    s.DrawString(Main.Fonts["UIFont"], "Seltected Tile:", SelectedTitleV, Color.Gray, 0, new Vector2(0, 0), new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), SpriteEffects.None, 1f);
                    s.DrawString(Main.Fonts["UIFont"], "Location: x: "+m.map.TileSelected.X+" y: "+m.map.TileSelected.Y, SelectedTitleLocV, Color.Gray, 0, new Vector2(0, 0), new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), SpriteEffects.None, 1f);
                    if (m.map.getData((int)m.map.TileSelected.X,(int)m.map.TileSelected.Y) == null)
                    {
                        s.DrawString(Main.Fonts["UIFont"], "Tile Status: Empty", SelectedTitleStaV, Color.Gray, 0, new Vector2(0, 0), new Vector2(1 / m.camera.Zoom, 1 / m.camera.Zoom), SpriteEffects.None, 1f);
                    }
                    else
                    {
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
    }
}