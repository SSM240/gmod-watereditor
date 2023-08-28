AddCSLuaFile()
DEFINE_BASECLASS("base_edit")

ENT.Spawnable = true
ENT.AdminOnly = true

ENT.PrintName = "#watereditor.name"
ENT.Category = "Editors"

function ENT:Initialize()
	BaseClass.Initialize(self)

	self:SetMaterial("gmod/edit_water")
    
    -- for debugging: uncomment to force refresh of allWaterMaterials
    -- allWaterMaterials = nil
    
    -- populates allWaterMaterials with every water material in the map and their properties
    WaterEdit_Initialize()
end

function ENT:SetupDataTables()

    local function GetComboOptions()
        local tbl = {
            ["#watereditor.watermaterialoptions.all"] = "All",
            ["#watereditor.watermaterialoptions.abovewater"] = "Above water",
            ["#watereditor.watermaterialoptions.belowwater"] = "Below water"
        }
        for materialName, _ in pairs(allWaterMaterials) do
            tbl[materialName] = materialName
        end
        return tbl
    end

    self:NetworkVar("Bool", 0, "EditWaterFogStart", {KeyName = "editwaterfogstart", Edit = {type = "Boolean", title = "#watereditor.editwaterfogstart", order = 1}})
    self:NetworkVar("Float", 0, "WaterFogStart", {KeyName = "waterfogstart", Edit = {type = "Float", title = "#watereditor.waterfogstart", min = -10000, max = 10000, order = 2}})
    
    self:NetworkVar("Bool", 1, "EditWaterFogEnd", {KeyName = "editwaterfogEnd", Edit = {type = "Boolean", title = "#watereditor.editwaterfogend", order = 3}})
    self:NetworkVar("Float", 1, "WaterFogEnd", {KeyName = "waterfogend", Edit = {type = "Float", title = "#watereditor.waterfogend", min = 0, max = 10000, order = 4}})

    self:NetworkVar("Bool", 2, "EditWaterFogColor", {KeyName = "editwaterfogcolor", Edit = {type = "Boolean", title = "#watereditor.editwaterfogcolor", order = 5}})
    self:NetworkVar("Vector", 0, "WaterFogColor", {KeyName = "waterfogcolor", Edit = {type = "VectorColor", title = "#watereditor.waterfogcolor", order = 6}})

    self:NetworkVar("String", 0, "WaterMaterial", 
        {KeyName = "watermaterial", Edit = {type = "Combo", title = "#watereditor.watermaterial", text = "All", order = 7, values = GetComboOptions()}})
    
    if CLIENT then
        self:NetworkVarNotify("EditWaterFogStart", self.OnChangeEditWaterFogStart)
        self:NetworkVarNotify("WaterFogStart", self.OnChangeWaterFogStart)
        self:NetworkVarNotify("EditWaterFogEnd", self.OnChangeEditWaterFogEnd)
        self:NetworkVarNotify("WaterFogEnd", self.OnChangeWaterFogEnd)
        self:NetworkVarNotify("EditWaterFogColor", self.OnChangeEditWaterFogColor)
        self:NetworkVarNotify("WaterFogColor", self.OnChangeWaterFogColor)
        self:NetworkVarNotify("WaterMaterial", self.OnChangeWaterMaterial)
    end

    -- defaults
    if SERVER then
        self:SetEditWaterFogStart(true)
        self:SetWaterFogStart(0)

        self:SetEditWaterFogEnd(true)
        self:SetWaterFogEnd(2000)

        self:SetEditWaterFogColor(false)
        -- TODO: figure out how to make this default to one of the normal material colors?
        self:SetWaterFogColor(Vector(0.027, 0.227, 0.259))

        self:SetWaterMaterial("All")
    end

    if CLIENT then
        self:ForceApplyProperties()
    end

end

function ENT:GetWaterMaterialTable(waterMaterialOverride)
    waterMaterialOverride = waterMaterialOverride or self:GetWaterMaterial()
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

-- note: these callbacks generally assume all properties change one at a time
-- which SHOULD be the case if you're using the normal menu

function ENT:OnChangeEditWaterFogStart(_, oldValue, newValue)
    for materialName, tbl in pairs(self:GetWaterMaterialTable()) do
        local material = Material(materialName)
        local fogStart = newValue and self:GetWaterFogStart() or tbl.orig.fogStart
        material:SetFloat("$fogstart", fogStart)
    end
