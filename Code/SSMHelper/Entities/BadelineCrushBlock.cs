using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.SSMHelper.Entities
{
    public class BadelineCrushBlock : Solid
    {
        private TileGrid tilesStart;

        private TileGrid tilesEnd;

        private SeekerCrushZone crushZone;

        private Vector2 badelinePosition;

        public BadelineCrushBlock(Vector2 position, char tile1, char tile2, int width, int height,
            Vector2 badelinePosition, SeekerCrushZone zone = null)
            : base(position, width, height, safe: false)
        {
            crushZone = zone;
            this.badelinePosition = badelinePosition;
            int newSeed = Calc.Random.Next();
            Calc.PushRandom(newSeed);
            Add(tilesStart = GFX.FGAutotiler.GenerateBox(tile1, width / 8, height / 8).TileGrid);
            Calc.PopRandom();
            Calc.PushRandom(newSeed);
            Add(tilesEnd = GFX.FGAutotiler.GenerateBox(tile2, width / 8, height / 8).TileGrid);
            tilesEnd.Alpha = 0f;
            Calc.PopRandom();
            Add(new TileInterceptor(tilesStart, highPriority: true));
        }

        public override void Render()
        {
            base.Render();
            // copied from FinalBossMovingBlock
            if (tilesEnd.Alpha > 0f && tilesEnd.Alpha < 1f)
            {
                int num = (int)((1f - tilesEnd.Alpha) * 16f);
                Rectangle rect = new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
                rect.Inflate(num, num);
                Draw.HollowRect(rect, Color.Lerp(Color.Purple, Color.Pink, 0.7f));
            }
        }

        public void Activate()
        {
            Add(new Coroutine(Sequence()));
        }

        public IEnumerator Sequence()
        {
            Level level = Scene as Level;
            BadelineDummy badeline = CreateBadeline(badelinePosition);
            badeline.Sprite.Play(PlayerSprite.IdleCarry);
            badeline.Appear(level, silent: true);
            Scene.Add(badeline);
            Audio.Play(SFX.char_bad_booster_begin);
            AddVisualTween();
            
            yield return 0.25f;

            Add(new Coroutine(BadelineThrow(badeline)));
            AddMoveTween();

            yield return 0.75f;

            badeline.Vanish();
        }

        private BadelineDummy CreateBadeline(Vector2 position)
        {
            BadelineDummy badeline = new BadelineDummy(position);
            badeline.Remove(badeline.Sprite);
            badeline.Add(badeline.Sprite = new PlayerSprite(PlayerSpriteMode.MadelineAsBadeline));
            badeline.Hair.Sprite = badeline.Sprite;
            badeline.Sprite.Scale.X = Math.Sign(Position.X - badeline.Position.X);
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
            Vector2 to = Position + new Vector2(0f, Height);
            Tween moveTween = Tween.Create(Tween.TweenMode.Oneshot, Ease.QuintIn, 0.25f, start: true);
            moveTween.OnUpdate = (t) =>
            {
                MoveTo(Vector2.Lerp(from, to, t.Eased));
            };
            moveTween.OnComplete = (t) =>
            {
                if (crushZone != null)
                {
                    crushZone.Visible = false;
                }
                if (CollideCheck<SolidTiles>(Position + (to - from).SafeNormalize() * 2f))
                {
                    Audio.Play("event:/game/06_reflection/fallblock_boss_impact", Center);
                    ImpactParticles(to - from);
                }
            };
            Add(moveTween);
        }

        private void AddVisualTween()
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

        /// <summary>
        /// Straight copypaste from <see cref="FinalBossMovingBlock"/>.
        /// </summary>
        /// <param name="moved"></param>
        private void ImpactParticles(Vector2 moved)
        {
            if (moved.X < 0f)
            {
                Vector2 value = new Vector2(0f, 2f);
                for (int i = 0; (float)i < base.Height / 8f; i++)
                {
                    Vector2 vector = new Vector2(base.Left - 1f, base.Top + 4f + (float)(i * 8));
                    if (!base.Scene.CollideCheck<Water>(vector) && base.Scene.CollideCheck<Solid>(vector))
                    {
                        SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector + value, 0f);
                        SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector - value, 0f);
                    }
                }
            }
            else if (moved.X > 0f)
            {
                Vector2 value2 = new Vector2(0f, 2f);
                for (int j = 0; (float)j < base.Height / 8f; j++)
                {
                    Vector2 vector2 = new Vector2(base.Right + 1f, base.Top + 4f + (float)(j * 8));
                    if (!base.Scene.CollideCheck<Water>(vector2) && base.Scene.CollideCheck<Solid>(vector2))
                    {
                        SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector2 + value2, (float)Math.PI);
                        SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector2 - value2, (float)Math.PI);
                    }
                }
            }
            if (moved.Y < 0f)
            {
                Vector2 value3 = new Vector2(2f, 0f);
                for (int k = 0; (float)k < base.Width / 8f; k++)
                {
                    Vector2 vector3 = new Vector2(base.Left + 4f + (float)(k * 8), base.Top - 1f);
                    if (!base.Scene.CollideCheck<Water>(vector3) && base.Scene.CollideCheck<Solid>(vector3))
                    {
                        SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector3 + value3, (float)Math.PI / 2f);
                        SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector3 - value3, (float)Math.PI / 2f);
                    }
                }
            }
            else
            {
                if (!(moved.Y > 0f))
                {
                    return;
                }
                Vector2 value4 = new Vector2(2f, 0f);
                for (int l = 0; (float)l < base.Width / 8f; l++)
                {
                    Vector2 vector4 = new Vector2(base.Left + 4f + (float)(l * 8), base.Bottom + 1f);
                    if (!base.Scene.CollideCheck<Water>(vector4) && base.Scene.CollideCheck<Solid>(vector4))
                    {
                        SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector4 + value4, -(float)Math.PI / 2f);
                        SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector4 - value4, -(float)Math.PI / 2f);
                    }
                }
            }
        }
    }
}
