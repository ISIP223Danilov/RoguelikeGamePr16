using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    public class Weapon
    {
        public string Name { get; set; }
        public int Damage { get; set; }

        public Weapon(string name, int damage)
        {
            Name = name;
            Damage = damage;
        }

        public static Weapon[] GetAllWeapons()
        {
            return new Weapon[]
            {
                new Weapon("Длинный меч", 15),
                new Weapon("Двуручный топор", 25),
                new Weapon("Кинжал", 12),
                new Weapon("Боевой молот", 20),
                new Weapon("Эльфийский лук", 18)
            };
        }
    }
}
