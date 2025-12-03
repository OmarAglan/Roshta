using Microsoft.EntityFrameworkCore;
using Rosheta.Infrastructure.Storage;
using Rosheta.Core.Application.Contracts.Infrastructure;
using Rosheta.Infrastructure.Data;
using Rosheta.Infrastructure.Data.Repositories;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Application.Services;
using Rosheta.Core.Application.Contracts.Services;
using Rosheta.Core.Application.Settings;
using Rosheta.Filters;
using Rosheta.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure settings
builder.Services.Configure<LicenseSettings>(
    builder.Configuration.GetSection(LicenseSettings.SectionName));

// Add services to the container.

// --- Add DbContext configuration ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
// -----------------------------------

// --- Register Infrastructure Services ---
builder.Services.AddSingleton<IFileStorageProvider, LocalFileStorageProvider>();
// -----------------------------------------

// --- Register Repositories --------
builder.Services.AddScoped<IMedicationRepository, MedicationRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
// -----------------------------------

// --- Register Services ------------
builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();
builder.Services.AddScoped<IMedicationService, MedicationService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<ILicenseService, LicenseService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
// -----------------------------------

// Configure Razor Pages and add the global filter
builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.RootDirectory = "/Presentation/Pages";
    })
    .AddMvcOptions(options =>
    {
        options.Filters.Add<ActivationCheckPageFilter>();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
// Development vs Production error handling
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // Use custom global exception handler
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    // Use default error page as fallback
    app.UseExceptionHandler("/Error");

    // HSTS
    app.UseHsts();
}

// Status code pages for common HTTP errors
app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");

app.UseHttpsRedirection();
app.UseStaticFiles(new Microsoft.AspNetCore.Builder.StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        System.IO.Path.Combine(builder.Environment.ContentRootPath, "Presentation/wwwroot")),
    RequestPath = ""
});

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
