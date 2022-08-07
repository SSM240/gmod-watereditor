local drawableSprite = require("structs.drawable_sprite")
local drawableRectangle = require("structs.drawable_rectangle")
local utils = require("utils")

local reverseKillbox = {}

reverseKillbox.name = "SSMHelper/ReverseKillbox"
reverseKillbox.canResize = {true, false}
reverseKillbox.placements = {
    name = "normal",
    data = {
        width = 8,
    }
}

rectColor = {0.8, 0.4, 0.4, 0.8}
arrowColor = {0.0, 0.0, 0.0, 0.25}
arrowTexture = "loenn/SSMHelper/downarrow"

function reverseKillbox.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 8, 32

    local centerX = x + width / 2
    local centerY = y + height / 2

    local rectSprite = drawableRectangle.fromRectangle("fill", x, y, width, height, rectColor)

    local arrowData = {x = centerX, y = centerY, color = arrowColor}
    local arrowSprite = drawableSprite.fromTexture(arrowTexture, arrowData)
    
    return {rectSprite, arrowSprite}
end

-- needed because the arrow texture is wider than the entity's minimum width
function reverseKillbox.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 8, 32
    return utils.rectangle(x, y, width, height)
end

return reverseKillbox
