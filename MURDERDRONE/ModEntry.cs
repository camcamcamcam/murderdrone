using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Framework.RewriteFacades;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;

namespace MURDERDRONE
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetLoader
    {
        readonly float r = 80f;
        float t;
        readonly float speed = 1f/120f;
        readonly float offsetY = 20f;
        readonly float offsetX = 5f;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += TimeEvents_AfterDayStarted;
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Critter\Drone");
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            return Helper.Content.Load<T>(@"Assets\drone_sprite.png", ContentSource.ModFolder);
        }

        /*********
        ** Private methods
        *********/
        /// <summary>
        /// The method called after the player loads their save.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            Helper.Events.Player.Warped += PlayerEvents_Warped;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.Q)
            {
                foreach (var npc in Game1.currentLocation.getCharacters())
                {
                    if (npc.IsMonster)
                    {
                        Monitor.Log(npc.getName());
                        Monitor.Log(WithinPlayerThreshold(6, npc).ToString());
                    }
                }
            }
        }

        /// <summary>
        /// The method called when the player warps.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void PlayerEvents_Warped(object sender, WarpedEventArgs e)
        {
            if (!e.IsLocalPlayer || Game1.CurrentEvent != null)
                return;

            if (e.NewLocation is MineShaft == false && e.NewLocation is Farm == false)
                return;

            if (Game1.getCharacterFromName("Drone") is NPC == false)
            {
                e.NewLocation.addCharacter(new NPC(new AnimatedSprite(@"Critter\Drone", 0, 12, 12), new Vector2(Game1.player.Position.X, Game1.player.Position.Y), 1, "Drone"));
                Game1.getCharacterFromName("Drone").Sprite.loop = false; /// TODO: set to true late please
                Game1.getCharacterFromName("Drone").Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
                {
                    new FarmerSprite.AnimationFrame(1, 100),
                    new FarmerSprite.AnimationFrame(2, 100),
                    new FarmerSprite.AnimationFrame(3, 100),
                    new FarmerSprite.AnimationFrame(4, 100),
                    new FarmerSprite.AnimationFrame(5, 100),
                    new FarmerSprite.AnimationFrame(6, 100),
                    new FarmerSprite.AnimationFrame(0, 100)
                });

                Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
                Helper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
            }
            else
                Game1.warpCharacter(Game1.getCharacterFromName("Drone"), e.NewLocation, new Vector2(Game1.player.Position.X, Game1.player.Position.Y));
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Game1.getCharacterFromName("Drone") is NPC drone)
            {
                float newX = Game1.player.Position.X + offsetX + r * (float)Math.Cos(t * 2 * Math.PI);
                float newY = Game1.player.Position.Y - offsetY + r * (float)Math.Sin(t * 2 * Math.PI);
                drone.Position = new Vector2(newX, newY);

                t = (t + speed) % 1;
            }
        }

        private void GameLoop_OneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {

        }

        private bool WithinPlayerThreshold(int threshold, NPC npc)
        {
            if (npc.currentLocation != null && !npc.currentLocation.Equals((object)Game1.currentLocation))
                return false;

            Vector2 tileLocation1 = Game1.player.getTileLocation();
            Vector2 tileLocation2 = npc.getTileLocation();

            double d = Math.Abs(Math.Sqrt(
                Math.Pow(Math.Abs(tileLocation2.X - tileLocation1.X), 2) +
                Math.Pow(Math.Abs(tileLocation2.Y - tileLocation1.Y), 2)
            ));

            return d <= threshold;
        }
    }
} 