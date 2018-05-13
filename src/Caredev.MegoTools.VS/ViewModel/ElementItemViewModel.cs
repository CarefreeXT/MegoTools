using Caredev.MegoTools.Core.DbObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Caredev.MegoTools.ViewModel
{
    public class ElementItemViewModel : ViewModelBase
    {
        public const string ImagePathRoot = "/Caredev.MegoTools;component/Resources/";

        public ElementItemViewModel(string title, ElementItemViewModel parent = null)
        {
            Title = title;
            _Children = new ObservableCollection<ElementItemViewModel>();
            if (parent != null)
            {
                parent._Children.Add(this);
            }
            Parent = parent;
        }

        public ElementItemViewModel(ElementBase data, ElementItemViewModel parent = null)
            : this(data.Name, parent)
        {
            Data = data;
        }

        public string Title { get; }

        public ElementItemViewModel Parent { get; }
        
        public bool? IsChecked
        {
            get => _IsChecked;
            set
            {
                if (Set(ref _IsChecked, value))
                {
                    ProcessIsChecked();
                }
            }
        }
        private bool? _IsChecked = false;

        public bool IsExpanded
        {
            get => _IsExpanded;
            set => Set(ref _IsExpanded, value);
        }
        public bool _IsExpanded = false;

        public string Image
        {
            get
            {
                if (_Image == null && Data != null)
                {
                    switch (Data.Kind)
                    {
                        case EObjectKind.Table:
                        case EObjectKind.View:
                            _Image = ImagePathRoot + "Table.png";
                            break;
                        case EObjectKind.Column:
                            if (((ColumnElement)Data).IsKey)
                            {
                                _Image = ImagePathRoot + "KeyColumn.png";
                            }
                            else
                            {
                                _Image = ImagePathRoot + "Column.png";
                            }
                            break;
                    }
                }
                return _Image;
            }
            set { _Image = ImagePathRoot + value; }
        }
        private string _Image;

        public ElementBase Data { get; }
        public IEnumerable<ElementItemViewModel> Children => _Children;
        private ObservableCollection<ElementItemViewModel> _Children;

        private void ProcessIsChecked()
        {
            if (CheckAllowProcess())
            {
                try
                {
                    SetIsProcessing(true);
                    ProcessParent();
                    ProcessChildren();
                }
                finally
                {
                    SetIsProcessing(false);
                }
            }
        }
        private bool IsProcessing = false;
        private void ProcessParent()
        {
            if (Parent != null)
            {
                if (Parent.Children.All(a => a.IsChecked == true))
                {
                    Parent.IsChecked = true;
                }
                else if (Parent.Children.All(a => a.IsChecked == false))
                {
                    Parent.IsChecked = false;
                }
                else
                {
                    Parent.IsChecked = null;
                }
                Parent.ProcessParent();
            }
        }
        private void ProcessChildren()
        {
            if (IsChecked.HasValue && Children.Any())
            {
                foreach (var item in Children)
                {
                    item.IsChecked = IsChecked;
                    item.ProcessChildren();
                }
            }
        }
        private void SetIsProcessing(bool value)
        {
            IsProcessing = value;
            if (Parent != null)
            {
                Parent.SetIsProcessing(value);
            }
        }
        private bool CheckAllowProcess()
        {
            if (this.IsProcessing)
            {
                return false;
            }
            if (Parent != null)
            {
                return Parent.CheckAllowProcess();
            }
            return true;
        }
    }
}
