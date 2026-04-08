using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    public class Enemy
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public string SpecialAbility { get; set; }

        // Особые способности
        public double CritChance { get; set; } = 0;
        public bool IgnoresArmor { get; set; } = false;
        public double FreezeChance { get; set; } = 0;

        public Enemy(string name, int health, int attack, int defense)
        {
            Name = name;
            MaxHealth = health;
            Health = health;
            Attack = attack;
            Defense = defense;
        }

        public static Enemy CreateGoblin()
        {
            return new Enemy("Гоблин", 30, 12, 3)
            {
                SpecialAbility = "Критический удар (20%)",
                CritChance = 0.2
            };
        }

        public static Enemy CreateSkeleton()
        {
            return new Enemy("Скелет", 40, 10, 5)
            {
                SpecialAbility = "Игнорирует защиту",
                IgnoresArmor = true
            };
        }

        public static Enemy CreateMage()
        {
            return new Enemy("Маг", 25, 15, 2)
            {
                SpecialAbility = "Заморозка (15%)",
                FreezeChance = 0.15
            };
        }

        public Enemy Clone()
        {
            return new Enemy(Name, MaxHealth, Attack, Defense)
            {
                SpecialAbility = this.SpecialAbility,
                CritChance = this.CritChance,
                IgnoresArmor = this.IgnoresArmor,
                FreezeChance = this.FreezeChance
            };
        }
    }
}
