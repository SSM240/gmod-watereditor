AddCSLuaFile()

properties.Add("watereditor_refresh", {
    MenuLabel = "#watereditor.properties.refresh",
    MenuIcon = "icon16/water.png",
    Order = 90000,
    PrependSpacer = true,
    Filter = function(self, ent)
        return ent:GetClass() == "edit_water"
    end,
    Action = function(self, ent)
        if (ent:GetClass() == "edit_water") then
            ent:ForceApplyProperties()
        end
    end
})
