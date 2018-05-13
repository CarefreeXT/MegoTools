using Caredev.MegoTools.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Caredev.MegoTools.View
{
    /// <summary>
    /// ServerConnectionView.xaml 的交互逻辑
    /// </summary>
    public partial class ServerConnectionView : UserControl
    {
        public ServerConnectionView()
        {
            InitializeComponent();
        }

        private bool IsProcessing = false;
        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (IsProcessing) return;
            try
            {
                IsProcessing = true;
                if (this.DataContext is ConnectionBaseViewModel connection)
                {
                    ((dynamic)connection).Password = password.Password;
                }
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ConnectionBaseViewModel connection)
            {
                connection.PropertyChanged += (ss, ee) =>
                {
                    if (ee.PropertyName == "Password" || ee.PropertyName == "")
                        password.Password = ((dynamic)connection).Password;
                };
                password.Password = ((dynamic)connection).Password;

                var type = connection.GetType();
                if (type.GetProperty("IntegratedSecurity") == null)
                {
                    rbIntegratedSecurity1.IsChecked = true;
                    spIntegratedScurity.Visibility = Visibility.Collapsed;
                }
                else
                {
                    rbIntegratedSecurity1.IsChecked = !rbIntegratedSecurity.IsChecked;
                }

                if (type.GetProperty("Database") == null)
                {
                    gbDatabase.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
