-- local enums = require("consts.celeste_enums")
-- local utils = require("utils")

-- local crystalBombBadelineBoss = {}

-- crystalBombBadelineBoss.name = "SSMHelper/CrystalBombBadelineBoss"
-- crystalBombBadelineBoss.depth = 0
-- crystalBombBadelineBoss.nodeLineRenderType = "line"
-- crystalBombBadelineBoss.texture = "objects/SSMHelper/crystalBombBadelineBoss/charge00"
-- crystalBombBadelineBoss.nodeLimits = {0, -1}
-- crystalBombBadelineBoss.fieldInformation = {
--     patternIndex = {
--         fieldType = "integer",
--         options = enums.badeline_boss_shooting_patterns,
--         editable = false
--     }
-- }
-- crystalBombBadelineBoss.placements = {
--     name = "normal",
--     data = {
--         patternIndex = 1,
--         cameraPastY = 120.0,
--         cameraLockY = true,
--         canChangeMusic = true,
--         music = "",
--         disableCameraLock = false
--     }
-- }
-- function crystalBombBadelineBoss.selection(room, entity)
--     local main = utils.rectangle(entity.x - 19, entity.y - 13, 38, 30)
--     local nodes = {}

--     if entity.nodes then
--         for i, node in ipairs(entity.nodes) do
--             nodes[i] = utils.rectangle(node.x - 19, node.y - 13, 38, 30)
--         end
--     end

--     return main, nodes
-- end

-- return crystalBombBadelineBoss
