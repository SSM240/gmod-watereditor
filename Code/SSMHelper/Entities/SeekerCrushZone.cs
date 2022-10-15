using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.SSMHelper.Entities
{
    [CustomEntity("SSMHelper/SeekerCrushZone")]
    public class SeekerCrushZone : Entity
    {
        private bool activated = false;

        private Seeker capturedSeeker;

        private Hitbox detectHitbox;

        private BadelineCrushBlock crushBlock;

        public SeekerCrushZone(Vector2 position, Vector2[] nodes, int width, int height, 
            char tile1, char tile2, int blockHeight)
            : base(position)
        {
            Collider = new Hitbox(width, height);
            detectHitbox = new Hitbox(width - 12f, height - 12f, 6f, 6f);

            Vector2 blockPosition = nodes[0];
            Vector2 badelinePosition = nodes[1];

            crushBlock = new BadelineCrushBlock(blockPosition, tile1, tile2, width, blockHeight, badelinePosition, this);
        }

        public SeekerCrushZone(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Nodes, data.Width, data.Height,
                  data.Char("tile1", '1'), data.Char("tile2", '2'), data.Int("blockHeight", 3))
        {
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            scene.Add(crushBlock);
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
                        crushBlock.Activate();
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

        public override void Render()
        {
            base.Render();
            Draw.Rect(Position + Vector2.One, Width - 2, Height - 2, Color.Violet * 0.15f);
            Draw.HollowRect(Collider, Color.Violet * 0.5f);
        }

        public override void DebugRender(Camera camera)
        {
            base.DebugRender(camera);
            Collider collider = Collider;
            Collider = detectHitbox;
            Draw.HollowRect(Collider, Color.MediumSeaGreen);
            Collider = collider;
        }
    }
}
