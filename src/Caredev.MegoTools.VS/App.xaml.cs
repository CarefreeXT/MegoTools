using Caredev.MegoTools.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Caredev.MegoTools
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            NewItemWizardViewModel vm = null;
            if (DialogHelper.ShowDialog(ref vm) == true)
            {

            }
        }
    }
}
