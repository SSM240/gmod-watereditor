module SSMZeroGravBoundsController

using ..Ahorn, Maple

@mapdef Entity "SSMHelper/ZeroGravBoundsController" ZeroGravBoundsController(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Zero Grav Bounds Controller (SSM Helper)" => Ahorn.EntityPlacement(
        ZeroGravBoundsController
    )
)

function Ahorn.selection(entity::ZeroGravBoundsController)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x - 12, y - 12, 24, 24)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::ZeroGravBoundsController, room::Maple.Room)
    Ahorn.drawImage(ctx, "ahorn/SSMHelper/zerogravcontroller", -12, -12)
end

end
