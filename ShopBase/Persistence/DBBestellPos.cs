using System;
using ShopBase.Model;

namespace ShopBase.Persistence;

public class DBBestellPos : DBAccess<BestellPos>
{
    public static DBBestellPos GetInstance()
    {
        return new DBBestellPos();
    }
}