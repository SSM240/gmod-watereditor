module SSMHelperTheoKillBoxTrigger

using ..Ahorn, Maple

@mapdef Trigger "SSMHelper/TheoKillBoxTrigger" TheoKillBox(x::Integer, y::Integer, width::Integer=16, height::Integer=16)

const placements = Ahorn.PlacementDict(
    "Theo Kill Box (SSM Helper)" => Ahorn.EntityPlacement(
        TheoKillBox,
        "rectangle"
    )
)

end
