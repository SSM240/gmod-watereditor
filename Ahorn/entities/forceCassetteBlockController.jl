module SSMForceCassetteBlockController

using ..Ahorn, Maple

@mapdef Entity "SSMHelper/ForceCassetteBlockController" ForceCassetteBlockController(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Force Cassette Block Controller (SSM Helper)" => Ahorn.EntityPlacement(
        ForceCassetteBlockController
    )
)

function Ahorn.selection(entity::ForceCassetteBlockController)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x - 12, y - 12, 24, 24)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::ForceCassetteBlockController, room::Maple.Room)
    Ahorn.drawImage(ctx, "ahorn/SSMHelper/forcecassetteblockcontroller", -12, -12)
end

end
