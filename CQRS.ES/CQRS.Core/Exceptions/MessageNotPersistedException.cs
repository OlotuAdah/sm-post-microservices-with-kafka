namespace CQRS.Core.Exceptions
{
    public class MessageNotPersistedException(string message) : Exception(message)
    {
    }
}