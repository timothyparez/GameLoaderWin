using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TeamArg.GameLoader.AvrDude
{
    public class StringEventArgs : EventArgs
    {
        public string Value { get; set; }
        public StringEventArgs(string value)
        {
            Value = value;
        }
    }

    public class AvrDudeInvoker
    {
        public delegate void StringEventHandler(object sender, StringEventArgs e);

        public event StringEventHandler OnOutputReceived;
        public event StringEventHandler OnErrorReceived;
        
        private CountdownEvent countdownEvent;


        [AvrDudeParameter('C', "avrdude.conf")]
        public string ConfigFile { get; set; }

        [AvrDudeParameter('p', "atmega32u4")]
        public string AvrDevice { get; set; }

        [AvrDudeParameter('c', "avr109")]
        public string Programmer { get; set; }

        [AvrDudeParameter('P')]
        public string SerialPort { get; set; }

        [AvrDudeParameter('b', "57600")]
        public string Baudrate { get; set; }

        [AvrDudeParameter('D')]
        public string DisableAutoEraseForFlash { get; }

        [AvrDudeParameter('U')]
        public MemoryOperationSpecification MemoryOperation { get; set; }
            

        public AvrDudeInvoker(string hexFile, string serialPort)
        {
            SerialPort = serialPort;
            MemoryOperation = new MemoryOperationSpecification(hexFile);
        }

        public async Task InvokeAsync(IProgress<string> progress)
        {
            await Task.Run(() => Invoke(progress));
        }

        public void Invoke(IProgress<string> progress)
        {
            progress.Report("Flashing to your Arduboy...");
            var startInfo = new ProcessStartInfo("avrdude.exe", this.ToString())
            {

                CreateNoWindow = true,

                 RedirectStandardError = true,
                 RedirectStandardOutput = true,
                 UseShellExecute = false,   
                 WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            };

            Console.WriteLine(startInfo.WorkingDirectory);
            countdownEvent = new CountdownEvent(2);

            using (var process = new Process() { StartInfo = startInfo })
            {
                process.EnableRaisingEvents = true;                
                process.OutputDataReceived += Process_OutputDataReceived;
                process.ErrorDataReceived += Process_ErrorDataReceived;
                
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                
                process.OutputDataReceived -= Process_OutputDataReceived;
                process.ErrorDataReceived -= Process_ErrorDataReceived;
                process.Close();                
            }

            progress.Report("Flash process completed");
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Console.WriteLine(e.Data);
                OnOutputReceived?.Invoke(this, new StringEventArgs(e.Data));                
            }
            else
            {
                countdownEvent.AddCount();
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Console.WriteLine(e.Data);
                OnErrorReceived?.Invoke(this, new StringEventArgs(e.Data));
            }
            else
            {
                countdownEvent.AddCount();
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            
            var properties = typeof(AvrDudeInvoker).GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var property in properties)
            {                
                var attribute = property.GetCustomAttributes<AvrDudeParameterAttribute>(false).FirstOrDefault();
                if (attribute != null)
                {
                    builder.Append($" -{attribute.Option} {this.GetType().GetProperty(property.Name).GetValue(this) ?? attribute.DefaultValue}");
                }
            }

            return builder.ToString();
        }
    }
}
