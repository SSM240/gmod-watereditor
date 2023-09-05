local drawableSprite = require("structs.drawable_sprite")

local barrierDashSwitch = {}

local directions = {"Left", "Right"}

barrierDashSwitch.name = "SSMHelper/BarrierDashSwitch"
barrierDashSwitch.placements = {}
for i, dir in ipairs(directions) do
    barrierDashSwitch.placements[i] = {
        name = dir,
        data = {
            orientation = dir,
            persistent = false,
            spritePath = "objects/SSMHelper/barrierDashSwitch"
        }
    }
end
barrierDashSwitch.fieldInformation = {
    orientation = {
        options = directions
    }
}

function barrierDashSwitch.sprite(room, entity)
    local rightSide = entity.orientation == "Right"
    local texture = (entity.spritePath ~= "" and entity.spritePath or "objects/temple") .. "/dashButton00"
    local sprite = drawableSprite.fromTexture(texture, entity)

    if rightSide then
        sprite:addPosition(0, 8)
        sprite.rotation = math.pi

    else
        sprite:addPosition(8, 8)
        sprite.rotation = 0
    end

    return sprite
end

return barrierDashSwitch