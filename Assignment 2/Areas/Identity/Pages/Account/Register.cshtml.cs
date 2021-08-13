using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DataValidator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MvcMCBA.Data;
using MvcMCBA.Models;

namespace Assignment_2.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly MCBAContext _context;

        public RegisterModel(

            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
             MCBAContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required]
            [Display(Name = "Opening Account Type")]
            public AccountType AccountType { get; set; }

            [Required]
            [Display(Name = "Opening Deposit")]
            public decimal Balance { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!Input.Balance.ToString().IsDollarAmount())
            {
                ModelState.AddModelError(nameof(Input.Balance), "Enter a dollar amount");
            }
            switch (Input.AccountType)
            {
                case (AccountType.C):
                    if (Input.Balance < AccountChecks.GetCheckingsMin())
                        ModelState.AddModelError(nameof(Input.Balance), "Opening Deposit does not meet minimum");
                    break;
                case (AccountType.S):
                    if (Input.Balance < AccountChecks.GetSavingsMin())
                        ModelState.AddModelError(nameof(Input.Balance), "Opening Deposit does not meet minimum");
                    break;
            }

            if (ModelState.IsValid)
            {
                Random r = new Random();
                int customerID = 0;
                while (customerID == 0)
                {
                    var attemptCustID = r.Next(1001, 9999);
                    var c = await _context.Customers.FindAsync(attemptCustID);
                    if (c == null)
                        customerID = attemptCustID;
                }
                var customer = new Customer { CustomerID = customerID, Name = Input.Name, Status = CustomerStatus.A };
                _context.Add(customer);
                int accountNumber = 0;
                while (accountNumber == 0)
                {
                    var attemptAccNum = r.Next(1001, 9999);
                    var c = await _context.Accounts.FindAsync(attemptAccNum);
                    if (c == null)
                        accountNumber = attemptAccNum;
                }
                _context.Add(
                new MvcMCBA.Models.Account
                {
                    AccountNumber = accountNumber,
                    AccountType = Input.AccountType,
                    CustomerID = customerID,
                    Balance = Input.Balance
                });
                _context.Add(
                new Transaction
                {
                    AccountNumber = accountNumber,
                    TransactionType = TransactionType.D,
                    Amount = Input.Balance,
                    Comment = "Opening Deposit",
                    TransactionTimeUtc = DateTime.UtcNow

                });
                await _context.SaveChangesAsync();

                int loginID = 0;
                while (loginID == 0)
                {
                    var attemptloginID = r.Next(10000001, 99999999);
                    var c = await _userManager.FindByNameAsync(attemptloginID.ToString());
                    if (c == null)
                        loginID = attemptloginID;
                }

                var user = new ApplicationUser { UserName = loginID.ToString(), Email = Input.Name, EmailConfirmed = true, CustomerID = customerID };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {

                    _logger.LogInformation("User created a new account with password.");
                    _ = await _userManager.AddToRoleAsync(user, "Customer");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    HttpContext.Session.SetString("LoginID", user.UserName);
                    if (user.CustomerID != null)
                    {
                        HttpContext.Session.SetInt32("CustomerID", user.CustomerID.Value);
                        HttpContext.Session.SetString("Name", user.Customer.Name);
                    }
                    return RedirectToAction("Registration", "Login");

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
