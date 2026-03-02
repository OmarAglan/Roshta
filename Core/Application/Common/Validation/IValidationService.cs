using Rosheta.Core.Application.Common.Results;

namespace Rosheta.Core.Application.Common.Validation;

public interface IValidationService
{
    Task<Result> ValidateAsync<T>(T instance, CancellationToken cancellationToken = default);
}
