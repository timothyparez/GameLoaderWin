using GalaSoft.MvvmLight.Threading;
using System.IO;
using System.Reflection;
using System.Windows;

namespace TeamArg.GameLoader.Wpf
{
    public partial class App : Application
    {
        public App()
        {
            DispatcherHelper.Initialize();

            if (!Directory.Exists(TempDirectory))
            {
                Directory.CreateDirectory(TempDirectory);
            }

            if (!Directory.Exists(HexArchiveDirectory))
            {
                Directory.CreateDirectory(HexArchiveDirectory);
            }
        }

        public static string WorkingDirectory
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }

        public static string TempDirectory
        {
            get
            {
                return Path.Combine(WorkingDirectory, "tmp");
            }
        }

        public static string HexArchiveDirectory
        {
            get
            {
                return Path.Combine(WorkingDirectory, "archives");
            }
        }
    }
}
