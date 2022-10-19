using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.SSMHelper.Entities
{
    [CustomEntity("SSMHelper/SeekerCrushBarrier")]
    public class SeekerCrushBarrier : SeekerBarrier
    {
        public static readonly Color FillColor = Calc.HexToColor("d365be");
        public static readonly Color ParticleColor = Calc.HexToColor("ff9ae2");

        private bool removing = false;
        private float removingFlashAlpha;
        private bool particlesVisible = true;

        private DynamicData baseData;
        private List<Vector2> particles => baseData.Get<List<Vector2>>("particles");
        private float solidifyDelay // why is this private -_-
        {
            get => baseData.Get<float>("solidifyDelay");
            set => baseData.Set("solidifyDelay", value);
        }

        public SeekerCrushBarrier(EntityData data, Vector2 offset)
            : base(data, offset)
        {
            baseData = new DynamicData(typeof(SeekerBarrier), this);
            Collidable = true;
            SurfaceSoundIndex = SurfaceIndex.DreamBlockActive;

            OnDashCollide = OnDashed;
            Add(new ClimbBlocker(edge: true));
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            scene.Tracker.GetEntity<SeekerCrushBarrierRenderer>().Track(this);
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            scene.Tracker.GetEntity<SeekerCrushBarrierRenderer>().Untrack(this);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            if (scene.Tracker.CountEntities<Seeker>() == 0)
            {
                RemoveSelf();
            }
        }

        public override void Update()
        {
            base.Update();

            if (!removing && SceneAs<Level>().Tracker.CountEntities<Seeker>() == 0)
            {
                removing = true;
                Add(new Coroutine(RemovalRoutine()));
            }
        }


        public override void Render()
        {
            if (particlesVisible)
            {
                Color color = ParticleColor * 0.5f;
                foreach (Vector2 particle in particles)
                {
                    Draw.Pixel.Draw(Position + particle, Vector2.Zero, color);
                }
            }
            if (Flashing)
            {
                Draw.Rect(Collider, Color.Lerp(FillColor, Color.White, 0.5f) * Flash * 0.5f);
            }
            if (removing)
            {
                float alpha = removingFlashAlpha;
                if (Settings.Instance.DisableFlashes)
                {
                    alpha *= 0.2f;
                }
                Draw.Rect(Collider, removingFlashColor * alpha);
            }
        }

        private Color removingFlashColor;

        private DashCollisionResults OnDashed(Player player, Vector2 direction)
        {
            Flash = 1f;
            Flashing = true;
            Solidify = 1f;
            solidifyDelay = 1f;
            Audio.Play("event:/game/03_resort/forcefield_bump", Position);
            return DashCollisionResults.Bounce;
        }

        private IEnumerator RemovalRoutine()
        {
            // todo: figure out better visual effects
            Solidify = 1f;
            solidifyDelay = 1f;
            removing = true;
            Tween flashFadeIn = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.15f, true);
            flashFadeIn.OnStart = (t) =>
            {
                removingFlashColor = FillColor;
                removingFlashAlpha = 0f;
            };
            flashFadeIn.OnUpdate = (t) =>
            {
                removingFlashColor = Color.Lerp(FillColor, Color.Lerp(FillColor, Color.White, 0.5f), t.Eased);
                removingFlashAlpha = t.Eased;
            };
            Add(flashFadeIn);
            yield return flashFadeIn.Wait();

            Collidable = false;
            SceneAs<Level>().Tracker.GetEntity<SeekerCrushBarrierRenderer>().Untrack(this);
            particlesVisible = false;

            Tween flashFadeOut = Tween.Create(Tween.TweenMode.Oneshot, Ease.Linear, 0.1f, true);
            flashFadeOut.OnStart = (t) =>
            {
                removingFlashColor = Color.Lerp(FillColor, Color.White, 0.5f);
                removingFlashAlpha = 1f;
            };
            flashFadeOut.OnUpdate = (t) =>
            {
                removingFlashAlpha = 1f - t.Eased;
            };
            Add(flashFadeOut);
            yield return flashFadeOut.Wait();

            RemoveSelf();
        }

        public static void Load()
        {
            On.Celeste.SeekerBarrierRenderer.Track += On_SeekerBarrierRenderer_Track;
            On.Celeste.SeekerBarrierRenderer.Untrack += On_SeekerBarrierRenderer_Untrack;
        }

        public static void Unload()
        {
            On.Celeste.SeekerBarrierRenderer.Track -= On_SeekerBarrierRenderer_Track;
            On.Celeste.SeekerBarrierRenderer.Untrack -= On_SeekerBarrierRenderer_Untrack;
        }

        // disable this from being tracked by the regular seeker barrier renderer
        private static void On_SeekerBarrierRenderer_Track(On.Celeste.SeekerBarrierRenderer.orig_Track orig,
            SeekerBarrierRenderer self, SeekerBarrier block)
        {
            if (block is SeekerCrushBarrier)
            {
                return;
            }
            orig(self, block);
        }
        private static void On_SeekerBarrierRenderer_Untrack(On.Celeste.SeekerBarrierRenderer.orig_Untrack orig,
            SeekerBarrierRenderer self, SeekerBarrier block)
        {
            if (block is SeekerCrushBarrier)
            {
                return;
            }
            orig(self, block);
        }
    }
}
