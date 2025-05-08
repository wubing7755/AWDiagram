using Microsoft.JSInterop;
using SharedLibrary.Components;
using SharedLibrary.Enums;

namespace SharedLibrary.Models;

public abstract class SvgElementModel
{
    public Guid ObjectId { get; set; } = Guid.NewGuid();

    public SVGElementType Type { get; set; }

    /// <summary>
    /// 删除
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// 选中
    /// </summary>
    public bool IsSelected { get; set; } = false;

    /// <summary>
    /// 复制
    /// </summary>
    public bool IsCopyed { get; set; } = false;

    public Type GetSvgType()
    {
        return Type switch
        {
            SVGElementType.Rect => typeof(Rect),
            SVGElementType.Circle => typeof(Circle),
            _ => throw new NotImplementedException($"SVGElementType {Type} not implemented")
        };
    }
}
