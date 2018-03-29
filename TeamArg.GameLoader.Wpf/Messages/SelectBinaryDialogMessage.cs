using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamArg.GameLoader.Common.Model;

namespace TeamArg.GameLoader.Messages
{
    public class SelectBinaryDialogMessage : MessageBase
    {
        public Action<BinaryInfo> Callback { get; }
        public List<BinaryInfo> BinaryInfos { get; }

        public SelectBinaryDialogMessage(List<BinaryInfo> binaryInfos, Action<BinaryInfo> callback)
        {
            BinaryInfos = binaryInfos;
            Callback = callback;
        }

        public static async Task<BinaryInfo> SendAsync(List<BinaryInfo> binaryInfos, IMessenger messengerInstance = null)
        {
            var tcs = new TaskCompletionSource<BinaryInfo>();
            var message = new SelectBinaryDialogMessage(binaryInfos, (b) =>
            {
                tcs.SetResult(b);
            });

            var messenger = messengerInstance ?? Messenger.Default;
            messenger.Send(message);
            return await tcs.Task;
        }

    }
}