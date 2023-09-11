//using Celeste.Mod.Entities;
//using Microsoft.Xna.Framework;
//using Mono.Cecil.Cil;
//using Monocle;
//using MonoMod.Cil;
//using MonoMod.RuntimeDetour;
//using MonoMod.Utils;
//using System;
//using System.Collections;
//using System.Reflection;

//namespace Celeste.Mod.SSMHelper.Entities
//{
//    [CustomEntity("SSMHelper/CrystalBombBadelineBoss")]
//    [TrackedAs(typeof(FinalBoss))]
//    public class CrystalBombBadelineBoss : FinalBoss
//    {
//        private string music;

//        // be more lenient with death hitbox
//        private const float playerCollideRadius = 8f;

//        public CrystalBombBadelineBoss(EntityData data, Vector2 offset) : base(data, offset)
//        {
//            // Replace player collider
//            Remove(Get<PlayerCollider>());
//            Add(new PlayerCollider(OnPlayerCBBB, new Circle(playerCollideRadius, 0f, -6f)));

//            music = data.Attr("music", "");
//            if (data.Bool("disableCameraLock"))
//            {
//                Remove(Get<CameraLocker>());
//            }

//            Component explosionCollider = CavernHelperImports.GetCrystalBombExplosionCollider?.Invoke(OnHit, null);
//            if (explosionCollider != null)
//            {
//                Add(explosionCollider);
//            }

//            Component exploderCollider = CavernHelperImports.GetCrystalBombExploderCollider?.Invoke(null);
//            if (exploderCollider != null)
//            {
//                Add(exploderCollider);
//            }
//        }

//        public override void Update()
//        {
//            base.Update();
//            // don't try to visually avoid the player
//            avoidPos = Vector2.Zero;
//        }

//        private void OnPlayerCBBB(Player player)
//        {
//            player.Die((player.Center - Center).SafeNormalize());
//        }

//        private void OnHit()
//        {
//            OnPlayer(null);
//        }

//        private void OnHit(Vector2 vector = default) // to satisfy cavern helper API
//        {
//            OnHit();
//        }

//        public static void Load()
//        {
//            On.Celeste.Seeker.RegenerateCoroutine += On_Seeker_RegenerateCoroutine;
//            On.Celeste.Puffer.Explode += On_Puffer_Explode;
//            IL.Celeste.FinalBoss.OnPlayer += IL_FinalBoss_OnPlayer;
//            IL.Celeste.FinalBoss.CreateBossSprite += IL_FinalBoss_CreateBossSprite;
//        }

//        public static void Unload()
//        {
//            On.Celeste.Seeker.RegenerateCoroutine -= On_Seeker_RegenerateCoroutine;
//            On.Celeste.Puffer.Explode -= On_Puffer_Explode;
//            IL.Celeste.FinalBoss.OnPlayer -= IL_FinalBoss_OnPlayer;
//            IL.Celeste.FinalBoss.CreateBossSprite -= IL_FinalBoss_CreateBossSprite;
//        }

//        private static IEnumerator On_Seeker_RegenerateCoroutine(On.Celeste.Seeker.orig_RegenerateCoroutine orig, Seeker self)
//        {
//            IEnumerator origEnum = orig(self);
//            while (origEnum.MoveNext())
//            {
//                yield return origEnum.Current;
//            }
//            Collider origCollider = self.Collider;
//            self.Collider = self.pushRadius;
//            foreach (FinalBoss boss in self.CollideAll<FinalBoss>())
//            {
//                if (boss is CrystalBombBadelineBoss cbbb)
//                    cbbb.OnHit();
//            }
//            self.Collider = origCollider;
//        }

//        private static void On_Puffer_Explode(On.Celeste.Puffer.orig_Explode orig, Puffer self)
//        {
//            orig(self);
//            Collider origCollider = self.Collider;
//            self.Collider = self.pushRadius;
//            foreach (FinalBoss boss in self.CollideAll<FinalBoss>())
//            {
//                if (boss is CrystalBombBadelineBoss cbbb)
//                    cbbb.OnHit();
//            }
//            self.Collider = origCollider;
//        }

//        private static void IL_FinalBoss_OnPlayer(ILContext il)
//        {
//            ILCursor cursor = new ILCursor(il);
//            while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdstr("event:/music/lvl6/badeline_fight")))
//            {
//                cursor.Emit(OpCodes.Ldarg_0);
//                cursor.EmitDelegate(ChangeMusic);
//            }
//        }

//        private static string ChangeMusic(string origMusic, FinalBoss self)
//        {
//            if (self is CrystalBombBadelineBoss crystalBoss && !string.IsNullOrEmpty(crystalBoss.music))
//            {
//                return crystalBoss.music;
//            }
//            else
//            {
//                return origMusic;
//            }
//        }

//        private static void IL_FinalBoss_CreateBossSprite(ILContext il)
//        {
//            ILCursor cursor = new ILCursor(il);
//            while (cursor.TryGotoNext(MoveType.Before, instr => instr.MatchDup()))
//            {
//                cursor.Emit(OpCodes.Ldarg_0);
//                cursor.EmitDelegate(ChangeSprite);
//                break;
//            }
//        }

//        private static Sprite ChangeSprite(Sprite origSprite, FinalBoss self)
//        {
//            if (self is CrystalBombBadelineBoss)
//            {
//                return SSMHelperModule.SpriteBank.Create("crystalBombBadelineBoss");
//            }
//            else
//            {
//                return origSprite;
//            }
//        }

//    }
//}
