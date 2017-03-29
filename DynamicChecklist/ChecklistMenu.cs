﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Menus;
using StardewValley;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using DynamicChecklist.ObjectLists;
using StardewValley.BellsAndWhistles;

namespace DynamicChecklist
{    
    public class ChecklistMenu : IClickableMenu
    {
        public static List<ObjectList> objectLists;

        private Rectangle MenuRect;
        public List<OptionsElement> options = new List<OptionsElement>();

        private static int iSelectedTab = 0;

        private List<ClickableComponent> tabs = new List<ClickableComponent>();
        private enum TabName { Checklist, Settings}
        private List<string> tabNames = new List<string>{"Checklist", "Settings"};

        private ClickableComponent selectedTab;

        private ModConfig config;

        public ChecklistMenu(ModConfig config)
        {
            this.config = config;

            Game1.playSound("bigSelect");
            MenuRect = createCenteredRectangle(Game1.viewport, 800, 600);
            initialize(MenuRect.X, MenuRect.Y, MenuRect.Width, MenuRect.Height, true);



            int lblWidth = 150;
            int lblx = (int)(this.xPositionOnScreen - lblWidth);
            int lbly = (int)(this.yPositionOnScreen + 20);
            int lblSeperation = 80;
            int lblHeight = 60;
            int i = 0;
            foreach (string s in Enum.GetNames(typeof(TabName)))
            {
                tabs.Add(new ClickableComponent(new Rectangle(lblx, lbly + lblSeperation * i++, lblWidth, lblHeight), s));
            }

            selectedTab = tabs[iSelectedTab];
            int lineHeight = 50;
            switch (selectedTab.name)
            {
                case "Checklist":
                    int j = 0;
                    foreach(ObjectList ol in objectLists)
                    {
                        if (ol.ShowInMenu )
                        {
                            var checkbox = new DynamicSelectableCheckbox(ol);
                            checkbox.bounds = new Rectangle(MenuRect.X + 50, MenuRect.Y + 50 + lineHeight * j, 100, 50);
                            options.Add(checkbox);
                            j++;
                        }                                           
                    }
                    break;
                case "Settings":
                    // TODO: Implement
                    break;
                default:
                    throw (new NotImplementedException());
            }


        }

        public override void receiveKeyPress(Keys key)
        {
            foreach(OptionsElement o in options)
            {
                o.receiveKeyPress(key);
            }
            base.receiveKeyPress(key);
        }
        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            SpriteText.drawStringWithScrollCenteredAt(b, "Checklist", this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen - Game1.tileSize, "", 1f, -1, 0, 0.88f, false);
            drawTextureBox(Game1.spriteBatch, MenuRect.X, MenuRect.Y, MenuRect.Width, MenuRect.Height, Color.White);
            var mouseX = Game1.getMouseX();
            var mouseY = Game1.getMouseY();

            int j = 0;
            foreach(ClickableComponent t in tabs)
            {                
                drawTextureBox(Game1.spriteBatch, t.bounds.X, t.bounds.Y, t.bounds.Width, t.bounds.Height, Color.White* (iSelectedTab == j ? 1F : 0.7F));
                b.DrawString(Game1.smallFont, t.name, new Vector2(t.bounds.X+15, t.bounds.Y+15), Color.Black);
                j++;
            }
            foreach(OptionsElement o in options)
            {
                o.draw(b, -1, -1);
            }
            base.draw(b);
            drawMouse(b);
        }

        private static Rectangle createCenteredRectangle(xTile.Dimensions.Rectangle v, int width, int height)
        {
            var x = v.Width / 2 - width/2;
            var y = v.Height / 2 - height/2;
            return new Rectangle(x, y, width, height);

        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            for(int i=0; i<tabs.Count; i++ )
            {
                if (tabs[i].bounds.Contains(x, y))
                {
                    iSelectedTab = i;
                    Game1.activeClickableMenu = new ChecklistMenu(config);
                }

            }
            foreach(OptionsElement o in options)
            {
                if (o.bounds.Contains(x, y))
                {
                    o.receiveLeftClick(x, y);
                }

            }
        }
        public static void Open(ModConfig config)
        {
            Game1.activeClickableMenu = new ChecklistMenu(config);
        }

        public override void receiveRightClick(int x, int y, bool playSound = false)
        {

        }
    }

}


