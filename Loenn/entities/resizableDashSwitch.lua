local utils = require("utils")
local entities = require("entities")
local drawableSprite = require("structs.drawable_sprite")

local resizableDashSwitch = {}

local directions = {"Up", "Down", "Left", "Right"}

resizableDashSwitch.name = "SSMHelper/ResizableDashSwitch"
resizableDashSwitch.justification = {0.5, 1.0}
resizableDashSwitch.placements = {}
for i, dir in ipairs(directions) do
    local horizontal = dir == "Left" or dir == "Right"
    resizableDashSwitch.placements[i] = {
        name = dir,
        data = {
            width = horizontal and 16 or 8,
            height = horizontal and 8 or 16,
            orientation = dir,
            persistent = false,
            actLikeTouchSwitch = true,
            attachToSolid = true
        }
    }
end

resizableDashSwitch.fieldInformation = {
    orientation = {
        options = directions
    }
}

resizableDashSwitch.canResize = function(room, entity)
    local direction = entity.orientation
    if direction == "Up" or direction == "Down" then
        return true, false
    else
        return false, true
    end
end

-- thank you to brokemia for basically all of the following

resizableDashSwitch.resize = function(room, entity, offsetX, offsetY, directionX, directionY)
    local canHorizontal, canVertical = entities.canResize(room, layer, entity)
    local minimumWidth = 16

    local oldWidth = entity.width or 0
    local newWidth = oldWidth

    if offsetX ~= 0 and canHorizontal then
        newWidth += offsetX * math.abs(directionX)

        if minimumWidth <= newWidth then
            entity.width = newWidth

            if directionX < 0 then
                entity.x -= offsetX
            end

            madeChanges = true
        end
    end

    if offsetY ~= 0 and canVertical then
        newWidth += offsetY * math.abs(directionY)

        if minimumWidth <= newWidth then
            entity.width = newWidth

            if directionY < 0 then
                entity.y -= offsetY
            end

            madeChanges = true
        end
    end

    return madeChanges
end

resizableDashSwitch.updateResizeSelection = function(room, entity, node, selection, offsetX, offsetY, directionX, directionY)
    local newSelection = resizableDashSwitch.selection(room, entity)

    selection.x = newSelection.x
    selection.y = newSelection.y
    selection.width = newSelection.width
    selection.height = newSelection.height
end

resizableDashSwitch.selection = function(room, entity)
    return ({
        ["Up"] = utils.rectangle(entity.x - entity.width / 2, entity.y - 4, entity.width, 4),
        ["Left"] = utils.rectangle(entity.x, entity.y - entity.width / 2, 4, entity.width),
        ["Right"] = utils.rectangle(entity.x - 4, entity.y - entity.width / 2, 4, entity.width),
        ["Down"] = utils.rectangle(entity.x - entity.width / 2, entity.y, entity.width, 4)
    })[entity.orientation]
end

local rotations = {
    ["Up"] = 0,
    ["Right"] = math.pi / 2,
    ["Left"] = -math.pi / 2,
    ["Down"]  = math.pi
}

resizableDashSwitch.rotation = function(room, entity)
    return rotations[entity.orientation]
end

resizableDashSwitch.texture = "objects/SSMHelper/bigDashSwitch/bigSwitch00"

resizableDashSwitch.scale = function(room, entity)
    local horizontal = entity.orientation == "Left" or entity.orientation == "Right"
    local drawable = drawableSprite.fromTexture(resizableDashSwitch.texture, {x = entity.x, y = entity.y})
    if drawable then
        return (entity.width) / (drawable.meta.realWidth), 1.0
    end
    return 1.0, 1.0
end

return resizableDashSwitch
