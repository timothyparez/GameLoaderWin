using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using TeamArg.GameLoader.Common.Model;

namespace TeamArg.GameLoader.Wpf
{
    public partial class BinarySelectionDialog : Window, INotifyPropertyChanged
    {
        public static readonly DependencyProperty AvailableBinaryInfosProperty = DependencyProperty.Register("AvailableBinaryInfos", typeof(List<BinaryInfo>), typeof(BinarySelectionDialog), new PropertyMetadata(null));
        public static readonly DependencyProperty SelectedBinaryInfoProperty = DependencyProperty.Register("SelectedBinaryInfo", typeof(BinaryInfo), typeof(BinarySelectionDialog), new PropertyMetadata(null));

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        public List<BinaryInfo> AvailableBinaryInfos
        {
            get { return (List<BinaryInfo>)GetValue(AvailableBinaryInfosProperty); }
            set { SetValue(AvailableBinaryInfosProperty, value); }
        }
        public BinaryInfo SelectedBinaryInfo
        {
            get { return (BinaryInfo)GetValue(SelectedBinaryInfoProperty); }
            set { SetValue(SelectedBinaryInfoProperty, value); }
        }
                            
        [RaisePropertyChanged]
        public RelayCommand SelectBinaryInfoCommand { get; set; }

        [RaisePropertyChanged]
        public RelayCommand CancelCommand { get; set; }
       
        public BinarySelectionDialog(List<BinaryInfo> availableBinaryInfos)             
        {
            InitializeComponent();
            AvailableBinaryInfos = availableBinaryInfos;
            SelectBinaryInfoCommand = new RelayCommand(SelectBinaryInfoCommand_Execute, () => SelectedBinaryInfo != null);
            CancelCommand = new RelayCommand(CancelCommand_Execute);
        }

        private void SelectBinaryInfoCommand_Execute()
        {
            DialogResult = true;
            Close();
        }

        private void CancelCommand_Execute()
        {
            SelectedBinaryInfo = null;
            DialogResult = false;
            Close();
        }
    }
}
