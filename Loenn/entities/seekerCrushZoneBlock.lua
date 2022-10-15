local fakeTilesHelper = require("helpers.fake_tiles")

local seekerCrushZoneBlock = {}

seekerCrushZoneBlock.name = "SSMHelper/SeekerCrushZoneBlock"
seekerCrushZoneBlock.nodeLimits = {1, 1}
seekerCrushZoneBlock.placements = {
    name = "normal",
    data = {
        width = 8,
        height = 8,
        tile1 = "g",
        tile2 = "G"
    }
}

seekerCrushZoneBlock.sprite = fakeTilesHelper.getEntitySpriteFunction("tile1", false)
fieldInformation = fakeTilesHelper.getFieldInformation("tile1")
fieldInformation = fakeTilesHelper.addTileFieldInformation(fieldInformation, "tile2")
seekerCrushZoneBlock.fieldInformation = fieldInformation

seekerCrushZoneBlock.nodeTexture = "characters/badeline/sleep00"

return seekerCrushZoneBlock
