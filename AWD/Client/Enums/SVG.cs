namespace AWUI.Enums;

/// <summary>
/// 对齐方式，对应 preserveAspectRatio 的前半部分：xMinYMin、xMidYMid 等
/// </summary>
/// <remarks>
/// 是否强制统一缩放，当 SVG 的 viewbox 属性与视图属性宽高比不一致时使用。
/// </remarks>
public enum SvgAlign
{
    xMinYMin,   // 左上角对齐
    xMidYMin,   // 顶部水平居中
    xMaxYMin,   // 右上角对齐
    xMinYMid,   // 左侧垂直居中
    xMidYMid,   // 水平和垂直居中（默认）
    xMaxYMid,   // 右侧垂直居中
    xMinYMax,   // 左下角对齐
    xMidYMax,   // 底部水平居中
    xMaxYMax,   // 右下角对齐
    none        // 不保留比例、不进行缩放对齐
}

/// <summary>
/// 缩放策略，对应 preserveAspectRatio 的后半部分：meet 或 slice
/// </summary>
public enum SvgMeetOrSlice
{
    meet,   // 默认，缩放至完全显示并保持比例，可能留空白
    slice   // 缩放至填满视口并保持比例，可能裁剪部分内容
}
