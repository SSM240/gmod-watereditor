using Monocle;

namespace Celeste.Mod.SSMHelper.Entities
{
    /// <summary>
    /// Forces the parent platform of the static mover to ignore this collider in collision checks.
    /// </summary>
    public class SolidStaticMoverHitbox : ColliderList
    {
        private StaticMover staticMover;

        public SolidStaticMoverHitbox(float width, float height, float x = 0, float y = 0, StaticMover staticMover = null)
            : base(new Hitbox(width, height, x, y))
        {
            this.staticMover = staticMover;
        }

        public SolidStaticMoverHitbox(Hitbox hitbox, StaticMover staticMover = null)
            : base(hitbox)
        {
            this.staticMover = staticMover;
        }

        public override bool Collide(Hitbox hitbox)
        {
            if (staticMover?.Platform == hitbox.Entity)
            {
                return false;
            }
            return base.Collide(hitbox);
        }
    }
}
