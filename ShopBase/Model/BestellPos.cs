using System;

namespace ShopBase.Model
{
    public class BestellPos : ModelBase<BestellPos>
    {
        private int _menge;

        public Artikel Artikel { get; set; }

        public int Menge
        {
            get
            {
                return _menge;
            }
            set
            {
                if (value < 1) { throw new ArgumentException("Menge darf nicht kleiner 1 sein!"); }

                _menge = value;
            }
        }

        public decimal SinglePrice { get => this.Artikel?.Preis ?? 0m; }

        public decimal TotalPrice { get => this.SinglePrice * this.Menge; }

        public BestellPos()
        {
            this.Artikel = new();
        }

        public BestellPos(Artikel artikel, int menge)
        {
            if (artikel == null) { throw new ArgumentNullException(nameof(artikel)); }

            this.Artikel = artikel;
            this.Menge = menge;
        }

        public override string ToString()
        {
            return $"{this.Artikel?.Bezeichnung ?? "?",-20} {this.SinglePrice,7:0.00} EUR {this.Menge,3}x {this.TotalPrice,7:0.00} EUR";
        }
    }
}