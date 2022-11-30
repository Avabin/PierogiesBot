using System.Runtime.Serialization;

namespace Wow;

[Serializable]
public class UnknownServerException : Exception
{
    public UnknownServerException(string server) : base($"Unknown server: {server}")
    {
    }

    protected UnknownServerException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
    }
}