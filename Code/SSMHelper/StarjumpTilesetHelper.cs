using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Celeste.Mod.SSMHelper
{
    public static class StarjumpTilesetHelper
    {
        // texture name and alpha
        private static readonly Dictionary<string, float> starJumpTextures = new Dictionary<string, float>();

        public static void Load()
        {
            On.Celeste.Autotiler.ReadInto += On_Autotiler_ReadInto;
            Everest.Events.Level.OnExit += On_Level_OnExit;
            IL.Monocle.TileGrid.RenderAt += IL_TileGrid_RenderAt;
        }

        public static void Unload()
        {
            On.Celeste.Autotiler.ReadInto -= On_Autotiler_ReadInto;
            Everest.Events.Level.OnExit -= On_Level_OnExit;
            IL.Monocle.TileGrid.RenderAt -= IL_TileGrid_RenderAt;
        }

        private static void On_Level_OnExit(Level level, LevelExit exit, LevelExit.Mode mode, Session session, HiresSnow snow)
        {
            starJumpTextures.Clear();
        }

        private static void On_Autotiler_ReadInto(On.Celeste.Autotiler.orig_ReadInto orig, Autotiler self, object data, Tileset tileset, XmlElement xml)
        {
            orig(self, data, tileset, xml);
            if (xml.HasAttr("starjump"))
            {
                starJumpTextures["tilesets/" + xml.Attr("path")] = xml.AttrFloat("starjump", 1f);
            }
        }

        private static void IL_TileGrid_RenderAt(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            // this is a godawful way of doing this but it should work hopefully
            if (cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdloc(4), instr => instr.MatchLdloc(7)))
            {
                // texture
                cursor.Emit(OpCodes.Ldloc_S, (byte)7);
                // tileGrid
                cursor.Emit(OpCodes.Ldarg_0);
                // position
                cursor.Emit(OpCodes.Ldarg_1);
                // i and j because we need these too
                cursor.Emit(OpCodes.Ldloc_S, (byte)5);
                cursor.Emit(OpCodes.Ldloc_S, (byte)6);
                // color
                cursor.Emit(OpCodes.Ldloc_3);
                cursor.EmitDelegate(CheckTextureAndRender);
            }
        }

        private static void CheckTextureAndRender(MTexture texture, TileGrid tileGrid, Vector2 position, int i, int j, Color color)
        {
            string parentPath = texture.Parent.AtlasPath;
            if (starJumpTextures.TryGetValue(parentPath, out float alpha))
            {
                color *= alpha;
                RenderStarClimbEffect(position.X + i * tileGrid.TileWidth, position.Y + j * tileGrid.TileHeight, 8f, 8f, color);
            }
        }

        // largely copy-pasted from Patches/StarJumpBlock.Render()
        public static void RenderStarClimbEffect(float x, float y, float width, float height, Color color)
        {
            if (Engine.Scene is Level level)
            {
                StarJumpController vanillaController = level.Tracker.GetEntity<StarJumpController>();
                StarClimbGraphicsController everestController = level.Tracker.GetEntity<StarClimbGraphicsController>();

                Vector2 cameraPos = level.Camera.Position.Floor();
                VirtualRenderTarget blockFill = null;

                if (everestController != null)
                {
                    blockFill = StarClimbGraphicsController.BlockFill;
                }
                else if (vanillaController != null)
                {
                    blockFill = vanillaController.BlockFill;
                }

                if (blockFill != null)
                {
                    Draw.SpriteBatch.Draw(
                        blockFill,
                        new Vector2(x, y),
                        new Rectangle?(new Rectangle((int)(x - cameraPos.X), (int)(y - cameraPos.Y), (int)width, (int)height)),
                        color
                    );
                }
            }
        }
    }
}
