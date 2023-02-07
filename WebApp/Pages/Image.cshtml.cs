using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShopBase.Model;
using ShopBase.Persistence;

namespace WebApp.Pages;

public class ImageModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int? ImageId { get; set; }

    public IActionResult OnGet()
    {
        if(this.ImageId == null)
        {
            return NotFound();
        }

        Image? image = DBAccess<Image>.Instance.TryGetById(this.ImageId.Value);
        if(image == null || image.ImageData == null)
        {
            return NotFound();
        }
        
        // Chache images, they are the same every time...
        Response.Headers["Cache-Control"] = $"public,max-age={60 * 60 * 24 /* 1 day */}";

        return File(image.ImageData, image.DataType);
    }

}