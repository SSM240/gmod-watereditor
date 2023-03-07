﻿using Microsoft.Xna.Framework;
using Monocle;
using System;
using Celeste.Mod.SSMHelper.Entities;
using Celeste.Mod.SSMHelper.Triggers;

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
        }

        public override void Unload()
        {
            StarjumpTilesetHelper.Unload();
            ReverseKillbox.Unload();
            ResizableDashSwitch.Unload();
            //ChangeMaxCassetteTrigger.Unload();
        }

        public override void LoadContent(bool firstLoad)
        {
            base.LoadContent(firstLoad);

            _CustomEntitySpriteBank = new SpriteBank(GFX.Game, "Graphics/SSMHelper/CustomEntitySprites.xml");

            ResizableDashSwitch.LoadParticles();
        }
    }
}
