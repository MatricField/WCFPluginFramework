using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace WCFPluginFramework.Common
{
    [DataContract]
    public class SerializableException
    {
        [DataMember]
        public Type OriginalException { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Source { get; set; }

        [DataMember]
        public string StackTract { get; set; }

        [DataMember]
        public IList<SerializableException> InnerExceptions { get; set; }

        public static SerializableException FromException(Exception ex)
        {
            var ret = new SerializableException()
            {
                OriginalException = ex.GetType(),
                Message = ex.Message,
                Source = ex.Source,
                StackTract = ex.StackTrace
            };
            switch(ex)
            {
                case AggregateException aex:
                    var exs = new List<SerializableException>();
                    foreach(var iex in aex.InnerExceptions)
                    {
                        exs.Add(FromException(iex));
                    }
                    ret.InnerExceptions = exs;
                    break;
                default:
                    if(null != ex.InnerException)
                    {
                        ret.InnerExceptions = 
                            new List<SerializableException>()
                            {
                                FromException(ex.InnerException)
                            };
                    }
                    break;
            }
            return ret;
        }
    }

    public static class ExceptionExtension
    {
        public static void ThrowFaultedExceptionFromThis<Ex>(this Ex ex) 
            where Ex : Exception 
        {
            throw new FaultException<SerializableException>(SerializableException.FromException(ex));
        }
    }
}
