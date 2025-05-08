using SharedLibrary.Models;

namespace SharedLibrary.Interfaces;

public interface IDiagramService
{
    IReadOnlyList<DraggableSvgElementModel> Elements { get; }

    int ElementCount { get; }

    void Add(DraggableSvgElementModel e);

    void Remove(DraggableSvgElementModel e);

    void RemoveAt(int index);

    bool Contains(DraggableSvgElementModel e);

    event Action? OnChange;
}
