using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Rosheta.Infrastructure.Data;
using Rosheta.Infrastructure.Data.Repositories;
using Rosheta.Infrastructure.Storage;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Application.Contracts.Infrastructure;

namespace Rosheta.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Database Configuration
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        // 2. Repositories
        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IMedicationRepository, MedicationRepository>();
        services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();

        // 3. External Infrastructure Services (File, Email, etc.)
        services.AddSingleton<IFileStorageProvider, LocalFileStorageProvider>();

        return services;
    }
}