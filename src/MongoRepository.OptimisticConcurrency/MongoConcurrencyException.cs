namespace MongoRepository.OptimisticConcurrency
{
    using System;
    using System.Runtime.Serialization;

    using MongoDB.Driver;

    [Serializable]
    public class MongoConcurrencyException : MongoException
    {
        public MongoConcurrencyException(string message)
            : base(message)
        {
        }

        public MongoConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public MongoConcurrencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
