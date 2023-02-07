using System;
using ShopBase.Model;

namespace ShopBase.Persistence;

public class DBArtikel : DBAccess<Artikel>
{
    public static DBArtikel GetInstance()
    {
        return new DBArtikel();
    }
}