using System;
using System.Linq;
using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace TeamArg.GameLoader.ViewModel
{

    public abstract class ChangeNotifyingViewModel : ViewModelBase
    {
        public new void RaisePropertyChanged(string property) => base.RaisePropertyChanged(property);
    }

}