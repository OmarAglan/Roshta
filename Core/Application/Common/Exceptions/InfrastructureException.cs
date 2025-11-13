namespace Rosheta.Core.Application.Common.Exceptions
{
    /// <summary>
    /// Exception for infrastructure-level failures (DB, file system, etc.)
    /// </summary>
    public class InfrastructureException : ApplicationException
    {
        public InfrastructureException(string message) : base(message)
        {
        }

        public InfrastructureException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}