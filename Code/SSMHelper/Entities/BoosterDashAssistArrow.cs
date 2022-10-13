using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SSMHelper.Entities
{
    /// <summary>
    /// Largely copied from <see cref="PlayerDashAssist"/>.
    /// </summary>
    [Tracked(false)]
    public class BoosterDashAssistArrow : Component
    {
        public float Direction;

        public float Scale;

        public Vector2 Offset;

        private List<MTexture> images;

        private int lastIndex;

        private bool wasVisibleLastFrame;

        public BoosterDashAssistArrow()
            : base(active: true, visible: false)
        {
            images = GFX.Game.GetAtlasSubtextures("util/dasharrow/dasharrow");
        }

        public override void Update()
        {
            if (Entity is not RedirectableBooster booster)
            {
                return;
            }
            float inputDir = booster.AimDirection.Angle();
            if (!Visible || !wasVisibleLastFrame || Calc.AbsAngleDiff(inputDir, Direction) >= 1.58079636f)
            {
                Direction = inputDir;
                Scale = 0f;
            }
            else
            {
                Direction = Calc.AngleApproach(Direction, inputDir, (float)Math.PI * 6f * Engine.RawDeltaTime);
            }
            Scale = Calc.Approach(Scale, 1f, Engine.DeltaTime * 4f);
            int num2 = 1 + (8 + (int)Math.Round(inputDir / ((float)Math.PI / 4f))) % 8;
            if (lastIndex != 0 && lastIndex != num2)
            {
                Audio.Play("event:/game/general/assist_dash_aim", booster.Center, "dash_direction", num2);
            }
            lastIndex = num2;
            wasVisibleLastFrame = Visible;
        }

        public override void Render()
        {
            if (Entity is not RedirectableBooster booster)
            {
                return;
            }
            MTexture mTexture = null;
            float num = float.MaxValue;
            for (int i = 0; i < 8; i++)
            {
                float num2 = Calc.AngleDiff((float)Math.PI * 2f * ((float)i / 8f), Direction);
                if (Math.Abs(num2) < Math.Abs(num))
                {
                    num = num2;
                    mTexture = images[i];
                }
            }
            if (mTexture != null)
            {
                if (Math.Abs(num) < 0.05f)
                {
                    num = 0f;
                }
                mTexture.DrawOutlineCentered((booster.sprite.RenderPosition + Offset + Calc.AngleToVector(Direction, 20f)).Round(), Color.White, Ease.BounceOut(Scale), num);
            }
        }
    }
}
