using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.SSMHelper.Entities
{
    [Tracked]
    [CustomEntity("SSMHelper/RedirectableBooster")]
    public class RedirectableBooster : Booster
    {
        public bool IsStopped;

        public Vector2 AimDirection;

        // vanilla speed is 240
        public const float BoostSpeed = 220f;

        #region Base class fields/methods
        private readonly DynamicData baseData;

        public SoundSource loopingSfx => baseData.Get<SoundSource>("loopingSfx");
        public Sprite sprite
        {
            get => baseData.Get<Sprite>("sprite");
            set => baseData.Set("sprite", value);
        }
        public Coroutine dashRoutine => baseData.Get<Coroutine>("dashRoutine");
        public Wiggler wiggler => baseData.Get<Wiggler>("wiggler");
        #endregion

        public RedirectableBooster(EntityData data, Vector2 offset) : base(data.Position + offset, true)
        {
            baseData = new DynamicData(typeof(Booster), this);
        }

        public override void Update()
        {
            base.Update();
            if (!BoostingPlayer)
            {
                IsStopped = false;
                return;
            }
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            if (Input.Jump.Pressed && player.Dashes > 0)
            {
                Input.Jump.ConsumeBuffer();
                IsStopped = true;
                OnStop(player);
            }
            if (IsStopped)
            {
                if (Input.Aim.Value != Vector2.Zero)  // don't override direction if not pressing anything
                {
                    AimDirection = Input.GetAimVector();
                }
                if (!Input.Jump.Check)
                {
                    IsStopped = false;
                    OnResume(player);
                }
            }
        }

        private void OnStop(Player player)
        {
            loopingSfx.Stop();
            Audio.Play(SFX.game_05_redbooster_enter);
            sprite.Play("loop", restart: true);
            wiggler.Start();
            AimDirection = player.DashDir;

            player.Speed = Vector2.Zero;
        }

        private void OnResume(Player player)
        {
            Audio.Play(SFX.game_05_redbooster_dash);
            loopingSfx.Play(SFX.game_05_redbooster_move_loop);
            loopingSfx.DisposeOnTransition = false;
            sprite.Play("spin");

            player.Dashes = Math.Max(0, player.Dashes - 1);
            AimDirection = AimDirection.CorrectDashPrecision();
            player.DashDir = AimDirection;
            player.Speed = player.DashDir * BoostSpeed;
            if (player.DashDir.X != 0f)
            {
                player.Facing = (Facings)Math.Sign(player.DashDir.X);
            }

            sprite.FlipX = player.Facing == Facings.Left;
        }

        // copypasted from vanilla but with different particle behavior
        // easier than IL hooking a coroutine
        private IEnumerator BoostRoutine(Player player, Vector2 dir)
        {
            float angle = (-dir).Angle();
            while ((player.StateMachine.State == Player.StDash || player.StateMachine.State == Player.StRedDash) && BoostingPlayer)
            {
                sprite.RenderPosition = player.Center + playerOffset;
                loopingSfx.Position = sprite.Position;
                if (Scene.OnInterval(0.02f))
                {
                    //(Scene as Level).ParticlesBG.Emit(particleType, 2, player.Center - dir * 3f + new Vector2(0f, -2f), new Vector2(3f, 3f), angle);
                }
                yield return null;
            }
            PlayerReleased();
            if (player.StateMachine.State == Player.StBoost)
            {
                sprite.Visible = false;
            }
            while (SceneAs<Level>().Transitioning)
            {
                yield return null;
            }
            Tag = 0;
        }

        public static void Load()
        {
            On.Celeste.Booster.PlayerBoosted += On_Booster_PlayerBoosted;
        }

        public static void Unload()
        {
            On.Celeste.Booster.PlayerBoosted -= On_Booster_PlayerBoosted;
        }

        private static void On_Booster_PlayerBoosted(On.Celeste.Booster.orig_PlayerBoosted orig, Booster self, Player player, Vector2 direction)
        {
            orig(self, player, direction);
            if (self is RedirectableBooster booster)
            {
                // replace with our own coroutine (easiest way to stop the particles)
                booster.dashRoutine.Replace(booster.BoostRoutine(player, direction));
                // reduce player speed
                player.Speed = player.Speed.WithMagnitude(BoostSpeed);
            }
        }
    }
}
