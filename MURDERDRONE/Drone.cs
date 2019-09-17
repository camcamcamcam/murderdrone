using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.Monsters;
using System.Xml.Serialization;

namespace MURDERDRONE
{
    public class Drone : NPC
    {
        private readonly float r = 80f;
        private float t;
        private readonly float offsetY = 20f;
        private readonly float offsetX = 5f;
        private bool throwing;
        private bool thrown;
        private Monster target;
        private BasicProjectile basicProjectile;
        private int damage;
        private float projectileVelocity;

        public Drone()
        {
        }

        public Drone(int speed, int damage, float projectileVelocity)
        : base(new AnimatedSprite(@"Helper\Drone", 0, 12, 12), Game1.player.Position, 1, "Drone")
        {
            this.speed = speed;
            this.hideShadow.Value = true;
            this.damage = damage;
            this.projectileVelocity = projectileVelocity;
        }

        public override bool CanSocialize => false;

        public override bool canTalk()
        {
            return false;
        }

        public override void doEmote(int whichEmote, bool playSound, bool nextEventCommand = true)
        {
        }

        public override void update(GameTime time, GameLocation location)
        {
            float newX = Game1.player.position.X + offsetX + r * (float)Math.Cos(t * 2 * Math.PI);
            float newY = Game1.player.position.Y - offsetY + r * (float)Math.Sin(t * 2 * Math.PI);
            position.Set(new Vector2(newX, newY));

            t = (t + (float)time.ElapsedGameTime.TotalMilliseconds/(1000 * speed)) % 1;

            if (!throwing)
            {
                foreach (var npc in Game1.currentLocation.getCharacters())
                {
                    if (npc.IsMonster && npc.withinPlayerThreshold(3))
                    {
                        if (npc is Bug bug && bug.isArmoredBug)
                            continue;

                        throwing = true;
                        target = (Monster)npc;
                        break;
                    }
                }
            }

            if (throwing && target.IsMonster)
                ShootTheBastard(time, location, target);
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
        }

        public virtual void ShootTheBastard(GameTime time, GameLocation location, Monster monster)
        {
            if (!thrown)
            {
                if (damage == -1)
                {
                    damage = monster.maxHealth;
                }

                Vector2 velocityTowardMonster = Utility.getVelocityTowardPoint(new Vector2(position.X, position.Y), new Vector2(monster.position.X, monster.position.Y), projectileVelocity);
                basicProjectile = new BasicProjectile(
                    damage,
                    Projectile.throwingKnife,
                    0,
                    0,
                    1f,
                    velocityTowardMonster.X,
                    velocityTowardMonster.Y,
                    position,
                    collisionSound: "hitEnemy",
                    firingSound: "daggerswipe",
                    explode: true,
                    damagesMonsters: true,
                    location: location,
                    firer: this
                );

                location.projectiles.Add((Projectile) basicProjectile);
                Logger.Log((Math.PI / (64.0 + Game1.random.Next(-63, 64))).ToString());
                thrown = true;
            }

            if (thrown && basicProjectile is BasicProjectile && basicProjectile.destroyMe)
            {
                throwing = false;
                thrown = false;
                basicProjectile = null;
            }
        }
    }
}