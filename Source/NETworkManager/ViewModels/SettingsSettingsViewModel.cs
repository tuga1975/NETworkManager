﻿using MahApps.Metro.Controls.Dialogs;
using NETworkManager.Models.Settings;
using NETworkManager.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NETworkManager.ViewModels
{
    public class SettingsSettingsViewModel : ViewModelBase
    {
        #region Variables
        private IDialogCoordinator dialogCoordinator;

        private bool _isLoading = true;

        public Action CloseAction { get; set; }

        private string _locationSelectedPath;
        public string LocationSelectedPath
        {
            get { return _locationSelectedPath; }
            set
            {
                if (value == _locationSelectedPath)
                    return;

                _locationSelectedPath = value;
                OnPropertyChanged();
            }
        }

        private bool _movingFiles;
        public bool MovingFiles
        {
            get { return _movingFiles; }
            set
            {
                if (value == _movingFiles)
                    return;

                _movingFiles = value;
                OnPropertyChanged();
            }
        }
              
        private bool _isPortable;
        public bool IsPortable
        {
            get { return _isPortable; }
            set
            {
                if (value == _isPortable)
                    return;

                if (!_isLoading)
                    MakePortable(value);

                _isPortable = value;
                OnPropertyChanged();
            }
        }

        private bool _makingPortable;
        public bool MakingPortable
        {
            get { return _makingPortable; }
            set
            {
                if (value == _makingPortable)
                    return;

                _makingPortable = value;
                OnPropertyChanged();
            }
        }

        private bool _resetEverything;
        public bool ResetEverything
        {
            get { return _resetEverything; }
            set
            {
                if (value == _resetEverything)
                    return;

                _resetEverything = value;
                OnPropertyChanged();
            }
        }

        private bool _applicationSettingsExists;
        public bool ApplicationSettingsExists
        {
            get { return _applicationSettingsExists; }
            set
            {
                if (value == _applicationSettingsExists)
                    return;

                _applicationSettingsExists = value;
                OnPropertyChanged();
            }
        }

        private bool _resetApplicationSettings;
        public bool ResetApplicationSettings
        {
            get { return _resetApplicationSettings; }
            set
            {
                if (value == _resetApplicationSettings)
                    return;

                _resetApplicationSettings = value;
                OnPropertyChanged();
            }
        }

        private bool _networkInterfaceProfilesExists;
        public bool NetworkInterfaceProfilesExists
        {
            get { return _networkInterfaceProfilesExists; }
            set
            {
                if (value == _networkInterfaceProfilesExists)
                    return;

                _networkInterfaceProfilesExists = value;
                OnPropertyChanged();
            }
        }

        private bool _resetNetworkInterfaceProfiles;
        public bool ResetNetworkInterfaceProfiles
        {
            get { return _resetNetworkInterfaceProfiles; }
            set
            {
                if (value == _resetNetworkInterfaceProfiles)
                    return;

                _resetNetworkInterfaceProfiles = value;
                OnPropertyChanged();
            }
        }

        private bool _ipScannerProfilesExists;
        public bool IPScannerProfilesExists
        {
            get { return _ipScannerProfilesExists; }
            set
            {
                if (value == _ipScannerProfilesExists)
                    return;

                _ipScannerProfilesExists = value;
                OnPropertyChanged();
            }
        }

        private bool _resetIPScannerProfiles;
        public bool ResetIPScannerProfiles
        {
            get { return _resetIPScannerProfiles; }
            set
            {
                if (value == _resetIPScannerProfiles)
                    return;

                _resetIPScannerProfiles = value;
                OnPropertyChanged();
            }
        }
                
        private bool _portScannerProfilesExists;
        public bool PortScannerProfilesExists
        {
            get { return _portScannerProfilesExists; }
            set
            {
                if (value == _portScannerProfilesExists)
                    return;

                _portScannerProfilesExists = value;
                OnPropertyChanged();
            }
        }

        private bool _resetPortScannerProfiles;
        public bool ResetPortScannerProfiles
        {
            get { return _resetPortScannerProfiles; }
            set
            {
                if (value == _resetPortScannerProfiles)
                    return;

                _resetPortScannerProfiles = value;
                OnPropertyChanged();
            }
        }

        private bool _pingProfilesExists;
        public bool PingProfilesExists
        {
            get { return _pingProfilesExists; }
            set
            {
                if (value == _pingProfilesExists)
                    return;

                _pingProfilesExists = value;
                OnPropertyChanged();
            }
        }

        private bool _resetPingProfiles;
        public bool ResetPingProfiles
        {
            get { return _resetPingProfiles; }
            set
            {
                if (value == _resetPingProfiles)
                    return;

                _resetPingProfiles = value;
                OnPropertyChanged();
            }
        }

        private bool _tracerouteProfilesExists;
        public bool TracerouteProfilesExists
        {
            get { return _tracerouteProfilesExists; }
            set
            {
                if (value == _tracerouteProfilesExists)
                    return;

                _tracerouteProfilesExists = value;
                OnPropertyChanged();
            }
        }

        private bool _resetTracerouteProfiles;
        public bool ResetTracerouteProfiles
        {
            get { return _resetTracerouteProfiles; }
            set
            {
                if (value == _resetTracerouteProfiles)
                    return;

                _resetTracerouteProfiles = value;
                OnPropertyChanged();
            }
        }

        private bool _remoteDesktopProfilesExists;
        public bool RemoteDesktopProfilesExists
        {
            get { return _remoteDesktopProfilesExists; }
            set
            {
                if (value == _remoteDesktopProfilesExists)
                    return;

                _remoteDesktopProfilesExists = value;
                OnPropertyChanged();
            }
        }

        private bool _resetRemoteDesktopProfiles;
        public bool ResetRemoteDesktopProfiles
        {
            get { return _resetRemoteDesktopProfiles; }
            set
            {
                if (value == _resetRemoteDesktopProfiles)
                    return;

                _resetRemoteDesktopProfiles = value;
                OnPropertyChanged();
            }
        }

        private bool _puTTYProfilesExists;
        public bool PuTTYProfilesExists
        {
            get { return _puTTYProfilesExists; }
            set
            {
                if (value == _puTTYProfilesExists)
                    return;

                _puTTYProfilesExists = value;
                OnPropertyChanged();
            }
        }

        private bool _resetPuTTYProfiles;
        public bool ResetPuTTYProfiles
        {
            get { return _resetPuTTYProfiles; }
            set
            {
                if (value == _resetPuTTYProfiles)
                    return;

                _resetPuTTYProfiles = value;
                OnPropertyChanged();
            }
        }

        private bool _wakeOnLANClientsExists;
        public bool WakeOnLANClientsExists
        {
            get { return _wakeOnLANClientsExists; }
            set
            {
                if (value == _wakeOnLANClientsExists)
                    return;

                _wakeOnLANClientsExists = value;
                OnPropertyChanged();
            }
        }

        private bool _resetWakeOnLANClients;
        public bool ResetWakeOnLANClients
        {
            get { return _resetWakeOnLANClients; }
            set
            {
                if (value == _resetWakeOnLANClients)
                    return;

                _resetWakeOnLANClients = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor, LoadSettings
        public SettingsSettingsViewModel(IDialogCoordinator instance)
        {
            dialogCoordinator = instance;

            LoadSettings();

            _isLoading = false;
        }

        private void LoadSettings()
        {
            LocationSelectedPath = SettingsManager.GetSettingsLocationNotPortable();
            IsPortable = SettingsManager.GetIsPortable();
        }
        #endregion

        #region ICommands & Actions
        public ICommand BrowseFolderCommand
        {
            get { return new RelayCommand(p => BrowseFolderAction()); }
        }

        private void BrowseFolderAction()
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (Directory.Exists(LocationSelectedPath))
                dialog.SelectedPath = LocationSelectedPath;

            System.Windows.Forms.DialogResult dialogResult = dialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
                LocationSelectedPath = dialog.SelectedPath;
        }

        public ICommand OpenLocationCommand
        {
            get { return new RelayCommand(p => OpenLocationAction()); }
        }

        private void OpenLocationAction()
        {
            Process.Start("explorer.exe", SettingsManager.GetSettingsLocation());
        }

        public ICommand ChangeSettingsCommand
        {
            get { return new RelayCommand(p => ChangeSettingsAction()); }
        }

        // Check if a file(name) is a settings file
        private bool FilesContainsSettingsFiles(string[] files)
        {
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);

                if (SettingsManager.GetSettingsFileName() == fileName)
                    return true;

                if (NetworkInterfaceProfileManager.ProfilesFileName == fileName)
                    return true;

                if (PuTTYProfileManager.ProfilesFileName == fileName)
                    return true;
            }

            return false;
        }

        private async void ChangeSettingsAction()
        {
            MovingFiles = true;
            bool overwrite = false;
            bool forceRestart = false;

            string[] filesTargedLocation = Directory.GetFiles(LocationSelectedPath);

            // Check if there are any settings files in the folder...
            if (FilesContainsSettingsFiles(filesTargedLocation))
            {
                MetroDialogSettings settings = AppearanceManager.MetroDialog;

                settings.AffirmativeButtonText = LocalizationManager.GetStringByKey("String_Button_Overwrite");
                settings.NegativeButtonText = LocalizationManager.GetStringByKey("String_Button_Cancel");
                settings.FirstAuxiliaryButtonText = LocalizationManager.GetStringByKey("String_Button_MoveAndRestart");
                settings.DefaultButtonFocus = MessageDialogResult.FirstAuxiliary;

                MessageDialogResult result = await dialogCoordinator.ShowMessageAsync(this, LocalizationManager.GetStringByKey("String_Header_Overwrite"),LocalizationManager.GetStringByKey("String_OverwriteSettingsInTheDestinationFolder"), MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, AppearanceManager.MetroDialog);

                if (result == MessageDialogResult.Negative)
                {
                    MovingFiles = false;
                    return;
                }
                else if (result == MessageDialogResult.Affirmative)
                {
                    overwrite = true;
                }
                else if (result == MessageDialogResult.FirstAuxiliary)
                {
                    forceRestart = true;
                }
            }

            // Try moving files (permissions, file is in use...)
            try
            {
                await SettingsManager.MoveSettingsAsync(SettingsManager.GetSettingsLocation(), LocationSelectedPath, overwrite, filesTargedLocation);

                Properties.Settings.Default.Settings_CustomSettingsLocation = LocationSelectedPath;

                // Show the user some awesome animation to indicate we are working on it :)
                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                MetroDialogSettings settings = AppearanceManager.MetroDialog;

                settings.AffirmativeButtonText = LocalizationManager.GetStringByKey("String_Button_OK");

               await dialogCoordinator.ShowMessageAsync(this, LocalizationManager.GetStringByKey("String_Header_Error") as string, ex.Message, MessageDialogStyle.Affirmative, settings);
            }

            LocationSelectedPath = string.Empty;
            LocationSelectedPath = Properties.Settings.Default.Settings_CustomSettingsLocation;

            if (forceRestart)
            {
                SettingsManager.ForceRestart = true;
                CloseAction();
            }

            MovingFiles = false;
        }

        public ICommand RestoreDefaultSettingsLocationCommand
        {
            get { return new RelayCommand(p => RestoreDefaultSettingsLocationAction()); }
        }

        private void RestoreDefaultSettingsLocationAction()
        {
            LocationSelectedPath = SettingsManager.GetDefaultSettingsLocation();
        }

        public ICommand ResetSettingsCommand
        {
            get { return new RelayCommand(p => ResetSettingsAction()); }
        }

        public async void ResetSettingsAction()
        {
            MetroDialogSettings settings = AppearanceManager.MetroDialog;

            settings.AffirmativeButtonText = LocalizationManager.GetStringByKey("String_Button_Continue");
            settings.NegativeButtonText = LocalizationManager.GetStringByKey("String_Button_Cancel");

            settings.DefaultButtonFocus = MessageDialogResult.Affirmative;

            string message = LocalizationManager.GetStringByKey("String_SelectedSettingsAreReset");

            if (ResetEverything || ResetApplicationSettings)
            {
                message += Environment.NewLine + Environment.NewLine + string.Format("* {0}", LocalizationManager.GetStringByKey("String_TheSettingsLocationIsNotAffected"));
                message += Environment.NewLine + string.Format("* {0}", LocalizationManager.GetStringByKey("String_ApplicationIsRestartedAfterwards"));
            }

            if (await dialogCoordinator.ShowMessageAsync(this, LocalizationManager.GetStringByKey("String_Header_AreYouSure"), message, MessageDialogStyle.AffirmativeAndNegative, settings) != MessageDialogResult.Affirmative)
                return;

            bool forceRestart = false;

            if (ApplicationSettingsExists && (ResetEverything || ResetApplicationSettings))
            {
                SettingsManager.Reset();
                forceRestart = true;
            }

            if (NetworkInterfaceProfilesExists && (ResetEverything || ResetNetworkInterfaceProfiles))
                NetworkInterfaceProfileManager.Reset();

            if (PuTTYProfilesExists && (ResetEverything || ResetPuTTYProfiles))
                PuTTYProfileManager.Reset();

            // Restart after reset or show a completed message
            if (forceRestart)
            {
                CloseAction();
            }
            else
            {
                settings.AffirmativeButtonText = LocalizationManager.GetStringByKey("String_Button_OK");

                await dialogCoordinator.ShowMessageAsync(this, LocalizationManager.GetStringByKey("String_Header_Success"), LocalizationManager.GetStringByKey("String_SettingsSuccessfullyReset"), MessageDialogStyle.Affirmative, settings);
            }
        }
        #endregion

        #region Methods
        private async void MakePortable(bool isPortable)
        {
            MakingPortable = true;

            // Save settings before moving them
            if (SettingsManager.Current.SettingsChanged)
                SettingsManager.Save();

            // Try moving files (permissions, file is in use...)
            try
            {
                await SettingsManager.MakePortableAsync(isPortable, true);

                Properties.Settings.Default.Settings_CustomSettingsLocation = string.Empty;
                LocationSelectedPath = SettingsManager.GetSettingsLocationNotPortable();

                // Show the user some awesome animation to indicate we are working on it :)
                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(this, LocalizationManager.GetStringByKey("String_Header_Error"), ex.Message, MessageDialogStyle.Affirmative, AppearanceManager.MetroDialog);
            }

            MakingPortable = false;
        }

        public void SaveAndCheckSettings()
        {
            // Save everything
            if (SettingsManager.Current.SettingsChanged)
                SettingsManager.Save();

            if (ProfileManager.ProfilesChanged)
                ProfileManager.Save();

            if (NetworkInterfaceProfileManager.ProfilesChanged)
                NetworkInterfaceProfileManager.Save();
                        
            if (PuTTYProfileManager.ProfilesChanged)
                PuTTYProfileManager.Save();

            // Check if files exist
            ApplicationSettingsExists = File.Exists(SettingsManager.GetSettingsFilePath());
            NetworkInterfaceProfilesExists = File.Exists(NetworkInterfaceProfileManager.GetProfilesFilePath());
            PuTTYProfilesExists = File.Exists(PuTTYProfileManager.GetProfilesFilePath());
        }

        public void SetLocationPathFromDragDrop(string path)
        {
            LocationSelectedPath = path;

            OnPropertyChanged(nameof(LocationSelectedPath));
        }
        #endregion
    }
}