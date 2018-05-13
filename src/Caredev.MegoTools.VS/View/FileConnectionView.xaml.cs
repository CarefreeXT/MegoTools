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
using Microsoft.Win32;
using Caredev.MegoTools.ViewModel.Connections;

namespace Caredev.MegoTools.View
{
    /// <summary>
    /// FileConnectionView.xaml 的交互逻辑
    /// </summary>
    public partial class FileConnectionView : UserControl
    {
        private bool IsProcessing = false;

        public FileConnectionView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ConnectionBaseViewModel connection)
            {
                var type = connection.GetType();
                if (type.GetMethod("Create") == null)
                {
                    btnCreate.Visibility = Visibility.Collapsed;
                }

                if (type.Name.StartsWith("Excel"))
                {
                    gbHost.Visibility = Visibility.Collapsed;
                }
                else
                {
                    connection.PropertyChanged += (ss, ee) =>
                    {
                        if (ee.PropertyName == "Password")
                            password.Password = ((dynamic)connection).Password;
                    };
                    password.Password = ((dynamic)connection).Password;
                    password.PasswordChanged += Password_PasswordChanged;
                }
            }
        }

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

        private void Create(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is IFileConnectionViewModel connection)
            {
                var dialog = new SaveFileDialog
                {
                    DefaultExt = connection.DefaultExt,
                    Filter = connection.Filter
                };
                if (dialog.ShowDialog() == true)
                {
                    connection.FileName = dialog.FileName;
                    ((dynamic)connection).Create();
                }
            }
        }

        private void Browse(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is IFileConnectionViewModel connection)
            {
                var dialog = new OpenFileDialog
                {
                    CheckFileExists = true,
                    DefaultExt = connection.DefaultExt,
                    Filter = connection.Filter
                };
                if (dialog.ShowDialog() == true)
                {
                    connection.FileName = dialog.FileName;
                }
            }
        }
    }
}
