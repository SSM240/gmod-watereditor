using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.SSMHelper.Triggers
{
    [CustomEntity("SSMHelper/DashCancelTrigger")]
    public class DashCancelTrigger : Trigger
    {
        private Vector2 playerCurrentPosition;
        private Vector2 playerLastPosition;

        public DashCancelTrigger(EntityData data, Vector2 offset)
            : base(data, offset)
        {
        }

        public override void Update()
        {
            base.Update();
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            if (player != null)
            {
                playerLastPosition = playerCurrentPosition;
                playerCurrentPosition = player.ExactPosition;
            }
        }

        public override void OnStay(Player player)
        {
            base.OnStay(player);
            CancelDash(player);
        }

        private void CancelDash(Player player)
        {
            if (player.StateMachine.State == Player.StDash 
                && player.DashDir != Vector2.Zero
                && playerCurrentPosition != playerLastPosition)
            {
                Audio.Play(SFX.game_04_whiteblock_fallthru);
                player.StateMachine.ForceState(Player.StNormal);
                // convert player's distance traveled last frame into actual speed
                player.Speed = (playerCurrentPosition - playerLastPosition) / Engine.DeltaTime;
                DynData<Player> playerData = new(player);
                playerData.Set("forceMoveXTimer", 0f);
            }
        }
    }
}
