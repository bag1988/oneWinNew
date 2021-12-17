using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using oneWin.Models;

namespace oneWin.Areas.Identity.Pages.Account.Manage
{   
  
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<userModel> _userManager;
        private readonly SignInManager<userModel> _signInManager;

        public IndexModel(
            UserManager<userModel> userManager,
            SignInManager<userModel> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string UserId { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
            [Display(Name = "Login")]
            public string Login { get; set; }
            
            [Display(Name = "Отдел")]
            public string otdel { get; set; }
          
        }

        private async Task LoadAsync(userModel user)
        {
            var userName = await Task.FromResult(user.Email);

            Username = userName;

            Input = new InputModel
            {
                UserId = user.Id,
                otdel = user.otdel,
                Login = user.Login,
            };
        }

        public async Task<IActionResult> OnGetAsync(string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userid}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(Input.UserId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{Input.UserId}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }
            if (user.Login != Input.Login)
            {
                if (_userManager.Users.Any(x => x.Login == Input.Login))
                {
                    StatusMessage = $"Логин {Input.Login} уже существует";
                    await LoadAsync(user);
                    return Page();
                }
                user.Login = Input.Login;
            }
            user.otdel = Input.otdel;
            await _userManager.UpdateAsync(user);
            

            //await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Профиль обновлен";
            return RedirectToPage(new { userid = user.Id });
        }
    }
}
