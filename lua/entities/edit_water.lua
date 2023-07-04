AddCSLuaFile()
DEFINE_BASECLASS("base_edit")

ENT.Spawnable = true
ENT.AdminOnly = true

ENT.PrintName = "Water Editor"
ENT.Category = "Editors"

function ENT:Initialize()

	BaseClass.Initialize(self)

	self:SetMaterial("gmod/edit_water")

end

function ENT:SetupDataTables()
    -- for debugging: uncomment to force refresh of allWaterMaterials
    -- allWaterMaterials = nil
    
    -- populate allWaterMaterials with every water material in the map and their properties
    -- TODO: reconcile this with the commands so this does actually only ever run once
    -- also figure out whether this stuff actually needs to run in the server realm
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

    local function GetComboOptions()
        local tbl = {
            ["All"] = "All",
            ["Above water"] = "Above water",
            ["Below water"] = "Below water"
        }
        for materialName, _ in pairs(allWaterMaterials) do
            tbl[materialName] = materialName
        end
        return tbl
    end

    self:NetworkVar("Bool", 0, "EditWaterFogStart", {KeyName = "editwaterfogstart", Edit = {type = "Boolean", title = "Modify Water Fog Start?", order = 1}})
    self:NetworkVar("Float", 0, "WaterFogStart", {KeyName = "waterfogstart", Edit = {type = "Float", title = "Water Fog Start", min = -10000, max = 10000, order = 2}})

    self:NetworkVar("Bool", 1, "EditWaterFogEnd", {KeyName = "editwaterfogEnd", Edit = {type = "Boolean", title = "Modify Water Fog End?", order = 3}})
    self:NetworkVar("Float", 1, "WaterFogEnd", {KeyName = "waterfogend", Edit = {type = "Float", title = "Water Fog End", min = 0, max = 10000, order = 4}})

    self:NetworkVar("Bool", 2, "EditWaterFogColor", {KeyName = "editwaterfogcolor", Edit = {type = "Boolean", title = "Modify Water Fog Color?", order = 5}})
    self:NetworkVar("Vector", 0, "WaterFogColor", {KeyName = "waterfogcolor", Edit = {type = "VectorColor", title = "Water Fog Color", order = 6}})

    self:NetworkVar("String", 0, "WaterMaterial", 
        {KeyName = "watermaterial", Edit = {type = "Combo", title = "Water Material(s) to Modify", text = "All", order = 7, values = GetComboOptions()}})

    -- defaults
    if SERVER then
        self:SetEditWaterFogStart(true)
        self:SetWaterFogStart(0)

        self:SetEditWaterFogEnd(true)
        self:SetWaterFogEnd(3000)

        self:SetEditWaterFogColor(true)
        self:SetWaterFogColor(Vector(0.027, 0.227, 0.259))

        self:SetWaterMaterial("All")
    end

end

function ENT:GetWaterMaterialTable()
    local waterMaterialOverride = self:GetWaterMaterial()
    if waterMaterialOverride == "All" then
        return allWaterMaterials
    elseif waterMaterialOverride == "Above water" or waterMaterialOverride == "Below water" then
        local aboveWater = (waterMaterialOverride == "Above water")
        local result = {}
        for materialName, tbl in pairs(allWaterMaterials) do
            if tbl.aboveWater == aboveWater then
                result[materialName] = tbl
            end
        end
        return result
    else
        return { [waterMaterialOverride] = allWaterMaterials[waterMaterialOverride] }
    end
end

function ENT:Think()
    local currWaterMaterialTable = self:GetWaterMaterialTable()

    -- reset any water materials that were just disabled
    for materialName, tbl in pairs(self.lastWaterMaterialTable or {}) do
        if not currWaterMaterialTable[materialName] then
            local material = Material(materialName)
            material:SetVector("$fogcolor", tbl.orig.fogColor)
            material:SetFloat("$fogstart", tbl.orig.fogStart)
            material:SetFloat("$fogend", tbl.orig.fogEnd)
        end
    end

    for materialName, tbl in pairs(currWaterMaterialTable) do
        -- TODO: figure out what to do about other instances of the entity modifying it when this one didn't?
        -- hm
        
        local fogStart = self:GetEditWaterFogStart() and self:GetWaterFogStart() or tbl.orig.fogStart
        local fogEnd = self:GetEditWaterFogEnd() and self:GetWaterFogEnd() or tbl.orig.fogEnd
        local fogColor = self:GetEditWaterFogColor() and self:GetWaterFogColor() or tbl.orig.fogColor

        local material = Material(materialName)
        material:SetVector("$fogcolor", fogColor)
        material:SetFloat("$fogstart", fogStart)
        material:SetFloat("$fogend", fogEnd)
    end

    self.lastWaterMaterialTable = currWaterMaterialTable
end

function ENT:OnRemove()
    for materialName, tbl in pairs(self:GetWaterMaterialTable()) do
        local material = Material(materialName)
        material:SetVector("$fogcolor", tbl.orig.fogColor)
        material:SetFloat("$fogstart", tbl.orig.fogStart)
        material:SetFloat("$fogend", tbl.orig.fogEnd)
    end
end

--
-- This edits something global - so always network - even when not in PVS
--
function ENT:UpdateTransmitState()

	return TRANSMIT_ALWAYS

end