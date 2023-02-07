using Microsoft.AspNetCore.Mvc;
using ShopBase.Model;
using WebApp.Misc;

namespace WebApp.Pages
{
    public class LoginModel : WebshopBasePageModel
    {
        [BindProperty]
        public string Login { get; set; } = String.Empty;

        [BindProperty]
        public string Passwort { get; set; } = String.Empty;

        [BindProperty(SupportsGet = true)]
        public string? RedirectAfterLogin { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPostLogin()
        {
            if (String.IsNullOrWhiteSpace(this.Login))
            {
                this.MessageList.Add(new PageMessage("Bitte E-Mail/Benutzer eingeben!", MessageType.Danger));
                return Page();
            }
            if (String.IsNullOrWhiteSpace(this.Passwort))
            {
                this.MessageList.Add(new PageMessage("Bitte Passwort eingeben!", MessageType.Danger));
                return Page();
            }

            Kunde? user = Kunde.CheckLogin(this.Login, this.Passwort);
            if (user == null)
            {
                this.MessageList.Add(new PageMessage("Benutzername oder Passwort falsch", MessageType.Danger));
                return Page();
            }

            this.CurrentUser = user;
            HttpContext.Session.SetInt32("user", user.Id);

            if (!String.IsNullOrEmpty(this.RedirectAfterLogin))
            {
                return RedirectToPage(this.RedirectAfterLogin);
            }

            return Page();
        }

        public void OnPostLogout()
        {
            HttpContext.Session.Remove("user");
            this.CurrentUser = null;

            this.MessageList.Add(new PageMessage("Erfolgreich abgemeldet", MessageType.Success));
        }
    }
}