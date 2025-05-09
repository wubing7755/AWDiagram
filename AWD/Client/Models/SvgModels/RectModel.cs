using AWUI.Enums;

namespace AWUI.Models;

public class RectModel : DraggableSvgElementModel
{
    public double Width { get; set; } = 20;

    public double Height { get; set; } = 20;

    public string Fill { get; set; } = "red";

    public RectModel()
    {
        Type = SVGElementType.Rect;
    }
}
