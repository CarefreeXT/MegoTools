using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using xceed = Xceed.Wpf.Toolkit;
using System.Windows.Threading;

namespace Caredev.MegoTools.ViewModel
{
    public static class DialogHelper
    {
        static DialogHelper()
        {
            _ViewModelMap = new Dictionary<Type, Type>();
            var types = typeof(DialogHelper).Assembly.GetTypes().Where(a => !a.IsAbstract && a.IsClass).ToArray();

            var views = types.Where(a => typeof(Window).IsAssignableFrom(a)).ToArray();
            var viewmodels = types.Where(a => typeof(ViewModelBase).IsAssignableFrom(a)).ToArray();

            var query = from a in views
                        from b in viewmodels
                        where b.Name.StartsWith(a.Name)
                        select new { View = a, ViewModel = b };
            foreach (var item in query)
            {
                _ViewModelMap.Add(item.ViewModel, item.View);
            }
        }
        static Dictionary<Type, Type> _ViewModelMap;

        public static Window Current { get; private set; }

        public static void MessageBox(string messageText, string caption = "")
        {
            xceed.MessageBox.Show(Current, messageText, caption);
        }

        public static bool? ShowDialog<T>(ref T viewmodel) where T : ViewModelBase
        {
            var oldWindows = Current;
            try
            {
                var window = (Window)Activator.CreateInstance(_ViewModelMap[typeof(T)]);
                Current = window;
                if (viewmodel != null)
                {
                    window.DataContext = viewmodel;
                }
                else
                {
                    viewmodel = window.DataContext as T;
                }
                if (oldWindows == null)
                {
                    window.Owner = Application.Current.MainWindow;
                }
                else
                {
                    window.Owner = oldWindows;
                }
                return window.ShowDialog();
            }
            finally
            {
                Current = oldWindows;
            }
        }

        public static void Close(bool? dialogResult = null)
        {
            Current.DialogResult = dialogResult;
            Current.Close();
        }

        public static void Execute(Action action)
        {
            if (Current != null)
            {
                Current.Dispatcher.Invoke(action);
            }
            else
            {
                Dispatcher.CurrentDispatcher.Invoke(action);
            }
        }
    }
}
