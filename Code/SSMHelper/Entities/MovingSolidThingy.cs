using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.SSMHelper.Entities
{
    [CustomEntity("SSMHelper/MovingSolidThingy")]
    public class MovingSolidThingy : Solid
    {
        public MovingSolidThingy(Vector2 position, float width, float height)
            : base(position, width, height, safe: false)
        {
        }

        public MovingSolidThingy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Width, data.Height)
        {
        }

        public override void Update()
        {
            base.Update();
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            if (player != null)
            {
                MoveH(player.CenterX - this.CenterX);
            }
        }

        public override void Render()
        {
            base.Render();
            Draw.Rect(base.Collider, Color.White);
        }
    }
}
