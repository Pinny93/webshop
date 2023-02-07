using System;
using ShopBase.Model;

namespace ShopBase.Persistence;

public class DBKunde : DBAccess<Kunde>
{
    public static DBKunde GetInstance()
    {
        return new DBKunde();
    }
}