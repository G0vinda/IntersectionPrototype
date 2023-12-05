using System;

namespace Helpers
{
    public static class ExtensionMethods
    {
        public static void ShuffleArray<T>(this T[] array)
        {
            Random random = new Random();

            for (var i = array.Length - 1; i > 0; i--)
            {
                var j = random.Next(0, i + 1);
                T temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }
    }
}