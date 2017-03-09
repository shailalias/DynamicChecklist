﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using DynamicChecklist.ObjectLists;


namespace DynamicChecklist
{

    public class MainClass : Mod
    {
        public ObjectCollection objectCollection;
        public Keys OpenMenuKey = Keys.NumPad1;
        private ModConfig Config;
        private Texture2D cropsTexture;
        private IModHelper helper;
        private List<ObjectList> objectLists = new List<ObjectList>();

        public override void Entry(IModHelper helper)
        {
            this.helper = helper;
            this.Config = helper.ReadConfig<ModConfig>();          
            // Menu Events
            MenuEvents.MenuChanged += MenuChangedEvent;
            ControlEvents.KeyPressed += this.ReceiveKeyPress;
            SaveEvents.AfterLoad += this.GameLoadedEvent;
            GameEvents.GameLoaded += this.onGameLoaded;
            GraphicsEvents.OnPreRenderHudEvent += this.drawTick;
            try
            {
                OpenMenuKey = (Keys)Enum.Parse(typeof(Keys), Config.OpenMenuKey);
            }
            catch
            {
                // use default value
            }


        }
        private void drawTick(object sender, EventArgs e)
        {
            if (Game1.currentLocation == null || Game1.gameMode == 11 || Game1.currentMinigame != null || Game1.showingEndOfNightStuff || Game1.gameMode == 6 || Game1.gameMode == 0 || Game1.menuUp || Game1.activeClickableMenu != null)
            {
                return;
            }
            foreach (ObjectList ol in objectLists)
            {
                ol.updateObjectInfo();
                ol.draw(Game1.spriteBatch);
            }
        }

        private void onGameLoaded(object sender, EventArgs e)
        {
            OverlayTextures.loadTextures(helper.DirectoryPath);
            cropsTexture = loadTexture("Crops.png");

            initializeObjectLists();
        }
        private void initializeObjectLists()
        {
            objectLists.Add(new PettingList());
        }
        private Texture2D loadTexture(String texName)
        {
            var textureStream = new FileStream(Path.Combine(Helper.DirectoryPath, "Resources", texName), FileMode.Open);
            var t = Texture2D.FromStream(Game1.graphics.GraphicsDevice, textureStream);
            return t;
        }
        private void ReceiveKeyPress(object sender, EventArgsKeyPressed e)
        {
            //TODO ignore close menu when entering checkbox name
            if (e.KeyPressed != OpenMenuKey) return;
            if (Game1.activeClickableMenu is ChecklistMenu)
            {
                Game1.activeClickableMenu = null;
            }
            else
            {
                objectCollection.update();
                ChecklistMenu.objectCollection = objectCollection;
                ChecklistMenu.Open();
            }

        }
        public void MenuChangedEvent(object sender, EventArgsClickableMenuChanged e)
        {
            if (!(e.NewMenu is GameMenu))
            {
                return;
            }
            var gameMenu = e.NewMenu;

        }
        public void GameLoadedEvent(object sender, EventArgs e)
        {
            objectCollection = new ObjectCollection(cropsTexture);
            objectCollection.update();
        }
    }

    internal class ModConfig
    {
        public bool val { get; set; } = false;
        public String OpenMenuKey = "NumPad1";
    }
}
