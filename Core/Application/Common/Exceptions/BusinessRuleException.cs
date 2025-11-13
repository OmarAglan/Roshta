namespace Rosheta.Core.Application.Common.Exceptions
{
    /// <summary>
    /// Exception for business rule violations
    /// </summary>
    public class BusinessRuleException : ApplicationException
    {
        public BusinessRuleException(string message) : base(message)
        {
        }

        public BusinessRuleException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}