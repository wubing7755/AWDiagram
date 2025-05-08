using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using SharedLibrary.Enums;
using SharedLibrary.Events;
using SharedLibrary.Interfaces;
using SharedLibrary.Models;
using SharedLibrary.Utils;

namespace SharedLibrary.Components;

/**
 * 画布功能：
 *      ① 添加元素
 *      ② 拖动元素
 *      ③ 复制粘贴元素
 */
public class Diagram : AWComponentBase
{
    /// <summary>
    /// 可视区域
    /// </summary>
    [Parameter]
    public ViewBox ViewBox { get; set; } = new ViewBox(0, 0, 512, 512);

    /// <summary>
    /// 物理尺寸-宽度
    /// </summary>
    [Parameter]
    public double Width { get; set; } = 512;

    /// <summary>
    /// 物理尺寸-高度
    /// </summary>
    /// <remarks>
    /// 默认高度是 100%
    /// </remarks>
    [Parameter]
    public double Height { get; set; } = 512;

    [Parameter]
    public ColorType ColorType { get; set; } = ColorType.Black;

    /// <summary>
    /// 显式坐标轴
    /// </summary>
    [Parameter] 
    public bool ShowAxis { get; set; } = true;

    [Parameter] 
    public string AxisColor { get; set; } = "red";

    [Parameter] 
    public string AxisDashArray { get; set; } = "30.9 5";

    [Parameter]
    public Func<MouseEventArgs, Task>? OnClick { get; set; }

    [Inject]
    public IDiagramService DiagramService { get; set; } = null!;

    private readonly float Version = 1.1f;
    private readonly string SvgNamespace = "http://www.w3.org/2000/svg";

    protected override void OnInitialized()
    {
        base.OnInitialized();

        DiagramService.OnChange += ForceImmediateRender;
    }

    protected override void BuildComponent(RenderTreeBuilder builder)
    {
        int seq = 0;

        builder.OpenElement(seq++, "div");
        builder.AddAttribute(seq++, "tabindex", "0");
        // 去除聚焦时元素显示的边框
        builder.AddAttribute(seq++, "style", "outline: none;");
        // HandleKeyDown
        builder.AddAttribute(seq++, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDown));

        builder.OpenElement(seq++, "svg");
        builder.AddAttribute(seq++, "xmlns", SvgNamespace);
        builder.AddAttribute(seq++, "version", $"{Version}");
        builder.AddAttribute(seq++, "viewBox", $"{ViewBox.MinX} {ViewBox.MinY} {ViewBox.Width} {ViewBox.Height}");

        /**
        * "[align] [meetOrSlice]"
        * align： viewBox在视口中的对齐方式
        * meetOrSlice： 如何适应宽高比，默认为meet
        */
        builder.AddAttribute(seq++, "preserveAspectRatio", $"{nameof(SvgAlign.xMidYMid)} {nameof(SvgMeetOrSlice.meet)}");
        builder.AddAttribute(seq++, "width", Width);
        builder.AddAttribute(seq++, "height", Height);
        builder.AddAttribute(seq++, "style", $"background-color: {ColorHelper.ConvertToString(ColorType)};");

        #region 坐标轴

        /* 翻转坐标系，与笛卡尔坐标系保持一致 */
        builder.OpenElement(seq++, "g");
        builder.AddAttribute(seq++, "transform", $"matrix(1 0 0 -1 {Width/2} {Height/2})");

        /* 原点 */
        builder.OpenElement(seq++, "circle");
        builder.AddMultipleAttributes(seq++, new Dictionary<string, object>
        {
            {"cx", "0" }, {"cy", "0" }, {"r", "5" }, {"fill", "red" }
        });
        builder.CloseElement();

        /* X轴 */
        builder.OpenElement(seq++, "line");
        builder.AddMultipleAttributes(seq++, new Dictionary<string, object>
        {
            {"x1", "-256" },
            {"y1", "0" },
            {"x2", "256" },
            {"y2", "0" },
            {"stroke", AxisColor },
            {"stroke-width", "1" },
            {"stroke-dasharray", AxisDashArray }
        });
        builder.CloseElement();

        /* Y轴 */
        builder.OpenElement(seq++, "line");
        builder.AddMultipleAttributes(seq++, new Dictionary<string, object>
        {
            {"x1", "0" },
            {"y1", "-256" },
            {"x2", "0" },
            {"y2", "256" },
            {"stroke", AxisColor },
            {"stroke-width", "1" },
            {"stroke-dasharray", AxisDashArray }
        });
        builder.CloseElement();

        #endregion

        /* 渲染声明式子内容（ChildContent） */
        builder.AddContent(seq++, childBuilder =>
        {
            /* 渲染动态添加的元素（Elements） */
            foreach (var element in DiagramService.Elements.Where(e => !e.IsDeleted))
            {
                // 触发子组件的渲染逻辑
                childBuilder.OpenComponent(seq++, element.GetSvgType());
                builder.AddAttribute(seq++, "key", element.ObjectId);
                childBuilder.AddAttribute(seq++, "Model", element);
                childBuilder.CloseComponent();
            }
        });

        builder.CloseElement();
        builder.CloseElement();
        builder.CloseElement();
    }

    /// <summary>
    /// 允许自定义键盘处理事件
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public virtual async Task HandleKeyDown(KeyboardEventArgs args)
    {
        var keyEvent = new DiagramKeyEvent(args.Code, args.CtrlKey, args.AltKey, args.ShiftKey);
        EventBus.Publish<DiagramKeyEvent>(keyEvent);

        ForceImmediateRender();
        await Task.CompletedTask;
    }
}
