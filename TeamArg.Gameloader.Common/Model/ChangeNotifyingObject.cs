using System.ComponentModel;

namespace TeamArg.GameLoader.Common.Model
{
    public abstract class ChangeNotifyingObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));        
    }
}