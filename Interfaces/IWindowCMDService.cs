namespace DiabloTwoMFTimer.Interfaces;

public interface IWindowCMDService
{
    void Initialize();
    void SetWindowPosition(string position);
    void SetWindowPositionTopLeft();
    void SetWindowPositionTopRight();
    void SetWindowPositionBottomLeft();
    void SetWindowPositionBottomRight();
}
