using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using TeamArg.GameLoader.Common.Model;
using TeamArg.GameLoader.Common.Extenders;
using TeamArg.GameLoader.Messages;
using TeamArg.GameLoader.Model;
using TeamArg.GameLoader.Wpf;
using static System.Console;

namespace TeamArg.GameLoader.ViewModel
{
    public class MainViewModel : ChangeNotifyingViewModel
    {
        [RaisePropertyChanged]
        public ObservableCollection<HexArchive> AvailableHexArchives { get; set; }

        [RaisePropertyChanged]
        public ObservableCollection<LogEntry> LogItems { get; set; }

        [RaisePropertyChanged]
        public HexArchive SelectedHexArchive { get; set; }

        [RaisePropertyChanged]
        public bool IsBusy { get; set; }

        [RaisePropertyChanged]
        public LogEntry SelectedLogItem { get; set; }

        public RelayCommand FlashHexCommand { get; set; }

        public RelayCommand FlashSelectedHexArchiveCommand { get; set; }

        public RelayCommand ImportHexArchiveCommand { get; set; }
                
        public MainViewModel()
        {
            InitializeRelayCommands();
            ListAvailableHexArchives();
        }

        private void InitializeRelayCommands()
        {
            FlashHexCommand = new RelayCommand(FlashHexCommand_Execute, FlashHexCommand_CanExecute);
            FlashSelectedHexArchiveCommand = new RelayCommand(FlashSelectedHexArchiveCommand_ExecuteAsync, FlashSelectedHexArchiveCommand_CanExecute);
            ImportHexArchiveCommand = new RelayCommand(ImportHexArchiveCommand_Execute, () => !IsBusy);
        }

        private void ListAvailableHexArchives()
        {
            AvailableHexArchives = Directory.GetFiles(App.HexArchiveDirectory, "*.arduboy")
                                            .ToList()
                                            .Select(f => HexArchive.FromFile(f))
                                            .Where(f => f.HasValue)
                                            .Select(f => f.Value)
                                            .OrderBy(h => h.Filename)
                                            .ToObservableCollection();       

            SelectedHexArchive = AvailableHexArchives.Count > 0 ? AvailableHexArchives[0] : null;
        }

        [LogException]
        private async Task FlashHexFileAsync(string filename)
        {
            Console.WriteLine($"Preparing to flash {filename}");

            var progress = new Progress<string>();

            try
            {
                LogItems = new ObservableCollection<LogEntry>();
                IsBusy = true;
                var arduboy = new Arduboy();

                progress.ProgressChanged += Progress_ProgressChanged;

                await arduboy.FlashHexFileAsync(filename, progress);
                
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger()
                          .Error($"Failed to flash hex '{filename}': {ex.Message}");                          
                                        
                AddLogEntry(ex.Message, true);
                Console.WriteLine(ex);
            }
            finally
            {
                await Task.Delay(3000);
                IsBusy = false;
                FlashHexCommand.RaiseCanExecuteChanged();
                progress.ProgressChanged -= Progress_ProgressChanged;
            }
        }

        private void Progress_ProgressChanged(object sender, string e)
        {
            AddLogEntry(e);            
        }

        [LogException]    
        private string GetTempFilePath()
        {
            return Path.Combine(App.TempDirectory, $"{Path.GetFileNameWithoutExtension(Path.GetTempFileName())}.hex");
        }

        #region "CommandHandlers#

        private bool FlashSelectedHexArchiveCommand_CanExecute()
        {
            return !IsBusy && SelectedHexArchive != null;
        }

        [LogException]
        private async void FlashSelectedHexArchiveCommand_ExecuteAsync()
        {
            var binaryCount = SelectedHexArchive.Info.Binaries.Count;
            var hexFile = GetTempFilePath();    
            var archivedHexFile = String.Empty;

            switch (binaryCount)
            {
                case 0:
                    ///TODO: Show an indication to the user, there are no binaries
                    return;                    
                case 1:
                    archivedHexFile = SelectedHexArchive.Info.Binaries.First().Filename;
                    break;
                default:
                    var selectedBinary = await SelectBinaryDialogMessage.SendAsync(SelectedHexArchive.Info.Binaries);
                    archivedHexFile = selectedBinary?.Filename ?? String.Empty;                    
                    break;
            }

            if (archivedHexFile != String.Empty)
            {
                var data = SelectedHexArchive.ExtactHexData(archivedHexFile);
                File.WriteAllBytes(hexFile, data);
                await FlashHexFileAsync(hexFile);
            }
            else
            {
                AddLogEntry("No binary selected", true);
            }
        }

        private bool FlashHexCommand_CanExecute()
        {
            return !IsBusy;
        }

        [LogException]
        private void FlashHexCommand_Execute()
        {
            var message = new OpenFileDialogMessage("Open a HEX file", false, ".hex", "Hex Files|*.hex", async (filenames) =>
            {
                var targetFile = Path.Combine(App.TempDirectory, Path.GetFileName(filenames[0]));
                if (!TryToDeleteFile(targetFile))
                {
                    AddLogEntry("Failed to delete existing file", true);
                }
                File.Copy(filenames[0], targetFile);

                await FlashHexFileAsync(targetFile);
            });
            MessengerInstance.Send(message);
        }

        [LogException]
        private void ImportHexArchiveCommand_Execute()
        {
            var message = new OpenFileDialogMessage("Import Ardobuy Files", true, ".arduboy", "Ardobuy Files|*.arduboy", async (filenames) =>
            {
                
                LogItems = new ObservableCollection<LogEntry>();
                await Task.Run(async () =>
                {
                    IsBusy = true;
                    foreach (var filePath in filenames)
                    {
                        var filename = Path.GetFileName(filePath);
                        AddLogEntry($"Checking {filename}");
                        var targetFilename = Path.Combine(App.HexArchiveDirectory, filename);

                        if (File.Exists(targetFilename))
                        {
                            AddLogEntry($"{filename} already exists, trying to replace");
                            if (!TryToDeleteFile(targetFilename))
                            {
                                AddLogEntry($"{filename} already exists, unable to delete");
                                continue;
                            }                            
                        }

                        var archive = HexArchive.FromFile(filePath);
                        if (archive.HasValue)
                        {
                            AddLogEntry($"Found {archive.Value.Info.Title}. Importing...");
                            File.Copy(filePath, targetFilename);
                        }
                        else
                        {
                            AddLogEntry($"{filename} is not a valid Arduboy File");
                        }                                          
                    }

                    AddLogEntry($"Reloading files");
                    ListAvailableHexArchives();
                    AddLogEntry($"Import complete");
                    await Task.Delay(5000);
                    IsBusy = false;
                });
            });
            MessengerInstance.Send(message);
        }

        #endregion

        private void AddLogEntry(string message, bool isError = false)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                LogItems.Add(new LogEntry(message, isError));
                SelectedLogItem = LogItems.Last();
            });
        }

        private bool TryToDeleteFile(string filename)
        {
            try
            {
                File.Delete(filename);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}