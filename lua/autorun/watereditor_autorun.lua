WaterEdit = WaterEdit or {}
local WE = WaterEdit

function WaterEdit_Initialize()
    -- populate WE.allWaterMaterials with every water material in the map and their properties
    if not WE.allWaterMaterials then
        WE.allWaterMaterials = {}
        for _, ent in ipairs(ents:GetAll()) do
            for _, brush in ipairs(ent:GetBrushSurfaces() or {}) do
                if brush:IsWater() then
                    local material = brush:GetMaterial()
                    local materialName = material:GetName()
                    if not WE.allWaterMaterials[materialName] then
                        WE.allWaterMaterials[materialName] = {
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

    if WE.disableDirt == nil then
        WE.disableDirt = false
    end

    -- not totally confident these will always be the same, so playing it safe
    if not WE.blurMaterialProperties then
        local material = Material("effects/water_warp01")
        WE.blurMaterialProperties = {
            orig = {
                normalMap = material:GetTexture("$normalmap"),
                blurAmount = material:GetInt("$bluramount")
            }
        }
    end

    if WE.disableBlur == nil then
        WE.disableBlur = false
    end
end
