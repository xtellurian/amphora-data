using System;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models.Emails;
using Amphora.Api.Models.Host;
using Amphora.Common.Models.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Amphora.Api.Areas.Profiles.Pages.Account.Manage
{
    public class ResendConfirmationEmailPageModel : PageModel
    {
        private readonly IOptionsMonitor<Models.Host.HostOptions> hostOptions;
        private readonly IEmailSender emailSender;
        private readonly IUserService userService;

        public ResendConfirmationEmailPageModel(IOptionsMonitor<Models.Host.HostOptions> hostOptions,
                                                IWebHostEnvironment webHost,
                                                IEmailSender emailSender,
                                                IUserService userService)
        {
            this.hostOptions = hostOptions;
            this.WebHost = webHost;
            this.emailSender = emailSender;
            this.userService = userService;
        }

        public IWebHostEnvironment WebHost { get; private set; }
        public bool Success { get; private set; }
        public ApplicationUser AppUser { get; private set; }

        public async Task<IActionResult> OnGet()
        {
            await LoadProperties();
            return Page();
        }

        private async Task LoadProperties()
        {
            this.AppUser = await userService.ReadUserModelAsync(User);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadProperties();
            if (WebHost.IsDevelopment())
            {
                // just set the email thing to true
                AppUser.EmailConfirmed = true;
                var res = await userService.UserManager.UpdateAsync(AppUser);
            }
            else
            {
                var code = await userService.UserManager.GenerateEmailConfirmationTokenAsync(AppUser);
                await emailSender.SendEmailAsync(new ConfirmEmailEmail(AppUser, hostOptions.CurrentValue, code));
            }

            this.Success = true;
            return Page();
        }
    }
}