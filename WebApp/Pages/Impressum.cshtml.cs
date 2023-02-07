using System.Reflection;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShopBase.Model;
using FWI2Helper;

namespace WebApp.Pages
{
    public class ImpressumModel : WebshopBasePageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IWebHostEnvironment _env;

        public ImpressumModel(ILogger<IndexModel> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;

            
        }

        public IWebHostEnvironment Environment => _env;
    }
}