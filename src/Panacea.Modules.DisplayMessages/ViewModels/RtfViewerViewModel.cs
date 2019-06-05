using Panacea.Modularity.UiManager;
using Panacea.Modules.DisplayMessages.Views;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace Panacea.Modules.DisplayMessages.ViewModels
{
    [View(typeof(RtfViewer))]
    class RtfViewerViewModel:PopupViewModelBase<object>
    {
        FlowDocument _document;
        public FlowDocument Document
        {
            get => _document;
            set
            {
                _document = value;
                OnPropertyChanged();
            }
        }
    }
}
