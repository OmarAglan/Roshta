using Microsoft.Extensions.DependencyInjection;
using Rosheta.Core.Application.Services;
using Rosheta.Core.Application.Contracts.Services;

namespace Rosheta.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register Application Services
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IMedicationService, MedicationService>();
        services.AddScoped<IPrescriptionService, PrescriptionService>();
        services.AddScoped<ILicenseService, LicenseService>();
        services.AddScoped<ISettingsService, SettingsService>();

        // Future: Add AutoMapper, Validators (FluentValidation), MediatR here
        
        return services;
    }
}