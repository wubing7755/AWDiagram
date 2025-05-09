using Microsoft.JSInterop;
using AWUI.Models;

namespace AWUI.Components;

public abstract class DraggableSvgElement<TValue> : SvgElementBase<TValue> where TValue : DraggableSvgElementModel
{
    protected override async ValueTask InitializeElementAsync()
    {
        await JSRuntime.InvokeVoidAsync("initializeDraggableSVGElement", ElementRef, DotNetRef, Model.X, Model.Y);
    }

    [JSInvokable]
    public virtual async Task UpdatePosition(double x, double y)
    {
        Model.X = x;
        // 采用笛卡尔坐标系，与SVG坐标Y轴相反
        Model.Y = -y;

        StateHasChanged();

        await Task.Delay(0);
    }
}

