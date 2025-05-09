using AWUI.Enums;

namespace AWUI.Models;

public class CircleModel : DraggableSvgElementModel
{
    public double Radius { get; set; } = 10.0f;

    public string Fill { get; set; } = "red";

    public CircleModel()
    {
        Type = SVGElementType.Circle;
    }
}
