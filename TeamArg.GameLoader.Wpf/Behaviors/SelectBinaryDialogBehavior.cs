using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Interactivity;
using TeamArg.GameLoader.Messages;
using TeamArg.GameLoader.Wpf;

namespace TeamArg.GameLoader.Behaviors
{
    public class SelectBinaryDialogBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            Messenger.Default.Register<SelectBinaryDialogMessage>(this, ShowSelectBinaryDialog);
        }

        private void ShowSelectBinaryDialog(SelectBinaryDialogMessage m)
        {
            var dialog = new BinarySelectionDialog(m.BinaryInfos);
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.Owner = Window.GetWindow(AssociatedObject);

            if (dialog.ShowDialog().Value)
            {
                m.Callback?.Invoke(dialog.SelectedBinaryInfo);
            }            
        }
    }

}