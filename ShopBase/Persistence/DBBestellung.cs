using System;
using ShopBase.Model;

namespace ShopBase.Persistence;

public class DBBestellung : DBAccess<Bestellung>
{
    public static DBBestellung GetInstance()
    {
        return new DBBestellung();
    }
}