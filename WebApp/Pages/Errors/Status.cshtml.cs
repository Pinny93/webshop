using Microsoft.AspNetCore.Mvc;
using ShopBase.Model;
using ShopBase.Persistence;

namespace WebApp.Pages;

public class StatusModel : WebshopBasePageModel
{
    [BindProperty(SupportsGet = true)]
    public int? ErrorStatusCode { get; set; } 

    public void OnGet()
    {
    }

}