end
function ENT:OnChangeWaterFogStart(_, oldValue, newValue)
    if not self:GetEditWaterFogStart() then return end
    for materialName, tbl in pairs(self:GetWaterMaterialTable()) do
        local material = Material(materialName)
        material:SetFloat("$fogstart", newValue)
    end
end

function ENT:OnChangeEditWaterFogEnd(_, oldValue, newValue)
    for materialName, tbl in pairs(self:GetWaterMaterialTable()) do
        local material = Material(materialName)
        local fogEnd = newValue and self:GetWaterFogEnd() or tbl.orig.fogEnd
        material:SetFloat("$fogend", fogEnd)
    end
end
function ENT:OnChangeWaterFogEnd(_, oldValue, newValue)
    if not self:GetEditWaterFogEnd() then return end
    for materialName, tbl in pairs(self:GetWaterMaterialTable()) do
        local material = Material(materialName)
        material:SetFloat("$fogend", newValue)
    end
end

function ENT:OnChangeEditWaterFogColor(_, oldValue, newValue)
    for materialName, tbl in pairs(self:GetWaterMaterialTable()) do
        local material = Material(materialName)
        local fogColor = newValue and self:GetWaterFogColor() or tbl.orig.fogColor
        material:SetVector("$fogcolor", fogColor)
    end
end
function ENT:OnChangeWaterFogColor(_, oldValue, newValue)
    if not self:GetEditWaterFogColor() then return end
    for materialName, tbl in pairs(self:GetWaterMaterialTable()) do
        local material = Material(materialName)
        material:SetVector("$fogcolor", newValue)
    end
end

function ENT:OnChangeWaterMaterial(_, oldValue, newValue)
    local oldWaterMaterialTable = self:GetWaterMaterialTable(oldValue)
    local currWaterMaterialTable = self:GetWaterMaterialTable(newValue)
    -- reset all materials that were just removed
    for materialName, tbl in pairs(oldWaterMaterialTable) do
        if not currWaterMaterialTable[materialName] then
            local material = Material(materialName)
            if self:GetEditWaterFogColor() then
                material:SetVector("$fogcolor", tbl.orig.fogColor)
            end
            if self:GetEditWaterFogStart() then
                material:SetFloat("$fogstart", tbl.orig.fogStart)
            end
            if self:GetEditWaterFogEnd() then
                material:SetFloat("$fogend", tbl.orig.fogEnd)
            end
        end
    end
    -- change all materials that were just added
    for materialName, tbl in pairs(currWaterMaterialTable) do
        if not oldWaterMaterialTable[materialName] then
            local material = Material(materialName)
            if self:GetEditWaterFogColor() then
                material:SetVector("$fogcolor", self:GetWaterFogColor())
            end
            if self:GetEditWaterFogStart() then
                material:SetFloat("$fogstart", self:GetWaterFogStart())
            end
            if self:GetEditWaterFogEnd() then
                material:SetFloat("$fogend", self:GetWaterFogEnd())
            end
        end
    end
end

function ENT:ForceApplyProperties()
    for materialName, tbl in pairs(self:GetWaterMaterialTable()) do
        local material = Material(materialName)
        if self:GetEditWaterFogColor() then
            material:SetVector("$fogcolor", self:GetWaterFogColor())
        end
        if self:GetEditWaterFogStart() then
            material:SetFloat("$fogstart", self:GetWaterFogStart())
        end
        if self:GetEditWaterFogEnd() then
            material:SetFloat("$fogend", self:GetWaterFogEnd())
        end
    end
end

function ENT:OnRemove()
    for materialName, tbl in pairs(self:GetWaterMaterialTable()) do
        local material = Material(materialName)
        if self:GetEditWaterFogColor() then
            material:SetVector("$fogcolor", tbl.orig.fogColor)
        end
        if self:GetEditWaterFogStart() then
            material:SetFloat("$fogstart", tbl.orig.fogStart)
        end
        if self:GetEditWaterFogEnd() then
            material:SetFloat("$fogend", tbl.orig.fogEnd)
        end
    end
end

-- This edits something global - so always network - even when not in PVS
function ENT:UpdateTransmitState()
	return TRANSMIT_ALWAYS
end
