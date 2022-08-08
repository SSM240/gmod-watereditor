using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SSMHelper
{
    public static class Extensions
    {
        /// <summary>
        /// Emulates much of the "Bounce" method but going down instead.
        /// </summary>
        /// <param name="player"></param>
        public static void BounceDown(this Player player)
        {
            if (player.StateMachine.State == 4 && player.CurrentBooster != null)
            {
                player.CurrentBooster.PlayerReleased();
                player.CurrentBooster = null;
            }
            if (!player.Inventory.NoRefills)
            {
                player.RefillDash();
            }
            player.RefillStamina();
            player.StateMachine.State = 0;
            Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
            player.Sprite.Scale = new Vector2(0.6f, 1.4f);
            player.Speed.Y = 160f;
        }

        /// <summary>
        /// Returns a new <see cref="Vector2"/> with <paramref name="vec"/>'s angle and <paramref name="mag"/>'s magnitude.
        /// </summary>
        public static Vector2 WithMagnitude(this Vector2 vec, float mag)
        {
            return Calc.AngleToVector(vec.Angle(), mag);
        }
    }
}
