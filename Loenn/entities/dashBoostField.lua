local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local dashBoostField = {}

dashBoostField.name = "SSMHelper/DashBoostField"
dashBoostField.placements = {
    {
        name = "useDash",
        data = {
            preserveDash = false,
            color = "ffffff",
            dashSpeedMultiplier = 1.7,
            timeRateMultiplier = 0.65,
            radius = 1.5
        }
    },
    {
        name = "refillDash",
        data = {
            refillDash = true,
            color = "ffffff",
            dashSpeedMultiplier = 1.7,
            timeRateMultiplier = 0.65,
            radius = 1.5
        }
    }
}
dashBoostField.fieldInformation = {
    color = {
        fieldType = "color"
    }
}

function dashBoostField.draw(room, entity, viewport)
    local circleSprite = drawableSprite.fromTexture("objects/SSMHelper/dashBoostField/white", entity)
    local color = utils.getColor(entity.color or "ffffff")
    circleSprite:setColor(color)
    circleSprite:draw()

    local radius = (entity.radius or 1.5) * 8
    local x, y = entity.x or 0, entity.y or 0
    drawing.callKeepOriginalColor(function()
        local bigCircleColor = table.shallowcopy(color)
        bigCircleColor[4] = 0.6
        love.graphics.setColor(bigCircleColor)
        love.graphics.circle("line", x, y, radius)
    end)
end

function dashBoostField.selection(room, entity)
    return utils.rectangle(entity.x - 8, entity.y - 8, 16, 16)
end

return dashBoostField