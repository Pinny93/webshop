using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShopBase.Model;
using WebApp.Misc;

namespace WebApp.Pages
{
    public class SearchModel : WebshopBasePageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; }

        [BindProperty]
        public int ArticlesPerPage { get; set; } = 10;

        public List<Artikel> FoundArtikel { get; } = new List<Artikel>();

        public int TotalArticles { get; set; }

        public SearchModel()
        {
        }

        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            await base.OnPageHandlerExecutionAsync(context, next);

            this.HandleSearch();
        }

        public void HandleSearch()
        {
            if (Request.Query.ContainsKey("s"))
            {
                string? getQuery = Request.Query["s"][0] ?? String.Empty;

                if (getQuery.Trim() == String.Empty)
                {
                    this.MessageList.Add(new PageMessage($"Bitte Suchbegriff eingeben!", MessageType.Warning));
                    return;
                }

                // Handle Search via form ?s={s} ...
                if (getQuery != null)
                {
                    Response.Redirect($"/Search/{WebUtility.UrlEncode(getQuery)}");
                    return;
                }
            }

            if (this.SearchTerm == null) { return; }

            if (this.SearchTerm.Trim() == String.Empty)
            {
                this.MessageList.Add(new PageMessage($"Glückwunsch, du hast ein Ei gefunden!", MessageType.Danger));
                return;
            }

            if (this.CurrentPage < 1) { this.CurrentPage = 1; }

            this.SearchTerm = this.SearchTerm.Trim();

            // TODO: Do Search and Page Limitation on the Database...
            IEnumerable<Artikel> artikelLst = Artikel.ReadAll();
            List<Artikel> foundArtikel = new List<Artikel>();

            foreach (Artikel curArtikel in artikelLst)
            {
                if (curArtikel.Bezeichnung.Contains(this.SearchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    curArtikel.Beschreibung.Contains(this.SearchTerm, StringComparison.CurrentCultureIgnoreCase))
                {
                    foundArtikel.Add(curArtikel);
                }
            }

            this.TotalArticles = foundArtikel.Count;
            int startIndex = (this.CurrentPage - 1) * this.ArticlesPerPage;
            int endIndex = this.CurrentPage * this.ArticlesPerPage;

            for (int i = startIndex; i < foundArtikel.Count && i < endIndex; i++)
            {
                this.FoundArtikel.Add(foundArtikel[i]);
            }

            this.MessageList.Add(new PageMessage($"Suche nach '{this.SearchTerm}' ergab {foundArtikel.Count} Treffer", MessageType.Info));
        }

        //public void OnPostAdd(int id, int? addAmmount, string s, int page)
        //{
        //    if (addAmmount == null || addAmmount.Value < 1) { return; }

        //    Artikel? foundArtikel = Artikel.GetById(id);
        //    if (foundArtikel == null) { return; }

        //    for (int i = 0; i < addAmmount; i++)
        //    {
        //        this.Cart.Add(foundArtikel);
        //    }

        //    this.MessageList.Add(new PageMessage($"{addAmmount}x '{foundArtikel.Bezeichnung}' in den Warenkorb gelegt", MessageType.Success));
        //}
    }
}