AddCSLuaFile()

WaterEdit = WaterEdit or {}
local WE = WaterEdit

DEFINE_BASECLASS("base_edit")

ENT.Spawnable = true
ENT.AdminOnly = true

ENT.PrintName = "#watereditor.name"
ENT.Category = "Editors"

function ENT:Initialize()
	BaseClass.Initialize(self)

	self:SetMaterial("gmod/edit_water")
end

function ENT:SetupDataTables()
    -- this feels like it should go in Initialize but that doesn't work as well for some reason
    WaterEdit_Initialize()

    local function GetComboOptions()
        local tbl = {
            ["#watereditor.watermaterialoptions.all"] = "All",
            ["#watereditor.watermaterialoptions.abovewater"] = "Above water",
            ["#watereditor.watermaterialoptions.belowwater"] = "Below water"
        }
        for materialName, _ in pairs(WE.allWaterMaterials) do
            tbl[materialName] = materialName
        end
        return tbl
    end

    self:NetworkVar("Bool", 0, "EditWaterFogStart", 
        {KeyName = "editwaterfogstart", Edit = {type = "Boolean", title = "#watereditor.editwaterfogstart", order = 1, category = "Fog"}})
    self:NetworkVar("Float", 0, "WaterFogStart", 
        {KeyName = "waterfogstart", Edit = {type = "Float", title = "#watereditor.waterfogstart", min = -10000, max = 10000, order = 2, category = "Fog"}})
    self:NetworkVar("Bool", 1, "EditWaterFogEnd", 
        {KeyName = "editwaterfogEnd", Edit = {type = "Boolean", title = "#watereditor.editwaterfogend", order = 3, category = "Fog"}})
    self:NetworkVar("Float", 1, "WaterFogEnd", 
        {KeyName = "waterfogend", Edit = {type = "Float", title = "#watereditor.waterfogend", min = 0, max = 10000, order = 4, category = "Fog"}})
    self:NetworkVar("Bool", 2, "EditWaterFogColor", 
        {KeyName = "editwaterfogcolor", Edit = {type = "Boolean", title = "#watereditor.editwaterfogcolor", order = 5, category = "Fog"}})
    self:NetworkVar("Vector", 0, "WaterFogColor", 
        {KeyName = "waterfogcolor", Edit = {type = "VectorColor", title = "#watereditor.waterfogcolor", order = 6, category = "Fog"}})
    self:NetworkVar("String", 0, "WaterMaterial", 
        {KeyName = "watermaterial", Edit = {type = "Combo", title = "#watereditor.watermaterial", text = "All", order = 7, category = "Fog", values = GetComboOptions()}})
    
    self:NetworkVar("Bool", 3, "DisableDirt",
        {KeyName = "disabledirt", Edit = {type = "Boolean", title = "#watereditor.disabledirt", order = 8, category = "Global"}})
    self:NetworkVar("Bool", 4, "DisableBlur",
        {KeyName = "disableblur", Edit = {type = "Boolean", title = "#watereditor.disableblur", order = 98, category = "Global"}})
    
    if CLIENT then
        self:NetworkVarNotify("EditWaterFogStart", self.OnChangeEditWaterFogStart)
        self:NetworkVarNotify("WaterFogStart", self.OnChangeWaterFogStart)
        self:NetworkVarNotify("EditWaterFogEnd", self.OnChangeEditWaterFogEnd)
        self:NetworkVarNotify("WaterFogEnd", self.OnChangeWaterFogEnd)
        self:NetworkVarNotify("EditWaterFogColor", self.OnChangeEditWaterFogColor)
        self:NetworkVarNotify("WaterFogColor", self.OnChangeWaterFogColor)
        self:NetworkVarNotify("WaterMaterial", self.OnChangeWaterMaterial)
        self:NetworkVarNotify("DisableDirt", self.OnChangeDisableDirt)
        self:NetworkVarNotify("DisableBlur", self.OnChangeDisableBlur)
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

        self:SetDisableDirt(WE.disableDirt)
        self:SetDisableBlur(WE.disableBlur)
    end

    if CLIENT then
        self:ForceApplyProperties()
    end

end

function ENT:GetWaterMaterialTable(waterMaterialOverride)
    waterMaterialOverride = waterMaterialOverride or self:GetWaterMaterial()
    if waterMaterialOverride == "All" then
        return WE.allWaterMaterials
    elseif waterMaterialOverride == "Above water" or waterMaterialOverride == "Below water" then
        local aboveWater = (waterMaterialOverride == "Above water")
        local result = {}
        for materialName, tbl in pairs(WE.allWaterMaterials) do
            if tbl.aboveWater == aboveWater then
                result[materialName] = tbl
            end
        end
        return result
    else
        return { [waterMaterialOverride] = WE.allWaterMaterials[waterMaterialOverride] }
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

function ENT:OnChangeDisableDirt(_, oldValue, newValue)
    WE.disableDirt = newValue
    if newValue then
        Material("effects/fleck_cement1"):SetTexture("$basetexture", "gmod/full_transparent")
        Material("effects/fleck_cement2"):SetTexture("$basetexture", "gmod/full_transparent")
    else
        Material("effects/fleck_cement1"):SetTexture("$basetexture", "effects/fleck_cement1")
        Material("effects/fleck_cement2"):SetTexture("$basetexture", "effects/fleck_cement2")
    end
end

function ENT:OnChangeDisableBlur(_, oldValue, newValue)
    WE.disableBlur = newValue
    local material = Material("effects/water_warp01")
    if newValue then
        material:SetTexture("$normalmap", "dev/flat_normal")
        material:SetInt("$bluramount", 0)
    else
        material:SetTexture("$normalmap", WE.blurMaterialProperties.orig.normalMap)
        material:SetInt("$bluramount", WE.blurMaterialProperties.orig.blurAmount)
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
