﻿using NETworkManager.Models.Settings;
using System.Net;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System;
using NETworkManager.Models.Network;
using System.ComponentModel;
using System.Windows.Data;
using NETworkManager.Views;
using NETworkManager.Utilities;
using System.Threading.Tasks;
using System.Diagnostics;

namespace NETworkManager.ViewModels
{
    public class WakeOnLANViewModel : ViewModelBase
    {
        #region  Variables 
        private IDialogCoordinator dialogCoordinator;

        private bool _isLoading = true;

        private bool _isSending;
        public bool IsSending
        {
            get { return _isSending; }
            set
            {
                if (value == _isSending)
                    return;

                _isSending = value;
                OnPropertyChanged();
            }
        }

        private string _MACAddress;
        public string MACAddress
        {
            get { return _MACAddress; }
            set
            {
                if (value == _MACAddress)
                    return;

                _MACAddress = value;
                OnPropertyChanged();
            }
        }

        private bool _macAddressHasError;
        public bool MACAddressHasError
        {
            get { return _macAddressHasError; }
            set
            {
                if (value == _macAddressHasError)
                    return;

                _macAddressHasError = value;
                OnPropertyChanged();
            }
        }

        private string _broadcast;
        public string Broadcast
        {
            get { return _broadcast; }
            set
            {
                if (value == _broadcast)
                    return;

                _broadcast = value;
                OnPropertyChanged();
            }
        }

        private bool _broadcastHasError;
        public bool BroadcastHasError
        {
            get { return _broadcastHasError; }
            set
            {
                if (value == _broadcastHasError)
                    return;

                _broadcastHasError = value;
                OnPropertyChanged();
            }
        }

        private int _port;
        public int Port
        {
            get { return _port; }
            set
            {
                if (value == _port)
                    return;

                _port = value;
                OnPropertyChanged();
            }
        }

        private bool _portHasError;
        public bool PortHasError
        {
            get { return _portHasError; }
            set
            {
                if (value == _portHasError)
                    return;

                _portHasError = value;
                OnPropertyChanged();
            }
        }

        private bool _displayStatusMessage;
        public bool DisplayStatusMessage
        {
            get { return _displayStatusMessage; }
            set
            {
                if (value == _displayStatusMessage)
                    return;

                _displayStatusMessage = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if (value == _statusMessage)
                    return;

                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        #region Clients
        ICollectionView _wakeOnLANClients;
        public ICollectionView WakeOnLANClients
        {
            get { return _wakeOnLANClients; }
        }

        private WakeOnLANClientInfo _selectedClient;
        public WakeOnLANClientInfo SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                if (value == _selectedClient)
                    return;

                if (value != null && !IsSending)
                {
                    MACAddress = value.MACAddress;
                    Broadcast = value.Broadcast;
                    Port = value.Port;
                }

                _selectedClient = value;
                OnPropertyChanged();
            }
        }

        private string _search;
        public string Search
        {
            get { return _search; }
            set
            {
                if (value == _search)
                    return;

                _search = value;

                WakeOnLANClients.Refresh();

                OnPropertyChanged();
            }
        }

        private bool _canProfileWidthChange = true;
        private double _tempClientWidth;

        private bool _expandClientView;
        public bool ExpandClientView
        {
            get { return _expandClientView; }
            set
            {
                if (value == _expandClientView)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.WakeOnLAN_ExpandClientView = value;

                _expandClientView = value;

                if (_canProfileWidthChange)
                    ResizeClient(dueToChangedSize: false);

                OnPropertyChanged();
            }
        }

        private GridLength _clientWidth;
        public GridLength ClientWidth
        {
            get { return _clientWidth; }
            set
            {
                if (value == _clientWidth)
                    return;

                if (!_isLoading && value.Value != 40) // Do not save the size when collapsed
                    SettingsManager.Current.WakeOnLAN_ClientWidth = value.Value;

                _clientWidth = value;

                if (_canProfileWidthChange)
                    ResizeClient(dueToChangedSize: true);

                OnPropertyChanged();
            }
        }
        #endregion
        #endregion

