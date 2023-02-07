using Microsoft.AspNetCore.Mvc;
using ShopBase;
using ShopBase.Model;
using WebApp.Misc;

namespace WebApp.Pages;

public class OrderModel : WebshopBasePageModel
{
    public IEnumerable<Bestellung> Orders { get; private set; } = Enumerable.Empty<Bestellung>();

    public IActionResult OnGet()
    {
        if (this.CurrentUser == null)
        {
            return NotFound();
        }

        this.ReadOrders();

        return Page();
    }

    private void ReadOrders()
    {
        this.Orders = Bestellung.ReadAll()
                                .Where(order => order.Kunde == this.CurrentUser &&
                                    (order.Status != BestellStatus.Warenkorb))
                                .OrderByDescending(order => order.BestellDatum)
                                .ToList();
    }

    public IActionResult OnPostExecuteOrder()
    {
        if (this.CurrentUser == null)
        {
            return RedirectToPage("Login", new { RedirectAfterLogin = "Cart" });
        }

        this.ReadOrders();

        if (this.Cart.Positionen.Count == 0)
        {
            this.MessageList.Add(new("Der Warenkorb ist leer!", MessageType.Danger));
            return Page();
        }

        Bestellung bestToCreate = this.Cart;
        bestToCreate.Status = BestellStatus.Bestellt;
        bestToCreate.Kunde = this.CurrentUser;
        bestToCreate.BestellDatum = DateTime.Today;
        bestToCreate.Update();

        this.Cart = new Bestellung();
        this.MessageList.Add(new($"Bestellung aufgegeben, wir melden uns (eventuell).", MessageType.Success));

        return Page();
    }

    public IActionResult OnPostCancelOrder(int? id)
    {
        if (this.CurrentUser == null)
        {
            return RedirectToPage("Login", new { RedirectAfterLogin = "Order" });
        }

        var bestToCancel = id == null ? null : Bestellung.GetById(id.Value);
        if (id == null || bestToCancel == null)
        {
            this.MessageList.Add(new("Bestellung nicht gefunden", MessageType.Danger));
            return Page();
        }

        if (bestToCancel.Status != BestellStatus.Bestellt)
        {
            this.MessageList.Add(new("Bestellung kann nicht mehr storniert werden!", MessageType.Danger));
            return Page();
        }

        bestToCancel.Status = BestellStatus.Storniert;
        bestToCancel.Update();
        this.MessageList.Add(new($"Bestellung #{id.Value.ToString("00000000")} erfolgreich storniert", MessageType.Success));

        this.ReadOrders();

        return Page();
    }

    public IActionResult OnPostAchiveOrder(int? id)
    {
        if (this.CurrentUser == null)
        {
            return RedirectToPage("Login", new { RedirectAfterLogin = "Order" });
        }

        var bestToCancel = id == null ? null : Bestellung.GetById(id.Value);
        if (id == null || bestToCancel == null)
        {
            this.MessageList.Add(new("Bestellung nicht gefunden", MessageType.Danger));
            return Page();
        }

        if (bestToCancel.Status != BestellStatus.Storniert && bestToCancel.Status != BestellStatus.Gezahlt)
        {
            this.MessageList.Add(new("Bestellung kann nicht archiviert werden!", MessageType.Danger));
            return Page();
        }

        bestToCancel.Status = BestellStatus.Archiviert;
        bestToCancel.Update();
        this.MessageList.Add(new($"Bestellung #{id.Value.ToString("00000000")} erfolgreich archiviert", MessageType.Success));

        this.ReadOrders();

        return Page();
    }

    public IActionResult OnPostPayOrder(int? id)
    {
        if (this.CurrentUser == null)
        {
            return RedirectToPage("Login", new { RedirectAfterLogin = "Order" });
        }

        var bestToCancel = id == null ? null : Bestellung.GetById(id.Value);
        if (id == null || bestToCancel == null)
        {
            this.MessageList.Add(new("Bestellung nicht gefunden", MessageType.Danger));
            return Page();
        }

        if (bestToCancel.Status != BestellStatus.Gezahlt)
        {
            this.MessageList.Add(new("Bestellung ist schon bezahlt oder storniert!", MessageType.Danger));
            return Page();
        }

        // TODO...
        this.MessageList.Add(new("Unser Zahlsystem ist aktuell kaputt. Bitte probieren Sie es später noch einmal oder rufen Sie den Support.", MessageType.Danger));
        return Page();
    }
}