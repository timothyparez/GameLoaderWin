using NLog;
using NLog.Fluent;
using PostSharp.Aspects;
using System;

namespace TeamArg.GameLoader
{

    [Serializable]
    public class LogException : OnExceptionAspect
    {
        private static Logger logger;

        public sealed override void OnException(MethodExecutionArgs args)
        {
            string message = string.Format("{0} had an error @ {1}: {2}\n{3}",
                                        args.Method.Name, DateTime.Now,
                                        args.Exception.Message, args.Exception.StackTrace);


            if (logger == null)
            {
                logger = LogManager.GetLogger("default");
                logger.Error()
                      .Message(message)
                      .Exception(args.Exception)
                      .Write();
            }
        }
    }

}