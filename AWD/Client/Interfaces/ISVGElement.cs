namespace SharedLibrary.Interfaces;

public interface ISVGElement : IDisposable
{
    bool IsSelected { get; set; }

    bool IsDeleted { get; set; }

    int ZIndex { get; set; }
}
