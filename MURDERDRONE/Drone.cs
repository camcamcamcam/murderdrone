using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using System.Collections.Generic;
using StardewValley.Characters;

namespace MURDERDRONE
{
    public class Drone : NPC
    {
        readonly float r = 80f;
        float t;
        new readonly float speed = 1f / 120f;
        readonly float offsetY = 20f;
        readonly float offsetX = 5f;

        public Drone()
        : base(new AnimatedSprite(@"Critter\Drone", 0, 12, 12), new Vector2(Game1.player.Position.X, Game1.player.Position.Y), 1, "Drone")
        {
            Sprite.loop = true;
            Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
            {
                new FarmerSprite.AnimationFrame(1, 100),
                new FarmerSprite.AnimationFrame(2, 100),
                new FarmerSprite.AnimationFrame(3, 100),
                new FarmerSprite.AnimationFrame(4, 100),
                new FarmerSprite.AnimationFrame(5, 100),
                new FarmerSprite.AnimationFrame(6, 100),
                new FarmerSprite.AnimationFrame(0, 100)
            });
        }

        public override void update(GameTime time, GameLocation location)
        {
            float newX = Game1.player.Position.X + offsetX + r * (float)Math.Cos(t * 2 * Math.PI);
            float newY = Game1.player.Position.Y - offsetY + r * (float)Math.Sin(t * 2 * Math.PI);
            Position = new Vector2(newX, newY);

            t = (t + speed) % 1;
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
        }
    }
}