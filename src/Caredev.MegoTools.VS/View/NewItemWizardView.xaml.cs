using Caredev.MegoTools.ViewModel;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace Caredev.MegoTools.View
{
    /// <summary>
    /// NewItemWizardView.xaml 的交互逻辑
    /// </summary>
    public partial class NewItemWizardView : Window
    {
        public NewItemWizardView()
        {
            InitializeComponent();
        }

        private void Wizard_PageChanged(object sender, RoutedEventArgs e)
        {
            var wizard = (Wizard)sender;
            if (wizard.CurrentPage.CanFinish == true)
            {
                var vm = DataContext as NewItemWizardViewModel;
                vm.Load();
            }
        }

        private void Wizard_Finish(object sender, Xceed.Wpf.Toolkit.Core.CancelRoutedEventArgs e)
        {
            ((NewItemWizardViewModel)DataContext).Finish();
        }
    }
}
