module SSMHelperBinoStorageTrigger

using ..Ahorn, Maple

@mapdef Trigger "SSMHelper/BinoStorageTrigger" BinoStorageTrigger(x::Integer, y::Integer, width::Integer=16, height::Integer=16)

const placements = Ahorn.PlacementDict(
    "Bino Storage Trigger (SSM Helper)" => Ahorn.EntityPlacement(
        BinoStorageTrigger,
        "rectangle"
    )
)

end