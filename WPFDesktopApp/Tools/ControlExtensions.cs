using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ShopBase.Tools
{
    public static class ControlExtensions
    {
        public static void AddRange<T>(this ItemCollection collection, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                collection.Add(item);
            }
        }
    }
}