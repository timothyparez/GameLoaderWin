using GalaSoft.MvvmLight.Messaging;
using System;

namespace TeamArg.GameLoader.Messages
{
    public class OpenFileDialogMessage : MessageBase
    {
        public string Title { get; }

        public string Filter { get; set; }
        
        public string DefaultExt { get; set; }

        public Action<string[]> Callback { get; }
        public bool MultiSelect { get;  }

        public OpenFileDialogMessage(string title, bool multiSelect, string defaultExt, string filter, Action<string[]> callback)
        {
            Title = title;
            MultiSelect = multiSelect;
            DefaultExt = defaultExt;
            Filter = filter;
            Callback = callback;
        }
    }
}
