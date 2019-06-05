using Panacea.Controls;
using Panacea.Modularity.UiManager;
using Panacea.Modules.DisplayMessages.Models;
using Panacea.Modules.DisplayMessages.Views;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.DisplayMessages.ViewModels
{
    [View(typeof(DisplayMessageSimpleNotification))]
    class DisplayMessageNotificationViewModel:PopupViewModelBase<object>
    {
        public DisplayMessageNotificationViewModel()
        {
            CloseCommand = new RelayCommand(args =>
            {
                if (Closable)
                    SetResult(null);
            });
        }

        DisplayMessageData _data;
        public DisplayMessageData Data
        {
            get => _data;
            set
            {
                _data = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand CloseCommand { get; }
    }
}
