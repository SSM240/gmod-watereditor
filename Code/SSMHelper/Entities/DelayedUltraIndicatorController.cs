using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.SSMHelper.Entities
{
    [CustomEntity("SSMHelper/DelayedUltraIndicatorController")]
    public class DelayedUltraIndicatorController : Entity
    {
        private static readonly Vector2 renderOffset = new(0, -8);
        private static readonly Color outlineColor = Calc.HexToColor("081e08");
        private static readonly Color renderColor = Calc.HexToColor("83dc83");

        private Vector2 renderPos;

        public DelayedUltraIndicatorController() : base()
        {
            Depth = Depths.Top - 2;
        }

        public override void Update()
        {
            if (SceneAs<Level>()?.Tracker.GetEntity<Player>() is not Player player)
            {
                return;
            }

            Visible = player.DashDir.Y > 0 && player.DashDir.X != 0;

            if (Visible)
            {
                renderPos = player.TopCenter + renderOffset;
            }
        }

        public override void Render()
        {
            Draw.Circle(renderPos, 2, outlineColor, 2);
            Draw.Rect(renderPos - Vector2.One, 2, 2, renderColor);
        }
    }
}
