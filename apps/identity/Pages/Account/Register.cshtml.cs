using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.DataAnnotations;
using Amphora.Common.Models.Dtos.Users;
using Amphora.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Amphora.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterPageModel : RegisterPageModelBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<RegisterPageModel> logger;
        private readonly IEmailSender emailSender;

        public RegisterPageModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterPageModel> logger,
            IEmailSender emailSender) : base(userManager, signInManager, logger, emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.emailSender = emailSender;
        }

        [BindProperty]
        public CreateAmphoraUser Input { get; set; } = new CreateAmphoraUser();
        [BindProperty]
        [IsTrue(ErrorMessage = "You must accept the service agreement.")]
        public bool AcceptServiceAgreement { get; set; }

        public IActionResult OnGet(string? returnUrl = null, string? email = null)
        {
            ReturnUrl = returnUrl;

            if (email != null)
            {
                Input.Email = email;
            }

            return Page();
        }

        public virtual async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.UserName,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                    About = Input.About,
                    FullName = Input.FullName
                };
                return await HandleRegistration(returnUrl, user, Input.Password!);
            }
            else
            {
                return Page();
            }
        }
    }
}
