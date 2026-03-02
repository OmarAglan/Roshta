using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Rosheta.Core.Application.Common.Results;

namespace Rosheta.Core.Application.Common.Validation;

public class FluentValidationService : IValidationService
{
    private readonly IServiceProvider _serviceProvider;

    public FluentValidationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<Result> ValidateAsync<T>(T instance, CancellationToken cancellationToken = default)
    {
        if (instance == null)
        {
            return Result.Failure("Validation", "The input payload cannot be null.");
        }

        var validators = _serviceProvider.GetServices<IValidator<T>>().ToList();
        if (validators.Count == 0)
        {
            return Result.Success();
        }

        var context = new ValidationContext<T>(instance);
        var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults
            .SelectMany(x => x.Errors)
            .Where(x => x != null)
            .ToList();

        if (failures.Count == 0)
        {
            return Result.Success();
        }

        var errorMessage = string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));
        return Result.Failure("Validation", errorMessage);
    }
}
