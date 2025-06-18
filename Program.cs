using Microsoft.EntityFrameworkCore;
using Roshta.Data;
using Roshta.Repositories;
using Roshta.Repositories.Interfaces;
using Roshta.Services;
using Roshta.Services.Interfaces;
using Roshta.Settings;
using Roshta.Filters;

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
.AddMvcOptions(options =>
{
    options.Filters.Add<ActivationCheckPageFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
