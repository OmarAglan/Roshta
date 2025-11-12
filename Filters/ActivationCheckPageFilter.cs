using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Roshta.Services.Interfaces;
using System.Threading.Tasks;
using System;

namespace Roshta.Filters;

public class ActivationCheckPageFilter : IAsyncPageFilter
{
    private readonly ILicenseService _licenseService;
    private readonly ILogger<ActivationCheckPageFilter> _logger;

    public ActivationCheckPageFilter(ILicenseService licenseService, ILogger<ActivationCheckPageFilter> logger)
    {
        _licenseService = licenseService;
        _logger = logger;
    }

    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
        // This method runs before model binding. We don't need logic here for this filter.
        return Task.CompletedTask;
    }

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        // Get the path of the page being accessed
        string pagePath = context.ActionDescriptor.RelativePath;
        string activatePagePhysicalPath = "/Pages/Activate.cshtml"; // Keep physical path for comparison
        string setupPagePhysicalPath = "/Pages/DoctorProfile/Setup.cshtml"; // Keep physical path for comparison

        string activatePageRoute = "/Activate"; // Correct route for redirection
        string setupPageRoute = "/DoctorProfile/Setup"; // Correct route for redirection

        // Allow access to Activation page if not activated
        if (pagePath.Equals(activatePagePhysicalPath, StringComparison.OrdinalIgnoreCase))
        {
            // If already activated, redirect away from activation page itself
            if (_licenseService.IsActivated())
            {
                 _logger.LogInformation("Already activated. Redirecting away from Activate page.");
                 // Check if profile setup is needed before redirecting
                 if(!await _licenseService.IsProfileSetupAsync())
                 {
                    _logger.LogInformation("Profile not set up. Redirecting to Setup page.");
                    context.Result = new RedirectToPageResult(setupPageRoute); // Use correct route
                    return;
                 }
                 else
                 {
                    _logger.LogInformation("Profile set up. Redirecting to Index page.");
                    context.Result = new RedirectToPageResult("/Index");
                    return;
                 }
            }
            // If not activated, allow access to the activation page
            _logger.LogDebug("Not activated. Allowing access to Activate page.");
            await next();
            return;
        }

        // --- Check Activation Status for other pages ---
        if (!_licenseService.IsActivated())
        {
            _logger.LogInformation("Application not activated. Redirecting to {ActivatePage} from {PagePath}.", activatePageRoute, pagePath);
            context.Result = new RedirectToPageResult(activatePageRoute); // Use correct route
            return; 
        }

        // --- Activated: Now Check Profile Setup ---

        // Allow access to Setup page itself if profile isn't set up yet
        if (pagePath.Equals(setupPagePhysicalPath, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogDebug("Accessing Setup page directly.");
             if (await _licenseService.IsProfileSetupAsync())
            {
                _logger.LogInformation("Profile already set up. Redirecting from Setup page to Index.");
                context.Result = new RedirectToPageResult("/Index"); // Or main page
                return;
            }
            // Allow access if profile is not set up
            await next();
            return;
        }
        
        // If activated BUT profile not set up, redirect other pages to setup page
        if (!await _licenseService.IsProfileSetupAsync())
        {
            _logger.LogInformation("Profile not set up. Redirecting to {SetupPage} from {PagePath}.", setupPageRoute, pagePath);
             context.Result = new RedirectToPageResult(setupPageRoute); // Use correct route
            return;
        }

        // Activated and Profile is Set up - proceed with the original request
        _logger.LogDebug("Activated and profile set up. Proceeding with request for {PagePath}.", pagePath);
        await next();
    }
} 