using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
   
        public class Armor
        {
            public string Name { get; set; }
            public int Defense { get; set; }

            public Armor(string name, int defense)
            {
                Name = name;
                Defense = defense;
            }

            public static Armor[] GetAllArmors()
            {
                return new Armor[]
                {
                new Armor("Стальная кираса", 8),
                new Armor("Черный доспех", 12),
                new Armor("Легкая кольчуга", 5),
                new Armor("Пластинчатый доспех", 15),
                new Armor("Магическая мантия", 6)
                };
            }
        }
    
}
