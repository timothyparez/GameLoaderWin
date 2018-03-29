using NLog;
using Polly;
using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Reactive.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeamArg.GameLoader.AvrDude;
using TeamArg.GameLoader.Wpf;

namespace TeamArg.GameLoader.Model
{
    public class Arduboy
    {
        private const int ARDUBOY_VID = 0x2341;
        private const int ARDUBOY_PID = 0x8036;
        private const int BOOTLOADER_VID = 0x2341;
        private const int BOOTLOADER_PID = 0x0036;
        private const string AVRDUDE_ERROR_TAG = "ERROR";
        private const string AVRDUDE_FAILED_TAG = "failed";

        private const string BASE_REGEX = ".*VID.*{0:X4}.*PID.*{1:X4}";

        private static Regex arduboyRegex = new Regex(String.Format(BASE_REGEX, ARDUBOY_VID, ARDUBOY_PID), RegexOptions.IgnoreCase);
        private static Regex bootloaderRegex = new Regex(String.Format(BASE_REGEX, BOOTLOADER_VID, BOOTLOADER_PID), RegexOptions.IgnoreCase);

        private IProgress<string> progress;
        private Logger logger = LogManager.GetCurrentClassLogger();

        public Arduboy()
        { 
                    
        }

        public async Task FlashHexFileAsync(string filename, IProgress<string> progress)
        {
            this.progress = progress;            
            var targetFilename = Path.Combine(App.TempDirectory, $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.hex");

            ///TODO: Create temp files for flashing in case existing files are still locked
            if (filename != targetFilename && !File.Exists(targetFilename))
            {
                LogManager.GetCurrentClassLogger().Info($"Copying {filename} to {targetFilename}");
                File.Copy(filename, targetFilename);
            }
            
            await RebootToBootloaderAsync();

            var searchPolicy = Policy.Handle<Exception>()
                                     .WaitAndRetryAsync(new[]
                                        {
                                            TimeSpan.FromMilliseconds(150),
                                            TimeSpan.FromMilliseconds(200),
                                            TimeSpan.FromMilliseconds(500),
                                            TimeSpan.FromMilliseconds(1000)
                                        });

            progress.Report("Searching for the Arduboy bootloader...");
            var searchResult = await searchPolicy.ExecuteAndCaptureAsync<string>(async () => await GetArduinoPortNameAsync(ArduinoMode.Bootloader));            
            if (searchResult.Outcome == OutcomeType.Successful)
            {
                progress.Report($"Arduboy bootloader found on port {searchResult.Result}");
                var avrInvoker = new AvrDudeInvoker(Path.Combine("tmp", Path.GetFileName(targetFilename)), searchResult.Result);

                //avrInvoker.OnErrorReceived += (s, e) => logger.Error(e.Value);
                //avrInvoker.OnOutputReceived += (s, e) => logger.Info(e.Value);

                /*The avrdude application sends both normal output and error output to StdOut instead
                 * of sending errors to StdStandard. Therefore we must filter the output, which is what the observables are for */
                Exception executionError = null;
                var logAndThrow = new Action<string>(s =>
                {
                    logger.Error(s);
                    executionError = new Exception(s);
                });

                var output = Observable.FromEventPattern<StringEventArgs>(avrInvoker, "OnOutputReceived")
                                       .Select(o => o.EventArgs.Value);

                var errors = Observable.FromEventPattern<StringEventArgs>(avrInvoker, "OnErrorReceived")
                                       .Select(o => o.EventArgs.Value);

                var info = output.Where(v => !v.Contains(AVRDUDE_ERROR_TAG));
                var standardOutputErrors = output.Where(v => v.Contains(AVRDUDE_ERROR_TAG));

                info.Subscribe(v => logger.Info(v));                
                errors.Subscribe(logAndThrow);
                standardOutputErrors.Subscribe(logAndThrow);
                
                await avrInvoker.InvokeAsync(progress);
                if (executionError != null)
                {
                    throw executionError;
                }
            }
            else
            {
                //Show the user it failed
                Console.WriteLine("That didn't work");
            }
        }

        private  async Task RebootToBootloaderAsync()
        {
            progress.Report("Searching for the Arduboy...");         
            var portName = await GetArduinoPortNameAsync(ArduinoMode.Normal);
            progress.Report($"Arduboy found on port {portName}");

            progress.Report("Reboot the Arduboy into bootloader mode...");
            using (var port = new SerialPort(portName, 1200) { DtrEnable = true })
            {
                port.Open();
                await Task.Delay(500);
                port.Close();
                await Task.Delay(1000);
            }
        }

        private async Task<string> GetArduinoPortNameAsync(ArduinoMode mode)
        {
            var tcs = new TaskCompletionSource<string>();

            await Task.Run(() =>
            {

                var query = $"SELECT Name, DeviceID, PNPDeviceID FROM Win32_SerialPort WHERE (PNPDeviceID LIKE '%VID_2341%PID_8036%') OR (PNPDeviceID LIKE '%VID_2341%PID_0036%')";                
                using (var searcher = new ManagementObjectSearcher(query))
                {
                var serialPortObjects = searcher.Get()
                                               .Cast<ManagementBaseObject>()
                                               .Select(o => new
                                               {
                                                   DeviceId = o.GetPropertyValue("DeviceID").ToString(),
                                                   PNPDeviceId = o.GetPropertyValue("PNPDeviceID").ToString()
                                               });

                var currentRegex = (mode == ArduinoMode.Normal ? arduboyRegex : bootloaderRegex);
                var serialPort = serialPortObjects.FirstOrDefault(o => currentRegex.Match(o.PNPDeviceId).Success);

                if (serialPort != null)
                {
                    tcs.SetResult(serialPort.DeviceId);
                }
                else
                {
                    tcs.SetException(new Exception("Arduino not found"));
                    throw new Exception("Arduino not found!!!");
                }
            }
            });

            return await tcs.Task;            
        }
    }
}
