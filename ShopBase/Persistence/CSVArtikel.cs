using System;
using ShopBase.Model;
using ShopBase.Tools;

namespace ShopBase.Persistence
{
    public class CSVArtikel
    {
        public static readonly string FILE_NAME = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"ImpotentWebShopData/Artikel.csv");

        public static IEnumerable<Artikel> ReadAll()
        {
            return Import(FILE_NAME);
        }

        public static IEnumerable<Artikel> Import(string path)
        {
            if (!File.Exists(path)) { yield break; }

            using (var sr = new StreamReader(path))
            {
                string? line = sr.ReadLine();
                while (line != null)
                {
                    string[] fields = line.Split(";");
                    Artikel a = GetFromFields(fields);

                    line = sr.ReadLine();
                    yield return a;
                }
            }
        }

        private static void SaveList(IEnumerable<Artikel> artikelList)
        {
            Export(artikelList, FILE_NAME);
        }

        public static void Export(IEnumerable<Artikel> artikelList, string path)
        {
            string? directory = Path.GetDirectoryName(path);
            if (!String.IsNullOrEmpty(directory) && !Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

            using (var sw = new StreamWriter(path, false))
            {
                foreach (Artikel artikel in artikelList)
                {
                    sw.WriteLine(ToCsv(artikel));
                }
            }
        }

        public static void Create(Artikel art)
        {
            // Get ID
            int nextId = ReadAll().Max(art => art.Id) + 1;
            art.Id = nextId;

            string? directory = Path.GetDirectoryName(FILE_NAME);
            if (!String.IsNullOrEmpty(directory) && !Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

            using (var sw = new StreamWriter(FILE_NAME, true))
            {
                sw.WriteLine(ToCsv(art));
            }
        }

        public static Artikel GetFromFields(string[] fields)
        {
            Artikel artikel = new Artikel();
            artikel.Id = fields[0].ToInt32();
            artikel.Bezeichnung = fields[1];
            artikel.Beschreibung = fields[2];
            artikel.Preis = fields[3].To<decimal>();
            artikel.Kundenbewertung = fields[4].ToInt32();

            return artikel;
        }

        public static string ToCsv(Artikel a)
        {
            return $"{a.Id};{a.Bezeichnung};{a.Beschreibung};{a.Preis};{a.Kundenbewertung}";
        }

        internal static void Delete(Artikel artikel)
        {
            var artikelList = CSVArtikel.ReadAll().ToList();
            artikelList.Remove(artikel);

            CSVArtikel.SaveList(artikelList);
        }

        internal static void Update(Artikel artikel)
        {
            var lArtikel = CSVArtikel.ReadAll().ToList();
            int index = lArtikel.IndexOf(artikel);

            if (index < 0) { throw new InvalidOperationException("Artikel Id ist nicht bekannt!"); }

            lArtikel[index] = artikel;

            CSVArtikel.SaveList(lArtikel);
        }
    }
}