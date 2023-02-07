using System;
using System.Text.Json.Serialization;

namespace ShopBase.Model
{
    public class Bestellung : ModelBase<Bestellung>
    {
        public Kunde? Kunde { get; set; }

        [JsonInclude]
        public List<BestellPos> Positionen { get; private set; } = new List<BestellPos>();

        public BestellStatus Status { get; set; }

        public DateTime? BestellDatum { get; set; }

        public override string ToString()
        {
            return $"{this.Id,4} {(this.Kunde?.Vorname ?? "?"),-15} {(this.Kunde?.Name ?? "?"),-15} {this.Positionen.Count,3} Artikel {this.Positionen.Sum(pos => pos.TotalPrice),7:0.00} EUR {this.Status}";
        }
    }
}