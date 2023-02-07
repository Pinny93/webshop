using System;

namespace ShopBase.Tools
{
    public static class ConversionExtensions
    {
        public static int ToInt32(this string toConvert, int defaultValue = default)
        {
            int result;
            if (!Int32.TryParse(toConvert, out result))
            {
                result = defaultValue;
            }

            return result;
        }

        public static T? To<T>(this string toConvert, T? defaultValue = default)
        {
            T? result;
            try
            {
                result = (T)Convert.ChangeType(toConvert, typeof(T));
            }
            catch (Exception)
            {
                result = defaultValue;
            }

            return result;
        }

        public static bool ConvertsTo<T>(this string toConvert)
        {
            try
            {
                Convert.ChangeType(toConvert, typeof(T));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}