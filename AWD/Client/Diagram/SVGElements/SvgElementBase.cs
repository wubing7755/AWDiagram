using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using AWUI.Models;
using System.Diagnostics.CodeAnalysis;

namespace AWUI.Components;

public abstract class SvgElementBase<TValue> : AWComponentBase where TValue : DraggableSvgElementModel
{
    /// <summary>
    /// 数据层
    /// </summary>
    [Parameter, NotNull, EditorRequired]
    public TValue? Model { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// 元素引用
    /// </summary>
    protected ElementReference ElementRef { get; set; }

    /// <summary>
    /// dotnet引用
    /// </summary>
    protected DotNetObjectReference<SvgElementBase<TValue>>? DotNetRef { get; private set; }

    public abstract void Render(RenderTreeBuilder builder);

    protected virtual ValueTask InitializeElementAsync() => ValueTask.CompletedTask;

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        Render(builder);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            DotNetRef = DotNetObjectReference.Create(this);
            await InitializeElementAsync();
        }
    }

    protected override void DisposeManagedResources()
    {
        DotNetRef?.Dispose();
    }

    [JSInvokable]
    public virtual async Task SelectedElement()
    {
        Model.IsSelected = true;

        await Task.CompletedTask;
    }

    [JSInvokable]
    public virtual async Task UnSelectedElement()
    {
        Model.IsSelected = false;

        await Task.CompletedTask;
    }
}
