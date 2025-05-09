using Microsoft.AspNetCore.Components.Rendering;
using AWUI.Models;

namespace AWUI.Components;

public class Rect : DraggableSvgElement<RectModel>
{
    public override void Render(RenderTreeBuilder builder)
    {
        int seq = 0;
        builder.OpenElement(seq++, "rect");
        builder.AddAttribute(seq++, "x", Model.X);
        builder.AddAttribute(seq++, "y", Model.Y);
        builder.AddAttribute(seq++, "width", Model.Width);
        builder.AddAttribute(seq++, "height", Model.Height);
        builder.AddAttribute(seq++, "fill", Model.Fill);
        builder.AddElementReferenceCapture(seq, eRef => ElementRef = eRef);
        builder.CloseElement();
    }
}
