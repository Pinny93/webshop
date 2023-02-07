using Microsoft.AspNetCore.Mvc.RazorPages;
using ShopBase.Model;

namespace WebApp.Pages
{
    public class IndexModel : WebshopBasePageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public List<Artikel> LstArtikel { get; set; } = Artikel.ReadAll().ToList();

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}