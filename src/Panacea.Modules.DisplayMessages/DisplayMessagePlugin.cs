using Panacea.Core;
using Panacea.Modularity;
using Panacea.Modularity.Billing;
using Panacea.Modularity.UiManager;
using Panacea.Modules.DisplayMessages.Models;
using Panacea.Modules.DisplayMessages.ViewModels;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Documents;

namespace Panacea.Modules.DisplayMessages
{

    public class DisplayMessagePlugin : IPlugin
    {
        private readonly PanaceaServices _core;

        public DisplayMessagePlugin(PanaceaServices core)
        {
            _core = core;
        }

        public Task BeginInit()
        {
            return Task.CompletedTask;
        }

        public Task EndInit()
        {
            Action<DisplayMessageModel> action = (obj) =>
            {
                if (_core.TryGetUiManager(out IUiManager ui))
                {
                    //ui.Unlock();
                }

                try
                {
                    if(_core.TryGetBilling(out IBillingManager billing))
                    {
                        var userServices = billing.GetActiveUserServices()
                                                    .Select(p => p.Plugin)
                                                    .Distinct()
                                                    .ToList();
                        var serv = obj.Data.Services;
                        if (serv.Count > 0 && serv.All(billing.IsPluginFree)) return;

                        if (serv.Count >= 0 && _core.UserService.User.Id != null && serv.Any(s => userServices?.Contains(s) == true))
                            return;
                    }
                }
                catch
                {
                }

                Application.Current.Dispatcher.Invoke(async () =>
                {
                    switch (obj.Type)
                    {
                        case "notify":
                            var content = new DisplayMessageNotificationViewModel()
                            {
                                Data = obj.Data,
                                Closable = true
                            };

                            ShowNotification(obj.Data, content);
                            break;
                        case "rtf":
                            var data = await _core.HttpClient.DownloadDataAsync(obj.Data.FileUrl);
                            try
                            {
                                var viewer = new RtfViewerViewModel()
                                {
                                    Document = new System.Windows.Documents.FlowDocument()
                                };
                                using (var fStream = new MemoryStream(data))
                                {
                                    var range =
                                        new TextRange(viewer.Document.ContentStart,
                                            viewer.Document.ContentEnd);
                                    range.Load(fStream, DataFormats.Rtf);
                                    fStream.Close();
                                }
                                ShowMessage(obj.Data, viewer);
                            }
                            catch
                            {
                            }
                            break;
                        case "image":
                            var image = new ImageViewerViewModel()
                            {
                                ImageUrl = obj.Data.FileUrl
                            };
                            ShowMessage(obj.Data, image);
                            break;
                    }
                });

            };

            _core.WebSocket.On<DisplayMessageModel>("alert", (dyn) => action(dyn));
            return Task.CompletedTask;

        }

        void ShowMessage(DisplayMessageData obj, PopupViewModelBase<object> content)
        {
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                if (obj.StartInFullScreen)
                {
                    ui.Navigate(content);
                    if (!obj.AllowClose) ui.EnableFullscreen();

                    if (obj.AllowCloseAfter > 0)
                    {
                        var t = new Timer
                        {
                            Interval = obj.AllowCloseAfter * 1000
                        };
                        t.Elapsed += (ooo, eee) =>
                        {
                            t.Stop();
                            t.Enabled = false;
                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                ui.DisableFullscreen();
                            }));

                        };
                        t.Start();
                    }
                    if (obj.MaxDisplayTime != 0)
                    {
                        var t = new Timer();
                        t.Interval = obj.MaxDisplayTime * 1000;
                        t.Elapsed += (ooo, eee) =>
                        {
                            t.Stop();
                            t.Enabled = false;
                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                if (ui.CurrentPage == content)
                                {
                                    if (!obj.AllowClose && obj.AllowCloseAfter == 0) ui.DisableFullscreen();
                                    ui.GoBack();
                                }
                            }));

                        };
                        t.Start();
                    }
                }
                else
                {
                    var shownPopup = ui.ShowPopup(content, null,
                            PopupType.Information, obj.AllowClose && obj.AllowCloseAfter == 0);
                    if (obj.AllowCloseAfter > 0)
                    {
                        var t = new Timer { Interval = obj.AllowCloseAfter * 1000 };
                        t.Elapsed += (ooo, eee) =>
                        {
                            t.Stop();
                            t.Enabled = false;
                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                content.Closable = true;
                            }));
                        };
                        t.Start();
                    }
                    if (obj.MaxDisplayTime != 0)
                    {
                        var t = new Timer
                        {
                            Interval = obj.MaxDisplayTime * 1000
                        };
                        t.Elapsed += (ooo, eee) =>
                        {
                            t.Stop();
                            t.Enabled = false;
                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                ui.HidePopup(content);
                            }));
                        };
                        t.Start();
                    }

                }
            }

        }

        async void ShowNotification(DisplayMessageData obj, DisplayMessageNotificationViewModel content)
        {
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                ui.Notify(content);
                if (obj.AllowCloseAfter > 0)
                {
                    content.Closable = false;
                    var t = new Timer { Interval = obj.AllowCloseAfter * 1000 };
                    t.Elapsed += (ooo, eee) =>
                    {
                        t.Stop();
                        t.Enabled = false;
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            content.Closable = true;
                        }));
                    };
                    t.Start();
                }
                if (obj.MaxDisplayTime != 0)
                {
                    var t = new Timer();
                    t.Interval = obj.MaxDisplayTime * 1000;
                    t.Elapsed += (ooo, eee) =>
                    {
                        t.Stop();
                        t.Enabled = false;
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            content.Close() ;
                        }));
                    };
                    t.Start();
                }
                await content.GetTask();
                ui.Refrain(content);
            }
        }

        public Task Shutdown()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

    }

}
