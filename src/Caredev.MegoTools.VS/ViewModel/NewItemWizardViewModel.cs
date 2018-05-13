using Caredev.MegoTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Caredev.MegoTools.Core.DbObjects;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit;
using Res = Caredev.MegoTools.Properties.Resources;
using Caredev.MegoTools.Core.Generates;

namespace Caredev.MegoTools.ViewModel
{
    public partial class NewItemWizardViewModel : ViewModelBase
    {
        public NewItemWizardViewModel()
        {
            Informations = new ObservableCollection<ConnectionInfoViewModel>(
                ConnectionInfo.Data.Select(a => new ConnectionInfoViewModel(a.ConnectionString, a.ProviderName)
                {
                    Title = a.Title
                }));
            if (Informations.Count > 0)
            {
                Information = Informations[0];
            }
        }

        public ObservableCollection<ConnectionInfoViewModel> Informations { get; }

        public ConnectionInfoViewModel Information
        {
            get { return _Information; }
            set
            {
                if (_Information != value)
                {
                    _Information = value;
                    Database = DatabaseBase.GetDatabase(value.ProviderName);
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Database));
                }
            }
        }
        private ConnectionInfoViewModel _Information;

        public IEnumerable<ElementItemViewModel> Items
        {
            get => _Items;
            set => Set(ref _Items, value);
        }
        private IEnumerable<ElementItemViewModel> _Items;

        public DatabaseBase Database { get; private set; }

        public bool IsLoading
        {
            get => _IsLoading;
            set => Set(ref _IsLoading, value);
        }
        private bool _IsLoading = false;

        public bool IsPluralization
        {
            get => _IsPluralization;
            set => Set(ref _IsPluralization, value);
        }
        private bool _IsPluralization = false;

        //IsPluralization
        public bool IsSaveConfig
        {
            get => _IsSaveConfig;
            set => Set(ref _IsSaveConfig, value);
        }
        private bool _IsSaveConfig = true;

        public string ConfigName
        {
            get => _ConfigName;
            set => Set(ref _ConfigName, value);
        }
        private string _ConfigName;

        public RelayCommand NewConnection
        {
            get
            {
                if (_NewConnection == null)
                {
                    _NewConnection = new RelayCommand(obj =>
                    {
                        var create = new CreateConnectionViewModel()
                        {
                            Name = Res.CreateConnection_DefaultName
                        };
                        if (DialogHelper.ShowDialog(ref create) == true)
                        {
                            var info = new ConnectionInfoViewModel(create.Connection.Builder.ToString(), create.Database.ProviderName)
                            {
                                Title = create.Name
                            };
                            var item = new ConnectionInfo()
                            {
                                ProviderName = info.ProviderName,
                                ConnectionString = info.ConnectionString,
                                Title = info.Title
                            };
                            try
                            {
                                ConnectionInfo.Add(item);
                                ConnectionInfo.Save();
                                Informations.Add(info);
                                Information = info;
                            }
                            catch
                            {
                                ConnectionInfo.Remove(item);
                            }
                        }
                    });
                }
                return _NewConnection;
            }
        }
        private RelayCommand _NewConnection;

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
                    });
                }
                return _Close;
            }
        }
        private RelayCommand _Close;

        public string Language { get; set; }

        internal CodeGeneratorBase Generator { get; private set; }

        private ConnectionInfoViewModel LastLoadInformation;
        public void Load()
        {
            if (LastLoadInformation != Information)
            {
                var current = Information;
                LastLoadInformation = current;
                IsLoading = true;
                Items = new ElementItemViewModel[] { };
                Task.Factory.StartNew(delegate ()
                {
                    try
                    {
                        var aaa = Database.CreateCollection(Information);
                        var table = new ElementItemViewModel("表") { Image = "TableGroup.png", IsExpanded = true };
                        var view = new ElementItemViewModel("视图") { Image = "TableGroup.png" };
                        if (aaa.SupportSchema)
                        {
                            CreateObjectSchema(aaa.Objects.Values.Where(a => a.Kind == EObjectKind.Table), table);
                            CreateObjectSchema(aaa.Objects.Values.Where(a => a.Kind == EObjectKind.View), view);
                        }
                        else
                        {
                            CreateObject(aaa.Objects.Values.Where(a => a.Kind == EObjectKind.Table), table);
                            CreateObject(aaa.Objects.Values.Where(a => a.Kind == EObjectKind.View), view);
                        }
                        if (LastLoadInformation == current)
                        {
                            DialogHelper.Execute(() => Items = new ElementItemViewModel[] { table, view });
                        }
                    }
                    catch (Exception ex)
                    {
                        DialogHelper.Execute(() =>
                        {
                            DialogHelper.MessageBox(ex.Message);
                        });
                    }
                    finally
                    {
                        DialogHelper.Execute(() => IsLoading = false);
                    }
                });
            }
        }

        public void Finish()
        {
            var roots = Items.Where(a => a.IsChecked != false).ToArray();
            var generator =Language== "VisualBasic"
                ? (CodeGeneratorBase)new VisualBasicCodeGenerator(IsPluralization) 
                : (CodeGeneratorBase)new CSharpCodeGenerator(IsPluralization);
            foreach (var item in Items)
            {
                if (item.IsChecked != false)
                {
                    RegisteItem(generator, item);
                }
            }
            Generator = generator;
            DialogHelper.Close(true);
        }

        private void RegisteItem(CodeGeneratorBase generator, ElementItemViewModel item)
        {
            var data = item.Data;
            if (data == null)
            {
                if (item.IsChecked != false)
                {
                    foreach (var subitem in item.Children)
                    {
                        RegisteItem(generator, subitem);
                    }
                }
            }
            else
            {
                if (item.IsChecked == true)
                {
                    switch (data.Kind)
                    {
                        case EObjectKind.Table:
                        case EObjectKind.View:
                            generator.Add((ObjectElement)data,
                                item.Children.Where(a => a.IsChecked == true)
                                    .Select(b => b.Data)
                                    .OfType<ColumnElement>());
                            break;
                    }
                }
            }
        }

        private void CreateObjectSchema(IEnumerable<ObjectElement> items, ElementItemViewModel parent)
        {
            var query = from a in items
                        group a by a.Schema into g
                        select new { g.Key, Elements = g };
            var isfirst = true;
            foreach (var item in query)
            {
                var schema = new ElementItemViewModel(item.Key, parent) { Image = "Schema.png" };
                if (isfirst)
                {
                    schema.IsExpanded = true;
                    isfirst = !isfirst;
                }
                CreateObject(item.Elements, schema);
            }
        }

        private void CreateObject(IEnumerable<ElementBase> items, ElementItemViewModel parent)
        {
            foreach (var subitem in items)
            {
                var element = new ElementItemViewModel(subitem, parent);
                if (subitem is ObjectElement objs)
                {
                    foreach (var column in objs.Columns.Values.Where(a => a.IsKey))
                    {
                        new ElementItemViewModel(column, element);
                    }
                    foreach (var column in objs.Columns.Values.Where(a => !a.IsKey))
                    {
                        new ElementItemViewModel(column, element);
                    }
                }
            }
        }
    }
}
