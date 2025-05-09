using Microsoft.AspNetCore.Components.Rendering;
using AWUI.Models;

namespace AWUI.Components;

public class Circle : DraggableSvgElement<CircleModel>
{
    public override void Render(RenderTreeBuilder builder)
    {
        int seq = 0;
        builder.OpenElement(seq++, "circle");
        builder.AddAttribute(seq++, "cx", Model.X);
        builder.AddAttribute(seq++, "cy", Model.Y);
        builder.AddAttribute(seq++, "r", Model.Radius);
        builder.AddAttribute(seq++, "fill", Model.Fill);
        builder.AddElementReferenceCapture(seq, eRef => ElementRef = eRef);
        builder.CloseElement();
    }
}
