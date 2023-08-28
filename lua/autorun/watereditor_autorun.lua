function WaterEdit_Initialize()
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