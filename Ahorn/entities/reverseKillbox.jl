module SSMReverseKillbox

using ..Ahorn, Maple

const defaultBlockWidth = 16

const killBoxColor = (0.8, 0.4, 0.4, 0.8)

@mapdef Entity "SSMHelper/ReverseKillbox" ReverseKillbox(x::Integer, y::Integer, width::Integer=defaultBlockWidth)

const placements = Ahorn.PlacementDict(
    "Reverse Killbox (SSM Helper)" => Ahorn.EntityPlacement(
        ReverseKillbox,
        "rectangle"
    ),
)

Ahorn.minimumSize(entity::ReverseKillbox) = 8, 0
Ahorn.resizable(entity::ReverseKillbox) = true, false

function Ahorn.selection(entity::ReverseKillbox)
    x, y = Ahorn.position(entity)

    width = Int(get(entity.data, "width", 8))
    height = 32

    return Ahorn.Rectangle(x, y, width, height)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::ReverseKillbox, room::Maple.Room)
    width = Int(get(entity.data, "width", 32))
    height = 32

    Ahorn.drawRectangle(ctx, 0, 0, width, height, killBoxColor, (0.0, 0.0, 0.0, 0.0))
end

function Ahorn.renderSelectedAbs(ctx::Ahorn.Cairo.CairoContext, entity::ReverseKillbox)
    x, y = Ahorn.position(entity)

    width = Int(get(entity.data, "width", 0))
    height = 32

    cx = x + floor(Int, width / 2)
    cy = y + 32

    Ahorn.drawArrow(ctx, cx, cy, cx, cy + 24, Ahorn.colors.selection_selected_fc, headLength=6)
end

end