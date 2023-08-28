WaterEdit = WaterEdit or {}
local WE = WaterEdit

local function GetMaterials()
    WaterEdit_Initialize()
    local cvarMaterialName = GetConVar("wateredit_material_override"):GetString()
    local cvarMaterial = Material(cvarMaterialName)
    -- return WE.allWaterMaterials if the cvar is not a valid texture
    if Material(cvarMaterialName):IsError() then return WE.allWaterMaterials end
    -- otherwise, return table with only one entry
    return { [cvarMaterialName] = WE.allWaterMaterials[cvarMaterialName] }
end

local function Cmd_FogColor(ply, cmd, args, str)
    WaterEdit_Initialize()
    if #args < 3 then print("[Error] Must include 3 numbers (R, G, and B in range 0-255)") return end
    -- can't use str:ToColor() because it requires an alpha
    r, g, b = tonumber(args[1]), tonumber(args[2]), tonumber(args[3])
    if not r or not g or not b then print("[Error] Invalid color parameter") return end
    local color = Color(r, g, b)
    for materialName, tbl in pairs(GetMaterials()) do
        Material(materialName):SetVector("$fogcolor", color:ToVector())
    end
end

local function Cmd_FogStart(ply, cmd, args, str)
    WaterEdit_Initialize()

    if #args < 1 then print("[Error] No number entered") return end
    local start = tonumber(args[1])
    if not start then print("[Error] Invalid number entered") return end

    for materialName, tbl in pairs(GetMaterials()) do
        Material(materialName):SetFloat("$fogstart", start)
    end
end

local function Cmd_FogEnd(ply, cmd, args, str)
    WaterEdit_Initialize()

    if #args < 1 then print("[Error] No number entered") return end
    local _end = tonumber(args[1]) -- "end" is a reserved word lol
    if not _end then print("[Error] Invalid number entered") return end

    for materialName, tbl in pairs(GetMaterials()) do
        Material(materialName):SetFloat("$fogend", _end)
    end
end

local function Cmd_ResetFogColor(ply, cmd, args, str)
    WaterEdit_Initialize()

    for materialName, tbl in pairs(GetMaterials()) do
        Material(materialName):SetVector("$fogcolor", tbl.orig.fogColor)
    end
end

local function Cmd_ResetFogStart(ply, cmd, args, str)
    WaterEdit_Initialize()

    for materialName, tbl in pairs(GetMaterials()) do
        Material(materialName):SetFloat("$fogstart", tbl.orig.fogStart)
    end
end

local function Cmd_ResetFogEnd(ply, cmd, args, str)
    WaterEdit_Initialize()

    for materialName, tbl in pairs(GetMaterials()) do
        Material(materialName):SetFloat("$fogend", tbl.orig.fogEnd)
    end
end

local function Cmd_ResetFogAll(ply, cmd, args, str)
    Cmd_ResetFogColor()
    Cmd_ResetFogStart()
    Cmd_ResetFogEnd()
end

local function Cmd_Reinitialize(ply, cmd, args, str)
    WE.allWaterMaterials = nil
    WaterEdit_Initialize()
end

local function Cmd_ListMaterials(ply, cmd, args, str)
    -- all this to ignore the alpha channel -_-
    -- yes i could just trim off " 255" from the end but i don't feel confident that it'll always work
    local function ColorVectorToString(vector)
        local color = vector:ToColor()
        if not color.r then return "nil" end
        return math.floor(color.r).." "..math.floor(color.g).." "..math.floor(color.b)
    end
    WaterEdit_Initialize()
    for materialName, tbl in pairs(WE.allWaterMaterials) do
        print(materialName.. "\n  defaults:  color "..ColorVectorToString(tbl.orig.fogColor)
            .." ("..tbl.orig.fogColor.."),  start "..tbl.orig.fogStart..",  end "..tbl.orig.fogEnd.."\n")
    end
end

local function Cmd_DisableDirt(ply, cmd, args, str)
    WaterEdit_Initialize()
    WE.disableDirt = not WE.disableDirt
    if WE.disableDirt then
        Material("effects/fleck_cement1"):SetTexture("$basetexture", "gmod/full_transparent")
        Material("effects/fleck_cement2"):SetTexture("$basetexture", "gmod/full_transparent")
        print("Disabled dirt particles")
    else
        Material("effects/fleck_cement1"):SetTexture("$basetexture", "effects/fleck_cement1")
        Material("effects/fleck_cement2"):SetTexture("$basetexture", "effects/fleck_cement2")
        print("Enabled dirt particles")
    end
end

local function Cmd_DisableBlur(ply, cmd, args, str)
    WaterEdit_Initialize()
    WE.disableBlur = not WE.disableBlur
    local material = Material("effects/water_warp01")
    if WE.disableBlur then
        material:SetTexture("$normalmap", "dev/flat_normal")
        material:SetInt("$bluramount", 0)
        print("Disabled blur")
    else
        material:SetTexture("$normalmap", WE.blurMaterialProperties.orig.normalMap)
        material:SetInt("$bluramount", WE.blurMaterialProperties.orig.blurAmount)
        print("Enabled blur")
    end
end

local commands = {
    ["wateredit_fog_color"] = {
        func = Cmd_FogColor, 
        help = "Sets water fog color (3 numbers for R, G, and B, in range 0-255)"
    },
    ["wateredit_fog_start"] = {
        func = Cmd_FogStart, 
        help = "Sets water fog start position (any number)"
    },
    ["wateredit_fog_end"] = {
        func = Cmd_FogEnd, 
        help = "Sets water fog end position (any number)"
    },
    ["wateredit_reset_fog_color"] = {
        func = Cmd_ResetFogColor, 
        help = "Resets water fog color to default"
    },
    ["wateredit_reset_fog_start"] = {
        func = Cmd_ResetFogStart, 
        help = "Resets water fog start position to default"
    },
    ["wateredit_reset_fog_end"] = {
        func = Cmd_ResetFogEnd, 
        help = "Resets water fog end position to default"
    },
    ["wateredit_reset_fog"] = {
        func = Cmd_ResetFogAll, 
        help = "Resets all water fog parameters to default"
    },
    ["wateredit_reinitialize"] = {
        func = Cmd_Reinitialize,
        help = "Re-scans map for water textures"
    },
    ["wateredit_list_materials"] = {
        func = Cmd_ListMaterials, 
        help = "Lists all known water materials in the current map and their default parameters"
    },
    ["wateredit_toggledirt"] = {
        func = Cmd_DisableDirt,
        help = "Disables the floating dirt particles underwater"
    },
    ["wateredit_toggleblur"] = {
        func = Cmd_DisableBlur,
        help = "Reduces underwater blur as much as possible"
    }
}

for name, info in pairs(commands) do
    concommand.Add(name, info.func, nil, info.help)
end

CreateClientConVar(
    "wateredit_material_override", "nil", false, false, 
    "If set to a valid material, wateredit commands will only change that material\n   (use wateredit_list_materials to see all known water materials in current map)")
