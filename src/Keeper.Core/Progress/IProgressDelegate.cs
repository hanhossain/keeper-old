namespace Keeper.Core.Progress
{
    public interface IProgressDelegate
    {
        void ShowProgressIndicator();

        void DismissProgressIndicator();

        void UpdateProgress(float progress);
    }
}