        #region Constructor, load settings
        public WakeOnLANViewModel(IDialogCoordinator instance)
        {
            dialogCoordinator = instance;

            if (WakeOnLANClientManager.Clients == null)
                WakeOnLANClientManager.Load();

            _wakeOnLANClients = CollectionViewSource.GetDefaultView(WakeOnLANClientManager.Clients);
            _wakeOnLANClients.GroupDescriptions.Add(new PropertyGroupDescription(nameof(WakeOnLANClientInfo.Group)));
            _wakeOnLANClients.SortDescriptions.Add(new SortDescription(nameof(WakeOnLANClientInfo.Group), ListSortDirection.Ascending));
            _wakeOnLANClients.SortDescriptions.Add(new SortDescription(nameof(WakeOnLANClientInfo.Name), ListSortDirection.Ascending));
            _wakeOnLANClients.Filter = o =>
            {
                if (string.IsNullOrEmpty(Search))
                    return true;

                WakeOnLANClientInfo info = o as WakeOnLANClientInfo;

                string search = Search.Trim();

                // Search by: Name
                return info.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) > -1;
            };

            LoadSettings();

            _isLoading = false;
        }

        private void LoadSettings()
        {
            Port = SettingsManager.Current.WakeOnLAN_DefaultPort;
            ExpandClientView = SettingsManager.Current.WakeOnLAN_ExpandClientView;

            if (ExpandClientView)
                ClientWidth = new GridLength(SettingsManager.Current.WakeOnLAN_ClientWidth);
            else
                ClientWidth = new GridLength(40);

            _tempClientWidth = SettingsManager.Current.WakeOnLAN_ClientWidth;
        }
        #endregion

        #region ICommands & Actions
        public ICommand WakeUpCommand
        {
            get { return new RelayCommand(p => WakeUpAction(), WakeUpAction_CanExecute); }
        }

        private bool WakeUpAction_CanExecute(object parameter)
        {
            return !MACAddressHasError && !BroadcastHasError && !PortHasError;
        }

        private void WakeUpAction()
        {
            WakeOnLANInfo info = new WakeOnLANInfo
            {
                MagicPacket = WakeOnLAN.CreateMagicPacket(MACAddress),
                Broadcast = IPAddress.Parse(Broadcast),
                Port = Port
            };

            WakeUp(info);
        }

        public ICommand WakeUpClientCommand
        {
            get { return new RelayCommand(p => WakeUpClientAction()); }
        }

        private void WakeUpClientAction()
        {
            WakeOnLANInfo info = new WakeOnLANInfo
            {
                MagicPacket = WakeOnLAN.CreateMagicPacket(SelectedClient.MACAddress),
                Broadcast = IPAddress.Parse(SelectedClient.Broadcast),
                Port = SelectedClient.Port
            };

            WakeUp(info);
        }

        public ICommand AddClientCommand
        {
            get { return new RelayCommand(p => AddClientAction()); }
        }

