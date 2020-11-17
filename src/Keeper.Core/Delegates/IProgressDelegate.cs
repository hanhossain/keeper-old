namespace Keeper.Core.Delegates
{
    public interface IProgressDelegate
    {
        void ShowProgressIndicator();

        void DismissProgressIndicator();
    }
}
