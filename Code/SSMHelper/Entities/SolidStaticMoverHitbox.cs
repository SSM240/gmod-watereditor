using Monocle;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoMod.Utils;

namespace Celeste.Mod.SSMHelper.Entities
{
    /// <summary>
    /// Forces the parent platform of the static mover to ignore this collider in collision checks.
    /// </summary>
    public class SolidStaticMoverHitbox : Hitbox
    {
        private StaticMover staticMover;

        public SolidStaticMoverHitbox(float width, float height, float x = 0, float y = 0, StaticMover staticMover = null)
            : base(width, height, x, y)
        {
            this.staticMover = staticMover;
        }

        public SolidStaticMoverHitbox(Hitbox hitbox, StaticMover staticMover = null)
            : this(hitbox.Width, hitbox.Height, hitbox.Position.X, hitbox.Position.Y, staticMover)
        {
        }

        public override bool Collide(Hitbox hitbox)
        {
            if (staticMover?.Platform == hitbox.Entity)
            {
                return false;
            }
            return base.Collide(hitbox);
        }

        public static void Load()
        {
            On.Monocle.Hitbox.Collide_Hitbox += On_Hitbox_Collide_Hitbox;
        }

        public static void Unload()
        {
            On.Monocle.Hitbox.Collide_Hitbox -= On_Hitbox_Collide_Hitbox;
        }

        private static bool On_Hitbox_Collide_Hitbox(On.Monocle.Hitbox.orig_Collide_Hitbox orig, Hitbox self, Hitbox hitbox)
        {
            if (hitbox is SolidStaticMoverHitbox && self is not SolidStaticMoverHitbox)
            {
                // reverse order so that the static mover hitbox is the one handling the collision
                return hitbox.Collide(self);
            }
            return orig(self, hitbox);
        }
    }
}
