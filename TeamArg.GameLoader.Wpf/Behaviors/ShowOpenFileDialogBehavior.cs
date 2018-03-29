using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Interactivity;
using TeamArg.GameLoader.Messages;

namespace TeamArg.GameLoader.Behaviors
{
    public class OpenFileDialogBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            Messenger.Default.Register<OpenFileDialogMessage>(this, ShowOpenFileDialog);
        }

        private void ShowOpenFileDialog(OpenFileDialogMessage m)
        {
            var dialog = new OpenFileDialog()
            {
                Title = m.Title,
                Filter = m.Filter,
                DefaultExt = m.DefaultExt,
                Multiselect = m.MultiSelect,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            var owner = Window.GetWindow(AssociatedObject);

            if (dialog.ShowDialog(owner).Value && dialog.FileNames.Length > 0)
            {
                m.Callback?.Invoke(dialog.FileNames);
            }
        }
    }
}