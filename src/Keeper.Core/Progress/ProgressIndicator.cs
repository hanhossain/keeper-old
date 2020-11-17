using System;

namespace Keeper.Core.Progress
{
    public class ProgressIndicator : IDisposable
    {
        private readonly IProgressDelegate _progressDelegate;

        public ProgressIndicator(IProgressDelegate progressDelegate)
        {
            _progressDelegate = progressDelegate ?? throw new ArgumentNullException(nameof(progressDelegate));
            progressDelegate.ShowProgressIndicator();
        }

        public void Dispose()
        {
            _progressDelegate.DismissProgressIndicator();
        }
    }
}
