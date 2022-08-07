using System;
using Celeste;
using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.SSMHelper.Entities
{
    /// <summary>
    /// Kills the player when she goes off the top of the screen.
    /// </summary>
    [CustomEntity("SSMHelper/ZeroGravBoundsController")]
    public class ZeroGravBoundsController : Entity
    {
        public ZeroGravBoundsController() : base() { }

        public override void Update()
        {
            Level level = SceneAs<Level>();
            Player player = level.Tracker.GetEntity<Player>();
            if (player != null)
            {
                // kill player if she goes off the top of the screen
                if (player.Bottom < level.Bounds.Top)
                {
                    if (SaveData.Instance.Assists.Invincible)
                    {
                        player.Play("event:/game/general/assist_screenbottom");
                        player.BounceDown();
                    }
                    else
                    {
                        player.Die(Vector2.Zero);
                    }
                }
            }
        }
    }
}