using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.SSMHelper.Entities
{
    [CustomEntity("SSMHelper/SeekerCrushZone")]
    [Tracked]
    public class SeekerCrushZone : Entity
    {
        private bool activated = false;

        private Seeker capturedSeeker;

        private Hitbox detectHitbox;

        public SeekerCrushZone(Vector2 position, int width, int height, 
            char tile1, char tile2)
            : base(position)
        {
            Collider = new Hitbox(width, height);
            detectHitbox = new Hitbox(width - 10f, height - 10f, 5f, 5f);
        }

        public SeekerCrushZone(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Width, data.Height,
                  data.Char("tile1", 'g'), data.Char("tile2", 'G'))
        {
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
        }

        public override void Update()
        {
            base.Update();
            if (!activated)
            {
                Collider collider = Collider;
                Collider = detectHitbox;
                Level level = SceneAs<Level>();
                foreach (Seeker seeker in level.Tracker.GetEntities<Seeker>())
                {
                    if (seeker.CollideCheck(this))
                    {
                        activated = true;
                        capturedSeeker = seeker;
                        SeekerCrushZoneBlock block = level.Tracker.GetNearestEntity<SeekerCrushZoneBlock>(Position);
                        block?.Activate(this);
                        break;
                    }
                }
                Collider = collider;
            }
            else if (capturedSeeker != null)
            {
                KeepInside(capturedSeeker);
            }
        }

        private void KeepInside(Seeker seeker)
        {
            seeker.Left = Math.Max(seeker.Left, this.Left + 6f);
            seeker.Right = Math.Min(seeker.Right, this.Right - 6f);
            seeker.Top = Math.Max(seeker.Top, this.Top + 6f);
            seeker.Bottom = Math.Min(seeker.Bottom, this.Bottom - 6f);
        }

        public new void Render()
        {
            base.Render();
            Vector2 position = Position - SceneAs<Level>().Camera.Position;
            Draw.Rect(position + Vector2.One, Width - 2, Height - 2, Color.Violet * 0.15f);
            Draw.HollowRect(position, Width, Height, Color.Violet * 0.5f);
        }

        public override void DebugRender(Camera camera)
        {
            base.DebugRender(camera);
            Collider collider = Collider;
            Collider = detectHitbox;
            Draw.HollowRect(Collider, Color.MediumSeaGreen);
            Collider = collider;
        }

        public static void Load()
        {
            On.Celeste.BackdropRenderer.Render += On_BackdropRenderer_Render;
        }

        public static void Unload()
        {
            On.Celeste.BackdropRenderer.Render -= On_BackdropRenderer_Render;
        }

        private static void On_BackdropRenderer_Render(On.Celeste.BackdropRenderer.orig_Render orig, BackdropRenderer self, Scene scene)
        {
            orig(self, scene);
            // render seeker crush zones like FG backdrops (to ignore lighting effects)
            if (scene.Tracker.CountEntities<SeekerCrushZone>() > 0 && self == (scene as Level)?.Foreground)
            {
                self.StartSpritebatch(BlendState.AlphaBlend);
                foreach (SeekerCrushZone zone in scene.Tracker.GetEntities<SeekerCrushZone>())
                {
                    if (zone.Visible)
                    {
                        zone.Render();
                    }
                }
                self.EndSpritebatch();
            }
        }
    }
}
