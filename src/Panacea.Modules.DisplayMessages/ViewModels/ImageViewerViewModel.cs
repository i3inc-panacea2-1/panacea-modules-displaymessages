using Panacea.Modularity.UiManager;
using Panacea.Modules.DisplayMessages.Views;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.DisplayMessages.ViewModels
{
    [View(typeof(ImageViewer))]
    class ImageViewerViewModel:PopupViewModelBase<object>
    {
        string _imageUrl;
        public string ImageUrl
        {
            get => _imageUrl;
            set
            {
                _imageUrl = value;
                OnPropertyChanged();
            }
        }
    }
}
