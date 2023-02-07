using Microsoft.AspNetCore.Mvc;
using ShopBase.Model;
using WebApp.Misc;

namespace WebApp.Pages
{
    public class CartModel : WebshopBasePageModel
    {
        public SearchRequest? LastSearch { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPostContinueShopping(string? searchterm, int? curpage)
        {
            return RedirectToPage("/Search", new { SearchTerm = searchterm, CurrentPage = curpage });
        }

        public void OnPostRemoveFromCart(int id)
        {
            var art = Artikel.GetById(id);
            if (art == null) { return; }

            this.Cart.Positionen.RemoveAll(pos => pos.Artikel?.Id == id);

            this.MessageList.Add(new($"Artikel '{art.Bezeichnung}' aus Warenkorb entfernt", MessageType.Success));
        }

        public void OnPostAdd(int id, int? addAmmount, string? searchterm, int? page)
        {
            // Remember the last search for "Weitershoppen" Button
            this.LastSearch = new(searchterm, page);

            if (addAmmount == null || addAmmount.Value < 1)
            {
                this.MessageList.Add(new($"Ungültige Artikelmenge", MessageType.Danger));
                return;
            }

            Artikel? foundArtikel = Artikel.GetById(id);
            if (foundArtikel == null)
            {
                this.MessageList.Add(new($"Ungültiger Artikel", MessageType.Danger));
                return;
            }

            // Check if any position has already this artikel
            var bestPos = this.Cart.Positionen.FirstOrDefault(pos => pos.Artikel == foundArtikel);
            if (bestPos == null)
            {
                bestPos = new() { Artikel = foundArtikel, Menge = addAmmount.Value };
                this.Cart.Positionen.Add(bestPos);
            }
            else { bestPos.Menge += addAmmount.Value; }

            this.MessageList.Add(new PageMessage($"{addAmmount}x '{foundArtikel.Bezeichnung}' in den Warenkorb gelegt", MessageType.Success));
        }
    }
}