local holdableDashTrigger = {}

local modes = {"EnableOnStay", "DisableOnStay", "EnableToggle", "DisableToggle"}

holdableDashTrigger.name = "SSMHelper/HoldableDashTrigger"
holdableDashTrigger.placements = {
    name = "normal",
    data = {
        mode = "EnableOnStay"
    }
}

holdableDashTrigger.fieldInformation = {
    mode = {
        options = modes,
        editable = false
    }
}

return holdableDashTrigger
