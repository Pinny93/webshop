using Microsoft.AspNetCore.Mvc;
using ShopBase.Model;
using WebApp.Misc;

namespace WebApp.Pages
{
    public class RegisterModel : WebshopBasePageModel
    {
        [BindProperty]
        public Kunde FormKunde { get; set; } = new Kunde();

        [BindProperty]
        public string PasswordConfirm { get; set; } = String.Empty;

        [BindProperty]
        public string Password { get; set; } = String.Empty;

        public bool RegisterSucessful { get; set; }

        public void OnGet()
        {
        }

        public void OnPostRegister()
        {
            // Check Passwords
            if (this.Password != this.PasswordConfirm)
            {
                this.MessageList.Add(new PageMessage("Die Passwörter stimmen nicht überein!", MessageType.Danger));
                return;
            }

            if (String.IsNullOrWhiteSpace(this.FormKunde.Name))
            {
                this.MessageList.Add(new PageMessage("Bitte Name eingeben!", MessageType.Danger));
            }
            if (String.IsNullOrWhiteSpace(this.FormKunde.Vorname))
            {
                this.MessageList.Add(new PageMessage("Bitte Vorname eingeben!", MessageType.Danger));
            }
            if (String.IsNullOrWhiteSpace(this.FormKunde.EMail))
            {
                this.MessageList.Add(new PageMessage("Bitte E-Mail eingeben!", MessageType.Danger));
            }

            // TODO: Check E-Mail already exists

            this.FormKunde.SetPassword(this.Password);

            try
            {
                this.FormKunde.Create();
            }
            catch (Exception ex)
            {
                this.MessageList.Add(new PageMessage($"Ein unerwarteter Fehler ist aufgetreten: {ex.Message}!", MessageType.Danger));
                return;
            }

            this.RegisterSucessful = true;
            this.MessageList.Add(new PageMessage("Benutzerkonto erfolgreich angelegt!", MessageType.Success));
        }
    }
}