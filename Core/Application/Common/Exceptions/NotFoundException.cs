namespace Rosheta.Core.Application.Common.Exceptions
{
    /// <summary>
    /// Exception when a requested entity is not found
    /// </summary>
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" with key ({key}) was not found.")
        {
        }

        public NotFoundException(string message) : base(message)
        {
        }
    }
}