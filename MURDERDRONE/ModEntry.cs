using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace MURDERDRONE
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetLoader
    {
        private ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            Helper.Events.GameLoop.Saving += GameLoop_SaveCreated;
            Helper.Events.Input.ButtonPressed += Input_ButtonPressed;

            if (Config.Active)
                Helper.Events.Player.Warped += PlayerEvents_Warped;
        }

        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Helper\Drone");
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
        private void GameLoop_SaveCreated(object sender, SavingEventArgs e)
        {
            RemoveDrone();
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree || Game1.currentMinigame != null)
                return;
            Type type = typeof(StardewModdingAPI.SButton);
            if (e.Button == (SButton)Enum.Parse(type, Config.KeyboardShortcut, true))
            {
                if (Config.Active)
                {
                    RemoveDrone();
                    Helper.Events.Player.Warped -= PlayerEvents_Warped;
                    Config.Active = false;
                }
                else
                {
                    AddDrone();
                    Helper.Events.Player.Warped += PlayerEvents_Warped;
                    Config.Active = true;
                }

                Helper.WriteConfig(Config);
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

            AddDrone();
        }

        private void RemoveDrone()
        {
            if (Game1.getCharacterFromName("Drone") is Drone drone)
            {
                Game1.removeThisCharacterFromAllLocations(drone);
            }
        }

        private bool AddDrone()
        {
            if (Game1.currentLocation is MineShaft == false && Game1.currentLocation is Farm == false)
                return false;

            if (Game1.getCharacterFromName("Drone") is NPC == false)
                Game1.currentLocation.addCharacter(new Drone(Config.RotationSpeed, Config.Damage, (float)Config.ProjectileVelocity));
            else
                Game1.warpCharacter(Game1.getCharacterFromName("Drone"), Game1.currentLocation, Game1.player.Position);

            return true;
        }
    }
} 