using System;
using Monocle;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System.Reflection;
using MonoMod.RuntimeDetour;
using MonoMod.Cil;

namespace Celeste.Mod.SSMHelper.Entities
{
    /// <summary>
    /// Acts like a killbox but for the top of the screen instead of the bottom.
    /// </summary>
    [CustomEntity("SSMHelper/ReverseKillbox")]
    [Tracked]
    public class ReverseKillbox : Entity
    {
        public ReverseKillbox(EntityData data, Vector2 offset)
            : base(data.Position + offset)
        {
            Collider = new Hitbox(data.Width, 32f);
            Collidable = false;
            Add(new PlayerCollider(OnPlayer));
        }

        private void OnPlayer(Player player)
        {
            if (SaveData.Instance.Assists.Invincible)
            {
                player.Play("event:/game/general/assist_screenbottom");
                player.BounceDown();
            }
            else
            {
                player.Die(Vector2.Zero);
            }
        }

        public override void Update()
        {
            Player player = Scene.Tracker.GetEntity<Player>();
            if (player == null)
            {
                return;
            }
            if (!Collidable)
            {
                if (player.Top > (this.Bottom + 32f))
                {
                    Collidable = true;
                }
            }
            else
            {
                if (player.Bottom < (this.Top - 32f))
                {
                    Collidable = false;
                }
            }

            base.Update();
        }

        private static ILHook playerUpdateHook;

        private static MethodInfo playerUpdateInfo =
            typeof(Player).GetMethod("orig_Update",
                BindingFlags.Instance | BindingFlags.Public);

        public static void Load()
        {
            playerUpdateHook = new ILHook(playerUpdateInfo, IL_ChangeCameraTargetCalls);
            IL.Celeste.Level.TeleportTo += IL_ChangeCameraTargetCalls;
        }

        public static void Unload()
        {
            playerUpdateHook?.Dispose();
            playerUpdateHook = null;
            IL.Celeste.Level.TeleportTo -= IL_ChangeCameraTargetCalls;
        }

        // It would have been nice to hook Player.CameraTarget directly,
        // but doing that just completely explodes the game for dumb reasons.
        // So I'm hooking the relevant call sites instead.
        private static void IL_ChangeCameraTargetCalls(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallOrCallvirt<Player>("get_CameraTarget")))
            {
                cursor.EmitDelegate<Func<Vector2, Vector2>>(ModifyCameraTarget);
            }
        }

        private static Vector2 ModifyCameraTarget(Vector2 cameraTarget)
        {
            if (!(Engine.Scene is Level level)) return cameraTarget;
            Player player = level.Tracker.GetEntity<Player>();
            foreach (Entity reverseKillbox in level.Tracker.GetEntities<ReverseKillbox>())
            {
                if (reverseKillbox.Collidable
                && player.Bottom > reverseKillbox.Top
                && player.Right > reverseKillbox.Left
                && player.Left < reverseKillbox.Right)
                {
                    cameraTarget.Y = Math.Max(cameraTarget.Y, reverseKillbox.Bottom + 8f);
                }
            }
            return cameraTarget;
        }
    }
}
