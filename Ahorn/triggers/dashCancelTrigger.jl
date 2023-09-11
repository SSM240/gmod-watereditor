module SSMHelperDashCancelTrigger

using ..Ahorn, Maple

@mapdef Trigger "SSMHelper/DashCancelTrigger" DashCancelTrigger(x::Integer, y::Integer, width::Integer=16, height::Integer=16, playSound::Bool=true)

const placements = Ahorn.PlacementDict(
    "Dash Cancel Trigger (SSM Helper)" => Ahorn.EntityPlacement(
        DashCancelTrigger,
        "rectangle"
    )
)

end