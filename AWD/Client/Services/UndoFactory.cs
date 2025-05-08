using SharedLibrary.Models;

namespace SharedLibrary.Services;

public static class UndoFactory
{
    public static ActionUndoItem<DraggableSvgElementModel> SvgElementPasteUndoItem(DraggableSvgElementModel model)
    {
        var undoItem = new ActionUndoItem<DraggableSvgElementModel>
        (
            model,
            (ctx) => {
                model.IsDeleted = false;
                return Result.Ok; 
            },
            (ctx) => {
                model.IsDeleted = true;
                return Result.Ok; 
            },
            "svg 元素 Ctrl + V 的 redo 和 undo 操作"
        );

        return undoItem;
    }

    public static ActionUndoItem<DraggableSvgElementModel> SvgElementDeleteUndoItem(DraggableSvgElementModel model)
    {
        var undoItem = new ActionUndoItem<DraggableSvgElementModel>
        (
            model,
            (ctx) => {
                model.IsDeleted = true;
                return Result.Ok;
            },
            (ctx) => {
                model.IsDeleted = false;
                return Result.Ok;
            },
            "svg 元素 Delete 的 redo 和 undo 操作"
        );

        return undoItem;
    }
}
