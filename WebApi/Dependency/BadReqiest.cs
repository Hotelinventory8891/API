using System;
using System.Runtime.Serialization;

namespace Dependency
{
    [Serializable]
    internal class BadReqiest : Exception
    {
        private int v1;
        private string v2;

        public BadReqiest()
        {
        }

        public BadReqiest(string message) : base(message)
        {
        }

        public BadReqiest(string message, Exception innerException) : base(message, innerException)
        {
        }

        public BadReqiest(int v1, string v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        protected BadReqiest(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}