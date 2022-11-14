local utils = require("utils")

local redirectableBooster = {}

redirectableBooster.name = "SSMHelper/RedirectableBooster"
redirectableBooster.depth = -8500
redirectableBooster.texture = "objects/SSMHelper/boosters/pink/boosterPink00"
redirectableBooster.placements = {
    name = "normal"
}

-- padding on the texture makes the default selection larger than it should be
function redirectableBooster.selection(room, entity)
    return utils.rectangle(entity.x - 9, entity.y - 9, 18, 18)
end

return redirectableBooster
