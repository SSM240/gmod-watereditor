// experiment that didn't pan out

/*
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;

namespace Celeste.Mod.SSMHelper.Triggers
{
    [CustomEntity("SSMHelper/ChangeMaxCassetteTrigger")]
    public class ChangeMaxCassetteTrigger : Trigger
    {
        private int maxBeat;

        private static int maxBeatToSet = 0;

        public ChangeMaxCassetteTrigger(EntityData data, Vector2 offset)
            : base(data, offset)
        {
            maxBeat = Calc.Clamp(data.Int("maxBeat"), 2, 4);
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            maxBeatToSet = this.maxBeat;
        }

        public static void Load()
        {
            On.Celeste.CassetteBlockManager.SetActiveIndex += On_CassetteBlockManager_SetActiveIndex;
        }

        public static void Unload()
        {
            On.Celeste.CassetteBlockManager.SetActiveIndex -= On_CassetteBlockManager_SetActiveIndex;
        }

        private static void On_CassetteBlockManager_SetActiveIndex(
            On.Celeste.CassetteBlockManager.orig_SetActiveIndex orig, CassetteBlockManager self, int index)
        {
            if (maxBeatToSet > 0)
            {
                DynamicData managerData = DynamicData.For(self);
                managerData.Set("maxBeat", maxBeatToSet);
                int currentIndex = managerData.Get<int>("currentIndex");
                if (currentIndex > maxBeatToSet - 1)
                {
                    index = 0;
                    managerData.Set("currentIndex", 0);
                }
                maxBeatToSet = 0;
            }
            orig(self, index);
        }
    }
}
*/
