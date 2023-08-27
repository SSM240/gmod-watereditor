function WaterFog_Initialize()
    -- populate allWaterMaterials with every water material in the map and their properties
    if not allWaterMaterials then
        allWaterMaterials = {}  -- intentionally global
        for _, ent in ipairs(ents:GetAll()) do
            for _, brush in ipairs(ent:GetBrushSurfaces() or {}) do
                if brush:IsWater() then
                    local material = brush:GetMaterial()
                    local materialName = material:GetName()
                    if not allWaterMaterials[materialName] then
                        allWaterMaterials[materialName] = {
                            aboveWater = tobool(material:GetInt("$abovewater")),
                            orig = {
                                fogColor = material:GetVector("$fogcolor"),
                                fogStart = material:GetFloat("$fogstart"),
                                fogEnd = material:GetFloat("$fogend"),
                            }
                        }
                    end
                end
            end
        end
    end
end

local function GetMaterials()
    WaterFog_Initialize()
    local cvarMaterialName = GetConVar("waterfog_material_override"):GetString()
    local cvarMaterial = Material(cvarMaterialName)
    -- return allWaterMaterials if the cvar is not a valid texture
    if Material(cvarMaterialName):IsError() then return allWaterMaterials end
    -- otherwise, return table with only one entry
    return { [cvarMaterialName] = allWaterMaterials[cvarMaterialName] }
end

local function WaterFog_CmdColor(ply, cmd, args, str)
    WaterFog_Initialize()
    if #args < 3 then print("[Error] Must include 3 numbers (R, G, and B in range 0-255)") return end
    -- can't use str:ToColor() because it requires an alpha
    r, g, b = tonumber(args[1]), tonumber(args[2]), tonumber(args[3])
    if not r or not g or not b then print("[Error] Invalid color parameter") return end
    local color = Color(r, g, b)
    for materialName, tbl in pairs(GetMaterials()) do
        Material(materialName):SetVector("$fogcolor", color:ToVector())
    end
end

local function WaterFog_CmdStart(ply, cmd, args, str)
    WaterFog_Initialize()

    if #args < 1 then print("[Error] No number entered") return end
    local start = tonumber(args[1])
    if not start then print("[Error] Invalid number entered") return end

    for materialName, tbl in pairs(GetMaterials()) do
        Material(materialName):SetFloat("$fogstart", start)
    end
end

local function WaterFog_CmdEnd(ply, cmd, args, str)
    WaterFog_Initialize()

    if #args < 1 then print("[Error] No number entered") return end
    local _end = tonumber(args[1]) -- "end" is a reserved word lol
    if not _end then print("[Error] Invalid number entered") return end

    for materialName, tbl in pairs(GetMaterials()) do
        Material(materialName):SetFloat("$fogend", _end)
    end
end

local function WaterFog_CmdResetColor(ply, cmd, args, str)
    WaterFog_Initialize()

    for materialName, tbl in pairs(GetMaterials()) do
        Material(materialName):SetVector("$fogcolor", tbl.orig.fogColor)
    end
end

local function WaterFog_CmdResetStart(ply, cmd, args, str)
    WaterFog_Initialize()

    for materialName, tbl in pairs(GetMaterials()) do
        Material(materialName):SetFloat("$fogstart", tbl.orig.fogStart)
    end
end

local function WaterFog_CmdResetEnd(ply, cmd, args, str)
    WaterFog_Initialize()

    for materialName, tbl in pairs(GetMaterials()) do
        Material(materialName):SetFloat("$fogend", tbl.orig.fogEnd)
    end
end

local function WaterFog_CmdResetAll(ply, cmd, args, str)
    WaterFog_CmdResetColor()
    WaterFog_CmdResetStart()
    WaterFog_CmdResetEnd()
end

local function WaterFog_CmdReinitialize(ply, cmd, args, str)
    initialized = false
    WaterFog_Initialize()
end

local function WaterFog_ListMaterials(ply, cmd, args, str)
    -- all this to ignore the alpha channel -_-
    -- yes i could just trim off " 255" from the end but i don't feel confident that it'll always work
    local function ColorVectorToString(vector)
        local color = vector:ToColor()
        if not color.r then return "nil" end
        return math.floor(color.r).." "..math.floor(color.g).." "..math.floor(color.b)
    end
    WaterFog_Initialize()
    for materialName, tbl in pairs(allWaterMaterials) do
        print(materialName.. "\n  defaults:  color "..ColorVectorToString(tbl.orig.fogColor)
            .." ("..tbl.orig.fogColor.."),  start "..tbl.orig.fogStart..",  end "..tbl.orig.fogEnd.."\n")
    end
end

local commands = {
    ["waterfog_color"] = {
        func = WaterFog_CmdColor, 
        help = "Sets water fog color (3 numbers for R, G, and B, in range 0-255)"
    },
    ["waterfog_start"] = {
        func = WaterFog_CmdStart, 
        help = "Sets water fog start position (any number)"
    },
    ["waterfog_end"] = {
        func = WaterFog_CmdEnd, 
        help = "Sets water fog end position (any number)"
    },
    ["waterfog_reset_color"] = {
        func = WaterFog_CmdResetColor, 
        help = "Resets water fog color to default"
    },
    ["waterfog_reset_start"] = {
        func = WaterFog_CmdResetStart, 
        help = "Resets water fog start position to default"
    },
    ["waterfog_reset_end"] = {
        func = WaterFog_CmdResetEnd, 
        help = "Resets water fog end position to default"
    },
    ["waterfog_reset"] = {
        func = WaterFog_CmdResetAll, 
        help = "Resets all water fog parameters to default"
    },
    ["waterfog_reinitialize"] = {
        func = WaterFog_CmdReinitialize,
        help = "Re-scans map for water textures"
    },
    ["waterfog_list_materials"] = {
        func = WaterFog_ListMaterials, 
        help = "Lists all known water materials in the current map and their default parameters"
    },
}

for name, info in pairs(commands) do
    concommand.Add(name, info.func, nil, info.help)
end

-- TODO: make a concommand that just acts like a fake convar? lol
CreateClientConVar(
    "waterfog_material_override", "nil", false, false, 
    "If set to a valid material, waterfog commands will only change that material\n   (use waterfog_list_materials to see all known water materials in current map)")
