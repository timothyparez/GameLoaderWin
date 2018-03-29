using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TeamArg.GameLoader.Common.Model;

namespace TeamArg.GameLoader.Wpf
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //var binaries = new List<BinaryInfo>();
            //binaries.Add(new BinaryInfo() { Device = "Arduboy", Filename = "arduboy1.hex", Title = "Chapter 1 - It was the best of times" });
            //binaries.Add(new BinaryInfo() { Device = "Devkit", Filename = "devkit1.hex", Title = "Chapter 1 - It was the best of times" });
            //binaries.Add(new BinaryInfo() { Device = "Arduboy", Filename = "arduboy2.hex", Title = "Chapter 2 - It was the worst of times" });
            //binaries.Add(new BinaryInfo() { Device = "Devkit", Filename = "devkit2.hex", Title = "Chapter 2 - It was the worst of times" });
            //var dialog = new BinarySelectionDialog(binaries);
            //dialog.Owner = this;
            //var result = dialog.ShowDialog();
            //var selected = dialog.SelectedBinaryInfo;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var file in Directory.GetFiles(App.TempDirectory))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to delete file '{file}': {ex.Message}");
                }
            }

            Application.Current.Shutdown();
        }

        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var listBox = (sender as ListBox);
            listBox.ScrollIntoView(listBox.SelectedItem);
        }
    }
}
