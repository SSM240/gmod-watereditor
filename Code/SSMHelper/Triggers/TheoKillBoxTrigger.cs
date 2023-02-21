using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.SSMHelper.Triggers
{
    [CustomEntity("SSMHelper/TheoKillBoxTrigger")]
    public class TheoKillBoxTrigger : Trigger
    {
        public TheoKillBoxTrigger(EntityData data, Vector2 offset) : base(data, offset) { }

        public override void Update()
        {
            if (!SaveData.Instance.Assists.Invincible)
            {
                foreach (TheoCrystal crystal in CollideAll<TheoCrystal>())
                {
                    crystal.Die();
                }
            }
        }
    }
}
