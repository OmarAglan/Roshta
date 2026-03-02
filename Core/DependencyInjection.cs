using FluentValidation;
using Rosheta.Core.Application.Common.Validation;
using Microsoft.Extensions.DependencyInjection;
using Rosheta.Core.Application.Services;
using Rosheta.Core.Application.Contracts.Services;
using Rosheta.Core.Application.Validators;

namespace Rosheta.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Automatic validation pipeline
        services.AddValidatorsFromAssemblyContaining<DoctorValidator>();
        services.AddScoped<IValidationService, FluentValidationService>();

        // Register Application Services
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IMedicationService, MedicationService>();
        services.AddScoped<IPrescriptionService, PrescriptionService>();
        services.AddScoped<ILicenseService, LicenseService>();
        services.AddScoped<ISettingsService, SettingsService>();

        // Future: Add AutoMapper and MediatR here
        
        return services;
    }
}
