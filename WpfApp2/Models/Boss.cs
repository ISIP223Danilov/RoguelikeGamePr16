using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    
        public class Boss : Enemy
        {
            public string Race { get; set; }

            public Boss(string name, string race, int health, int attack, int defense)
                : base(name, health, attack, defense)
            {
                Race = race;
            }

            public static Boss CreateVVG()
            {
                var boss = new Boss("ВВГ", "Гоблин",
                    (int)(30 * 2.0),
                    (int)(12 * 1.5),
                    (int)(3 * 1.2))
                {
                    SpecialAbility = "Критический удар (30%)",
                    CritChance = 0.3
                };
                return boss;
            }

            public static Boss CreateKowalski()
            {
                var boss = new Boss("Ковальский", "Скелет",
                    (int)(40 * 2.5),
                    (int)(10 * 1.3),
                    (int)(5 * 1.4))
                {
                    SpecialAbility = "Игнорирует защиту",
                    IgnoresArmor = true
                };
                return boss;
            }

            public static Boss CreateArchmage()
            {
                var boss = new Boss("Архимаг C++", "Маг",
                    (int)(25 * 1.8),
                    (int)(15 * 1.6),
                    (int)(2 * 1.1))
                {
                    SpecialAbility = "Заморозка (25%)",
                    FreezeChance = 0.25
                };
                return boss;
            }

            public static Boss CreatePestov()
            {
                var boss = new Boss("Пестов С––", "Скелет",
                    (int)(40 * 1.3),
                    (int)(10 * 1.8),
                    (int)(5 * 0.6))
                {
                    SpecialAbility = "Заморозка (15%)",
                    FreezeChance = 0.15,
                    IgnoresArmor = true
                };
                return boss;
            }
        }
    
}
