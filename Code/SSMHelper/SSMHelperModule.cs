using Microsoft.Xna.Framework;
using Monocle;
using System;
using Celeste.Mod.SSMHelper.Entities;
using Celeste.Mod.SSMHelper.Triggers;
using MonoMod.ModInterop;

namespace Celeste.Mod.SSMHelper
{
    public class SSMHelperModule : EverestModule
    {
        public static SSMHelperModule Instance;

        public static SpriteBank SpriteBank => Instance._CustomEntitySpriteBank;
        private SpriteBank _CustomEntitySpriteBank;

        public SSMHelperModule()
        {
            Instance = this;
        }

        public override void Load()
        {
            StarjumpTilesetHelper.Load();
            ReverseKillbox.Load();
            ResizableDashSwitch.Load();
            //ChangeMaxCassetteTrigger.Load();
            RedirectableBooster.Load();
            SeekerCrushBarrier.Load();
            SeekerCrushBarrierRenderer.Load();
            SeekerCrushZone.Load();
            DashBoostField.Load();
            BarrierDashSwitch.Load();
            //CrystalBombBadelineBoss.Load();

            //typeof(CavernHelperImports).ModInterop();
        }

        public override void Unload()
        {
            StarjumpTilesetHelper.Unload();
            ReverseKillbox.Unload();
            ResizableDashSwitch.Unload();
            //ChangeMaxCassetteTrigger.Unload();
            RedirectableBooster.Unload();
            SeekerCrushBarrier.Unload();
            SeekerCrushBarrierRenderer.Unload();
            SeekerCrushZone.Unload();
            DashBoostField.Unload();
            BarrierDashSwitch.Unload();
            //CrystalBombBadelineBoss.Unload();
        }

        public override void LoadContent(bool firstLoad)
        {
            base.LoadContent(firstLoad);

            _CustomEntitySpriteBank = new SpriteBank(GFX.Game, "Graphics/SSMHelper/CustomEntitySprites.xml");
            RedirectableBooster.LoadParticles();
            ResizableDashSwitch.LoadParticles();
            DashBoostField.LoadParticles();
        }
    }
}
