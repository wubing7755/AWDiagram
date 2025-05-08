namespace SharedLibrary.Events;

public class DiagramKeyEvent
{
    public string Code { get; }
    public bool CtrlKey { get; }
    public bool ShiftKey { get; }
    public bool AltKey { get; }

    public DiagramKeyEvent(string key, bool isCtrlPressed, bool isShiftPressed, bool isAltPressed)
    {
        Code = key;
        CtrlKey = isCtrlPressed;
        ShiftKey = isShiftPressed;
        AltKey = isAltPressed;
    }
}
