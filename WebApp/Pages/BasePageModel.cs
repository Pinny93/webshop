using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShopBase.Model;
using WebApp.Extensions;
using WebApp.Misc;

namespace WebApp.Pages;

public class WebshopBasePageModel : PageModel
{

    public List<PageMessage> MessageList { get; } = new List<PageMessage>();

    public Bestellung Cart { get; protected set; } = new Bestellung();

    public Kunde? CurrentUser { get; protected set; }

    public WebshopBasePageModel()
    {
    }

    public override Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        Bestellung? serializedCart = HttpContext.Session.GetObject<Bestellung>("cart");
        if (serializedCart != null) { this.Cart = serializedCart; }

        int? user = HttpContext.Session.GetInt32("user");
        if (user != null) { this.CurrentUser = Kunde.TryGetById(user.Value); }

        return base.OnPageHandlerExecutionAsync(context, next);
    }

    public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {

        HttpContext.Session.SetObject("cart", this.Cart);

        base.OnPageHandlerExecuted(context);
    }
}

public class MenuEntry
{
    public string Title { get; set; } = "NewMenuEntry";

    public string Link { get; set; } = "/Index";
}