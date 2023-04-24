module SSMHelperHoldableDashTrigger

using ..Ahorn, Maple

@mapdef Trigger "SSMHelper/HoldableDashTrigger" HoldableDashTrigger(x::Integer, y::Integer, width::Integer=16, height::Integer=16,
    mode::String="EnableOnStay")

const modes = String["EnableOnStay", "DisableOnStay", "EnableToggle", "DisableToggle"]

const placements = Ahorn.PlacementDict(
    "Dash With Holdable Trigger (SSM Helper)" => Ahorn.EntityPlacement(
        HoldableDashTrigger,
        "rectangle"
    )
)

Ahorn.editingOptions(entity::HoldableDashTrigger) = Dict{String, Any}(
    "mode" => modes
)

end