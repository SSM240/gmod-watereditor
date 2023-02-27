local drawableSprite = require("structs.drawable_sprite")

local resizableDashSwitch = {}

local directions = {"Up", "Down", "Left", "Right"}

resizableDashSwitch.name = "SSMHelper/ResizableDashSwitch"
resizableDashSwitch.placements = {}
for i, dir in ipairs(directions) do
    resizableDashSwitch.placements[i] = {
        name = dir,
        data = {
            width = 16,
            height = 16,
            orientation = dir,
            persistent = false,
            actLikeTouchSwitch = true,
            attachToSolid = true,
            bounceInDreamBlock = true
        }
    }
end

resizableDashSwitch.fieldInformation = {
    orientation = {
        options = directions
    }
}

resizableDashSwitch.canResize = {true, true}

local justifications = {
    ["Up"] = {0.0, 0.0},
    ["Down"] = {1.0, 1.0},
    ["Left"] = {1.0, 0.0},
    ["Right"] = {0.0, 1.0}
}

resizableDashSwitch.justification = function(room, entity)
    return table.unpack(justifications[entity.orientation])
end

local rotations = {
    ["Up"] = 0,
    ["Down"] = math.pi,
    ["Left"] = -math.pi / 2,
    ["Right"] = math.pi / 2
}

resizableDashSwitch.rotation = function(room, entity)
    return rotations[entity.orientation]
end

resizableDashSwitch.texture = "objects/SSMHelper/bigDashSwitch/bigSwitch00"

resizableDashSwitch.scale = function(room, entity)
    local horizontal = entity.orientation == "Left" or entity.orientation == "Right"

    -- dirty awful hack (thank you CommunalHelper for the idea :gladstare:)
    if horizontal then
        entity.width = 8
    else
        entity.height = 8
    end

    local drawable = drawableSprite.fromTexture(resizableDashSwitch.texture, {x = entity.x, y = entity.y})
    if drawable then
        local scaleFactor
        if horizontal then
            scaleFactor = (entity.height) / (drawable.meta.realWidth)
        else
            scaleFactor = (entity.width) / (drawable.meta.realWidth)
        end
        return scaleFactor, 1.0
    end
    return 1.0, 1.0
end

return resizableDashSwitch
