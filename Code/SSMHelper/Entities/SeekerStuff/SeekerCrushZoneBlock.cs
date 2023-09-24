using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.SSMHelper.Entities
{
    [CustomEntity("SSMHelper/SeekerCrushZoneBlock")]
    [Tracked]
    public class SeekerCrushZoneBlock : Solid
    {
        private TileGrid tilesStart;

        private TileGrid tilesEnd;

        private SeekerCrushZone crushZone;

        private Vector2 badelinePosition;

        private bool finished;

        public SeekerCrushZoneBlock(Vector2 position, Vector2[] nodes, int width, int height,
            char tile1, char tile2)
            : base(position, width, height, safe: false)
        {
            badelinePosition = nodes[0];
            int newSeed = Calc.Random.Next();
            Calc.PushRandom(newSeed);
            Add(tilesStart = GFX.FGAutotiler.GenerateBox(tile1, width / 8, height / 8).TileGrid);
            Calc.PopRandom();
            Calc.PushRandom(newSeed);
            Add(tilesEnd = GFX.FGAutotiler.GenerateBox(tile2, width / 8, height / 8).TileGrid);
            tilesEnd.Alpha = 0f;
            Calc.PopRandom();
            Add(new TileInterceptor(tilesStart, highPriority: true));
            Add(new LightOcclude());
        }

        public SeekerCrushZoneBlock(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.NodesOffset(offset), data.Width, data.Height,
                  data.Char("tile1", 'g'), data.Char("tile2", 'G'))
        {
        }

        public override void Render()
        {
            base.Render();
            // copied from FinalBossMovingBlock
            if (!finished && tilesEnd.Alpha > 0f && tilesEnd.Alpha < 1f)
            {
                int num = (int)((1f - tilesEnd.Alpha) * 16f);
                Rectangle rect = new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
                rect.Inflate(num, num);
                Draw.HollowRect(rect, Color.Lerp(Color.Purple, Color.Pink, 0.7f));
            }
        }

        public override void OnShake(Vector2 amount)
        {
            base.OnShake(amount);
            tilesStart.Position += amount;
            tilesEnd.Position += amount;
        }

        public void Activate(SeekerCrushZone zone)
        {
            crushZone = zone;
            Add(new Coroutine(Sequence()));
        }

        public IEnumerator Sequence()
        {
            Level level = Scene as Level;
            BadelineDummy badeline = CreateBadeline(badelinePosition);
            badeline.Sprite.Play(PlayerSprite.FallCarry);
            badeline.Appear(level, silent: true);
            Scene.Add(badeline);
            Audio.Play(SFX.char_bad_booster_begin);
            StartShaking(2f);
            AddFadeInTween();
            crushZone.Visible = false;

            yield return 0.25f;
            float playerWaitTimer = 1f;
            while (crushZone.CollideCheck<Player>() && playerWaitTimer > 0f)
            {
                playerWaitTimer -= Engine.DeltaTime;
                yield return null;
            }
            StopShaking();

            Add(new Coroutine(BadelineThrow(badeline)));
            AddMoveTween();

            yield return 0.75f;

            badeline.Vanish();
            finished = true;
            AddFadeOutTween();
        }

        private BadelineDummy CreateBadeline(Vector2 position)
        {
            BadelineDummy badeline = new BadelineDummy(position);
            // change the sprite so the carry and throw animations can be used
            badeline.Remove(badeline.Sprite);
            badeline.Add(badeline.Sprite = new PlayerSprite(PlayerSpriteMode.MadelineAsBadeline));
            badeline.Hair.Sprite = badeline.Sprite;
            badeline.Sprite.Scale.X = Math.Sign(CenterX - badeline.CenterX);
            return badeline;
        }

        private IEnumerator BadelineThrow(BadelineDummy badeline)
        {
            badeline.Sprite.Play(PlayerSprite.Throw);
            while (badeline.Sprite.CurrentAnimationFrame < 2)
            {
                yield return null;
            }
            badeline.Sprite.Rate = 0f;
        }

        private void AddMoveTween()
        {
            Vector2 from = Position;
            // move this entity's bottom center to the crush zone's bottom center
            Vector2 to = crushZone.BottomCenter - new Vector2(Width / 2, Height);
            Tween moveTween = Tween.Create(Tween.TweenMode.Oneshot, Ease.QuintIn, 0.25f, start: true);
            moveTween.OnUpdate = (t) =>
            {
                MoveTo(Vector2.Lerp(from, to, t.Eased));
            };
            moveTween.OnComplete = (t) =>
            {
                if (CollideCheck<SolidTiles>(Position + (to - from).SafeNormalize() * 2f))
                {
                    Audio.Play("event:/game/06_reflection/fallblock_boss_impact", Center);
                    ImpactParticles(to - from);
                }
            };
            Add(moveTween);
        }

        private void AddFadeInTween()
        {
            Tween visualTween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, 0.25f, start: true);
            visualTween.OnUpdate = (t) =>
            {
                tilesEnd.Alpha = t.Eased;
                tilesStart.Alpha = 1 - tilesEnd.Alpha;
            };
            visualTween.OnComplete = (t) =>
            {
                tilesEnd.Alpha = 1f;
                tilesStart.Alpha = 0f;
            };
            Add(visualTween);
        }

        private void AddFadeOutTween()
        {
            Tween visualTween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, 0.25f, start: true);
            visualTween.OnUpdate = (t) =>
            {
                tilesEnd.Alpha = 1 - t.Eased;
                tilesStart.Alpha = 1 - tilesEnd.Alpha;
            };
            visualTween.OnComplete = (t) =>
            {
                tilesEnd.Alpha = 0f;
                tilesStart.Alpha = 1f;
            };
            Add(visualTween);
        }

        // entirely copypasted & cleaned up from FinalBossMovingBlock
        private void ImpactParticles(Vector2 moved)
        {
            Level level = SceneAs<Level>();
            bool collided = false;
            if (moved.X < 0f)
            {
                Vector2 offset = new Vector2(0f, 2f);
                for (int i = 0; i < Height / 8f; i++)
                {
                    Vector2 leftEdge = new Vector2(Left - 1f, Top + 4f + (i * 8));
                    if (!Scene.CollideCheck<Water>(leftEdge) && Scene.CollideCheck<Solid>(leftEdge))
                    {
                        collided = true;
                        level.ParticlesFG.Emit(CrushBlock.P_Impact, leftEdge + offset, 0f);
                        level.ParticlesFG.Emit(CrushBlock.P_Impact, leftEdge - offset, 0f);
                    }
                }
            }
            else if (moved.X > 0f)
            {
                Vector2 offset = new Vector2(0f, 2f);
                for (int j = 0; j < Height / 8f; j++)
                {
                    Vector2 rightEdge = new Vector2(Right + 1f, Top + 4f + (j * 8));
                    if (!Scene.CollideCheck<Water>(rightEdge) && Scene.CollideCheck<Solid>(rightEdge))
                    {
                        collided = true;
                        level.ParticlesFG.Emit(CrushBlock.P_Impact, rightEdge + offset, (float)Math.PI);
                        level.ParticlesFG.Emit(CrushBlock.P_Impact, rightEdge - offset, (float)Math.PI);
                    }
                }
            }
            if (moved.Y < 0f)
            {
                Vector2 offset = new Vector2(2f, 0f);
                for (int k = 0; k < Width / 8f; k++)
                {
                    Vector2 topEdge = new Vector2(Left + 4f + (k * 8), Top - 1f);
                    if (!Scene.CollideCheck<Water>(topEdge) && Scene.CollideCheck<Solid>(topEdge))
                    {
                        collided = true;
                        level.ParticlesFG.Emit(CrushBlock.P_Impact, topEdge + offset, (float)Math.PI / 2f);
                        level.ParticlesFG.Emit(CrushBlock.P_Impact, topEdge - offset, (float)Math.PI / 2f);
                    }
                }
            }
            else if (moved.Y > 0f)
            {
                Vector2 offset = new Vector2(2f, 0f);
                for (int l = 0; l < Width / 8f; l++)
                {
                    Vector2 bottomEdge = new Vector2(Left + 4f + (l * 8), Bottom + 1f);
                    if (!Scene.CollideCheck<Water>(bottomEdge) && Scene.CollideCheck<Solid>(bottomEdge))
                    {
                        collided = true;
                        level.ParticlesFG.Emit(CrushBlock.P_Impact, bottomEdge + offset, -(float)Math.PI / 2f);
                        level.ParticlesFG.Emit(CrushBlock.P_Impact, bottomEdge - offset, -(float)Math.PI / 2f);
                    }
                }
            }
            if (collided)
            {
                level.DirectionalShake(moved);
            }
        }
    }
}
