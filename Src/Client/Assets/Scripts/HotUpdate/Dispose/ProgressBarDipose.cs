using System;

namespace Assets.Scripts.HotUpdate.Dispose
{
    internal class ProgressBarDipose : IDisposable
    {
        private UI.Common.ProgressBar _progressBar;
        public ProgressBarDipose(UI.Common.ProgressBar progressBar)
        {
            _progressBar = progressBar;
            _progressBar.gameObject.SetActive(true);
        }

        public void Dispose()
        {
            _progressBar.gameObject.SetActive(false);
        }
    }
}
