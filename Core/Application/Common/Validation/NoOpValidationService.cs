using Rosheta.Core.Application.Common.Results;

namespace Rosheta.Core.Application.Common.Validation;

public class NoOpValidationService : IValidationService
{
    public Task<Result> ValidateAsync<T>(T instance, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success());
    }
}
