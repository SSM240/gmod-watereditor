﻿using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Celeste.Mod.SSMHelper.Entities
{
    [Tracked]
    [CustomEntity("SSMHelper/RedirectableBooster")]
    public class RedirectableBooster : Booster
    {
        // vanilla speed is 240
        public const float BoostSpeed = 220f;

        public static VirtualButton StopButton => Input.Dash;

        public static ParticleType P_BurstBlue;
        public static ParticleType P_BurstPink;
        public static ParticleType P_PinkAppear;
        public static ParticleType P_PinkBoosterGlow;

        private static ParticleType[] burstParticleTypes;

        public bool IsStopped;

        public Vector2 AimDirection;

        private Sprite[] sprites = new Sprite[3];
        private int currentSprite;

        private BoosterDashAssistArrow dashAssistArrow;

        public RedirectableBooster(EntityData data, Vector2 offset) : base(data.Position + offset, true)
        {

            Add(dashAssistArrow = new BoosterDashAssistArrow());

            Add(sprites[0] = SSMHelperModule.SpriteBank.Create("boosterBlue"));
            sprites[1] = sprite;
            Add(sprites[2] = SSMHelperModule.SpriteBank.Create("boosterPink"));
            SetSprite(2);
        }

        public override void Update()
        {
            base.Update();

            if (!BoostingPlayer && currentSprite == 2 && Scene.OnInterval(0.15f))
            {
                SceneAs<Level>().ParticlesFG.Emit(P_PinkBoosterGlow, 2, sprite.RenderPosition, Vector2.One * 11f);
            }

            if (!BoostingPlayer)
            {
                IsStopped = false;
                return;
            }
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            if (StopButton.Pressed && player.Dashes > 0)
            {
                StopButton.ConsumeBuffer();
                IsStopped = true;
                OnStop(player);
            }
            if (IsStopped)
            {
                if (Input.Aim.Value != Vector2.Zero)  // don't override direction if not pressing anything
                {
                    AimDirection = Input.GetAimVector();
                }
                if (!StopButton.Check)
                {
                    IsStopped = false;
                    OnResume(player);
                }
            }
        }

        public override void Render()
        {
            base.Render();
            if (AimDirection.X != 0)
            {
                sprite.FlipX = Math.Sign(AimDirection.X) < 0;
            }
        }

        private void OnStop(Player player)
        {
            loopingSfx.Stop();
            Audio.Play(SFX.game_05_redbooster_enter);
            sprite.Play("inside");
            wiggler.Start();
            dashAssistArrow.Visible = true;

            player.Speed = Vector2.Zero;
        }

        private void OnResume(Player player)
        {
            Audio.Play(SFX.game_05_redbooster_dash);
            loopingSfx.Play(SFX.game_05_redbooster_move_loop);
            loopingSfx.DisposeOnTransition = false;
            sprite.Play("spin");
            dashAssistArrow.Visible = false;

            player.Dashes = Math.Max(0, player.Dashes - 1);
            AimDirection = AimDirection.CorrectDashPrecision();
            player.DashDir = AimDirection;
            player.Speed = player.DashDir * BoostSpeed;
            if (player.DashDir.X != 0f)
            {
                player.Facing = (Facings)Math.Sign(player.DashDir.X);
            }

            Celeste.Freeze(0.05f);
            Dust.Burst(player.Position, (-player.DashDir).Angle(), 8, null);
            SceneAs<Level>().Displacement.AddBurst(player.Center + playerOffset, 0.5f, 0f, 80f, 0.666f, Ease.QuadOut, Ease.QuadOut);
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
        }

        // copypasted from vanilla but with different graphical behavior
        // easier than IL hooking a coroutine
        private IEnumerator BoostRoutine(Player player)
        {
            while ((player.StateMachine.State == Player.StRedDash) && BoostingPlayer)
            {
                SetSprite(player.Dashes);
                sprite.RenderPosition = player.Center + playerOffset;
                loopingSfx.Position = sprite.Position;
                if (!IsStopped && Scene.OnInterval(0.02f))
                {
                    Vector2 dir = player.DashDir;
                    float angle = (-dir).Angle();
                    (Scene as Level).ParticlesBG.Emit(particleType, 2, player.Center - dir * 3f + new Vector2(0f, -2f), new Vector2(3f, 3f), angle);
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

        private void SetSprite(int index)
        {
            index = Calc.Clamp(index, 0, 2);
            if (index == currentSprite)
            {
                return;
            }
            foreach (Sprite sprite in sprites)
            {
                sprite.Visible = false;
            }
            if (!string.IsNullOrEmpty(sprite.CurrentAnimationID))
            {
                sprites[index].Play(sprite.CurrentAnimationID);
            }
            sprites[index].Visible = true;
            sprite = sprites[index];
            particleType = burstParticleTypes[index];
            currentSprite = index;
        }

        private new void AppearParticles()
        {
            ParticleSystem particlesBG = SceneAs<Level>().ParticlesBG;
            for (int i = 0; i < 360; i += 30)
            {
                particlesBG.Emit(P_PinkAppear, 1, Center, Vector2.One * 2f, i * ((float)Math.PI / 180f));
            }
        }

        private static BindingFlags publicInstance = BindingFlags.Public | BindingFlags.Instance;
        private static Hook playerCanDashHook;
        private static MethodInfo playerCanDashInfo = typeof(Player).GetProperty("CanDash", publicInstance).GetGetMethod();

        public static void Load()
        {
            On.Celeste.Booster.PlayerBoosted += On_Booster_PlayerBoosted;
            On.Celeste.Booster.Respawn += On_Booster_Respawn;
            On.Celeste.Booster.AppearParticles += On_Booster_AppearParticles;
            playerCanDashHook = new Hook(playerCanDashInfo, On_Player_CanDash);
            On.Celeste.Player.RefillDash += On_Player_RefillDash;
        }

        public static void Unload()
        {
            On.Celeste.Booster.PlayerBoosted -= On_Booster_PlayerBoosted;
            On.Celeste.Booster.Respawn -= On_Booster_Respawn;
            On.Celeste.Booster.AppearParticles -= On_Booster_AppearParticles;
            playerCanDashHook?.Dispose();
            playerCanDashHook = null;
            On.Celeste.Player.RefillDash -= On_Player_RefillDash;
        }

        public static void LoadParticles()
        {
            P_BurstBlue = new ParticleType(P_BurstRed)
            {
                Color = Calc.HexToColor("27678b")
            };
            P_BurstPink = new ParticleType(P_BurstRed)
            {
                Color = Calc.HexToColor("a84a99")
            };
            burstParticleTypes = new ParticleType[3]
            {
                P_BurstBlue,
                P_BurstRed,
                P_BurstPink
            };
            P_PinkAppear = new ParticleType(P_RedAppear)
            {
                Color = Calc.HexToColor("ff7ffa")
            };
            P_PinkBoosterGlow = new ParticleType(Refill.P_Glow)
            {
                Color = Calc.HexToColor("e27ecc"),
                Color2 = Calc.HexToColor("ffe0f8")
            };
        }

        private static void On_Booster_PlayerBoosted(On.Celeste.Booster.orig_PlayerBoosted orig, Booster self, Player player, Vector2 direction)
        {
            orig(self, player, direction);
            if (self is RedirectableBooster booster)
            {
                // refill dash manually (since we're hooking RefillDash)
                player.Dashes = player.MaxDashes;
                // replace with our own coroutine (easiest way to stop the particles)
                booster.dashRoutine.Replace(booster.BoostRoutine(player));
                // reduce player speed
                player.Speed = player.Speed.WithMagnitude(BoostSpeed);

                booster.AimDirection = player.DashDir;
            }
        }

        private static void On_Booster_Respawn(On.Celeste.Booster.orig_Respawn orig, Booster self)
        {
            if (self is RedirectableBooster booster)
            {
                booster.SetSprite(2);
            }
            orig(self);
        }

        private static void On_Booster_AppearParticles(On.Celeste.Booster.orig_AppearParticles orig, Booster self)
        {
            if (self is not RedirectableBooster booster)
            {
                orig(self);
                return;
            }
            booster.AppearParticles();
        }

        private static bool On_Player_CanDash(Func<Player, bool> orig, Player self)
        {
            bool result = orig(self);
            if (self.LastBooster is RedirectableBooster booster && booster.BoostingPlayer)
            {
                result = false;
            }
            return result;
        }

        private static bool On_Player_RefillDash(On.Celeste.Player.orig_RefillDash orig, Player self)
        {
            // this method does not affect refill gems for some reason so it works for me :)
            if (self.LastBooster is RedirectableBooster booster && booster.BoostingPlayer)
            {
                Engine.Scene.OnEndOfFrame += () =>
                {
                    // refill dash anyway if player state has changed by the end of the frame
                    if (self.StateMachine.State != Player.StRedDash)
                    {
                        orig(self);
                    }
                };
                return false;
            }
            return orig(self);
        }
    }
}
