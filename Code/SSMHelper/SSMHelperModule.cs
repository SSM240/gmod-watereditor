using Microsoft.Xna.Framework;
using Monocle;
using System;
using Celeste.Mod.SSMHelper.Entities;
using Celeste.Mod.SSMHelper.Triggers;

namespace Celeste.Mod.SSMHelper
{
    public class SSMHelperModule : EverestModule
    {
        public static SSMHelperModule Instance;

        public SSMHelperModule()
        {
            Instance = this;
        }

        public override void Load()
        {
            StarjumpTilesetHelper.Load();
            ReverseKillbox.Load();
            //ChangeMaxCassetteTrigger.Load();
            RedirectableBooster.Load();
        }

        public override void Unload()
        {
            StarjumpTilesetHelper.Unload();
            ReverseKillbox.Unload();
            //ChangeMaxCassetteTrigger.Unload();
            RedirectableBooster.Unload();
        }
    }
}
