using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Services
{
    public static class RandomService
    {
        private static readonly Random _random = new Random();

        public static int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        public static double NextDouble()
        {
            return _random.NextDouble();
        }

        public static bool Chance(double probability)
        {
            return NextDouble() < probability;
        }

        public static T GetRandomItem<T>(T[] items)
        {
            if (items == null || items.Length == 0)
                return default;
            return items[Next(0, items.Length)];
        }
    }
}