        private async void AddClientAction()
        {
            CustomDialog customDialog = new CustomDialog()
            {
                Title = LocalizationManager.GetStringByKey("String_Header_AddClient")
            };

            WakeOnLANClientViewModel wakeOnLANClientViewModel = new WakeOnLANClientViewModel(instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);

                WakeOnLANClientInfo wakeOnLANClientInfo = new WakeOnLANClientInfo
                {
                    Name = instance.Name,
                    MACAddress = instance.MACAddress,
                    Broadcast = instance.Broadcast,
                    Port = instance.Port,
                    Group = instance.Group
                };

                WakeOnLANClientManager.AddClient(wakeOnLANClientInfo);
            }, instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
            }, WakeOnLANClientManager.GetClientGroups(), new WakeOnLANClientInfo() { MACAddress = MACAddress, Broadcast = Broadcast, Port = Port });

            customDialog.Content = new WakeOnLANClientDialog
            {
                DataContext = wakeOnLANClientViewModel
            };

            await dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        public ICommand EditClientCommand
        {
            get { return new RelayCommand(p => EditClientAction()); }
        }

        private async void EditClientAction()
        {
            CustomDialog customDialog = new CustomDialog()
            {
                Title = LocalizationManager.GetStringByKey("String_Header_EditClient")
            };

            WakeOnLANClientViewModel wakeOnLANClientViewModel = new WakeOnLANClientViewModel(instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);

                WakeOnLANClientManager.RemoveClient(SelectedClient);

                WakeOnLANClientInfo wakeOnLANClientInfo = new WakeOnLANClientInfo
                {
                    Name = instance.Name,
                    MACAddress = instance.MACAddress,
                    Broadcast = instance.Broadcast,
                    Port = instance.Port,
                    Group = instance.Group
                };

                WakeOnLANClientManager.AddClient(wakeOnLANClientInfo);
            }, instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
            }, WakeOnLANClientManager.GetClientGroups(), SelectedClient);

            customDialog.Content = new WakeOnLANClientDialog
            {
                DataContext = wakeOnLANClientViewModel
            };

            await dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        public ICommand CopyAsClientCommand
        {
            get { return new RelayCommand(p => CopyAsProfileAction()); }
        }

        private async void CopyAsProfileAction()
        {
            CustomDialog customDialog = new CustomDialog()
            {
                Title = LocalizationManager.GetStringByKey("String_Header_CopyClient")
            };

            WakeOnLANClientViewModel wakeOnLANClientViewModel = new WakeOnLANClientViewModel(instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);

                WakeOnLANClientInfo wakeOnLANClientInfo = new WakeOnLANClientInfo
                {
                    Name = instance.Name,
                    MACAddress = instance.MACAddress,
                    Broadcast = instance.Broadcast,
                    Port = instance.Port,
                    Group = instance.Group
                };

                WakeOnLANClientManager.AddClient(wakeOnLANClientInfo);
            }, instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
            }, WakeOnLANClientManager.GetClientGroups(), SelectedClient);

            customDialog.Content = new WakeOnLANClientDialog
            {
                DataContext = wakeOnLANClientViewModel
            };

            await dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        public ICommand DeleteClientCommand
        {
            get { return new RelayCommand(p => DeleteClientAction()); }
        }

        private async void DeleteClientAction()
        {
            CustomDialog customDialog = new CustomDialog()
            {
                Title = LocalizationManager.GetStringByKey("String_Header_DeleteClient")
            };

            ConfirmRemoveViewModel confirmRemoveViewModel = new ConfirmRemoveViewModel(instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);

                WakeOnLANClientManager.RemoveClient(SelectedClient);
            }, instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
            }, LocalizationManager.GetStringByKey("String_DeleteClientMessage"));

            customDialog.Content = new ConfirmRemoveDialog
            {
                DataContext = confirmRemoveViewModel
            };

            await dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        public ICommand EditGroupCommand
        {
            get { return new RelayCommand(p => EditGroupAction(p)); }
        }

        private async void EditGroupAction(object group)
        {
            CustomDialog customDialog = new CustomDialog()
            {
                Title = LocalizationManager.GetStringByKey("String_Header_EditGroup")
            };

            GroupViewModel editGroupViewModel = new GroupViewModel(instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);

                WakeOnLANClientManager.RenameGroup(instance.OldGroup, instance.Group);

                _wakeOnLANClients.Refresh();
            }, instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
            }, group.ToString());

            customDialog.Content = new GroupDialog
            {
                DataContext = editGroupViewModel
            };

            await dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        public ICommand ClearSearchCommand
        {
            get { return new RelayCommand(p => ClearSearchAction()); }
        }

        private void ClearSearchAction()
        {
            Search = string.Empty;
        }
        #endregion

        #region Methods
        private async void WakeUp(WakeOnLANInfo info)
        {
            DisplayStatusMessage = false;
            IsSending = true;

            try
            {
                WakeOnLAN.Send(info);

                await Task.Delay(2000); // Make the user happy, let him see a reload animation (and he cannot spam the send command)

                StatusMessage = LocalizationManager.GetStringByKey("String_MagicPacketSuccessfulSended");
                DisplayStatusMessage = true;
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                DisplayStatusMessage = true;
            }
            finally
            {
                IsSending = false;
            }
        }

        private void ResizeClient(bool dueToChangedSize)
        {
            _canProfileWidthChange = false;

            if (dueToChangedSize)
            {
                if (ClientWidth.Value == 40)
                    ExpandClientView = false;
                else
                    ExpandClientView = true;
            }
            else
            {
                if (ExpandClientView)
                {
                    if (_tempClientWidth == 40)
                        ClientWidth = new GridLength(250);
                    else
                        ClientWidth = new GridLength(_tempClientWidth);
                }
                else
                {
                    _tempClientWidth = ClientWidth.Value;
                    ClientWidth = new GridLength(40);
                }
            }

            _canProfileWidthChange = true;
        }
        #endregion
    }
}
