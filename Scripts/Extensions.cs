using System;

namespace SnowIsland.Scripts
{
    public static class Extensions
    {
        public static int ToInt(this string value, int errVal=0)
        {
            Int32.TryParse(value, out errVal);
            return errVal;
        }
    }
}