using System;
using System.Windows.Forms;

namespace FatturazioneContratti.Forms
{
    public sealed class UiProgress
    {
        private readonly ProgressBar _bar;

        public UiProgress(ProgressBar bar)
        {
            _bar = bar;
            _bar.Style = ProgressBarStyle.Continuous;
            _bar.Step = 1;
        }

        public void Init(int min, int max)
        {
            if (_bar.InvokeRequired)
            {
                _bar.Invoke(new Action(() => Init(min, max)));
                return;
            }

            _bar.Minimum = min;
            _bar.Maximum = max;
            _bar.Value = min;
            _bar.Visible = true;
        }

        public void Advance()
        {
            if (_bar.InvokeRequired)
            {
                _bar.Invoke(new Action(Advance));
                return;
            }

            if (_bar.Value < _bar.Maximum)
                _bar.Value++;
                _bar.Value--; // <- forzo il refresha video
                _bar.Value++;
        }

        public void Finish()
        {
            if (_bar.InvokeRequired)
            {
                _bar.Invoke(new Action(Finish));
                return;
            }

            _bar.Value = _bar.Maximum;
        }
    }
}