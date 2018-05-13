using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Caredev.MegoTools.Core;
using Caredev.MegoTools.View;
using System.Windows;
using System.Windows.Threading;

namespace Caredev.MegoTools.ViewModel
{
    public class AdvancedPropertyViewModel : ViewModelBase, IDisposable
    {

        private DispatcherTimer Timer;

        public AdvancedPropertyViewModel(DbConnectionStringBuilder builder)
        {
            Builder = builder;

            Timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
            Timer.Tick += (s, e1) => { OnPropertyChanged(nameof(ConnectionString)); };
            Timer.Interval = TimeSpan.FromMilliseconds(1000);
            Timer.Start();
        }

        public string ConnectionString => Builder.ToString();

        public DbConnectionStringBuilder Builder { get; }

        public void Refresh()
        {
            OnPropertyChanged(nameof(ConnectionString));
        }

        public void Dispose()
        {
            if (!_IsDisposed)
            {
                _IsDisposed = true;
                Timer?.Stop();
            }
        }
        private bool _IsDisposed = false;

        public RelayCommand Close
        {
            get
            {
                if (_Close == null)
                {
                    _Close = new RelayCommand(obj =>
                    {
                        if (obj != null)
                        {
                            DialogHelper.Close(true);
                        }
                        else
                        {
                            DialogHelper.Close();
                        }
                        Timer.Stop();
                    });
                }
                return _Close;
            }
        }
        private RelayCommand _Close;
    }
}
