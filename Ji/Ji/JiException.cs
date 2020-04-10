using System;
using System.Runtime.Serialization;

namespace Ji
{
    [Serializable]
    public class JiException : Exception
    {
        public JiException()
        {
        }

        public JiException(string message) : base(message)
        {
        }

        public JiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected JiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}