using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.ModInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.SSMHelper.Entities
{
    [ModImportName("CavernHelper")]
    public static class CavernHelperImports
    {
        public static Func<Action<Vector2>, Collider, Component> GetCrystalBombExplosionCollider;
        public static Func<Collider, Component> GetCrystalBombExploderCollider;
    }
}
