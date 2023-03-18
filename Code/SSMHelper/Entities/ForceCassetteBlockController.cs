using Celeste.Mod.Entities;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.SSMHelper.Entities
{
    [CustomEntity("SSMHelper/ForceCassetteBlockController")]
    public class ForceCassetteBlockController : Entity
    {
        public ForceCassetteBlockController()
        {
        }

        public override void Awake(Scene scene)
        {
            Level level = SceneAs<Level>();
            CassetteBlockManager cbm = level.Tracker.GetEntity<CassetteBlockManager>();
            if (cbm != null) 
            {
                cbm.OnLevelStart();
                return;
            }
            if (level.HasCassetteBlocks && !level.ShouldCreateCassetteManager)
            {
                cbm = new CassetteBlockManager();
                level.Add(cbm);
                level.OnEndOfFrame += () =>
                {
                    level.Entities.UpdateLists();
                    cbm.OnLevelStart();
                };
            }
        }
    }
}
