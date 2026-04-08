using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    public enum ItemType
    {
        HealthPotion,
        Weapon,
        Armor
    }

    public class Item
    {
        public string Name { get; set; }
        public ItemType Type { get; set; }
        public int Value { get; set; } // Для оружия - урон, для брони - защита, для зелья - количество лечения
        public string Description { get; set; }
        public string Icon { get; set; }

        public Item(string name, ItemType type, int value, string description = "")
        {
            Name = name;
            Type = type;
            Value = value;
            Description = string.IsNullOrEmpty(description) ? GetDefaultDescription(type, value) : description;
            Icon = GetIconForType(type);
        }

        private string GetDefaultDescription(ItemType type, int value)
        {
            return type switch
            {
                ItemType.HealthPotion => $"Восстанавливает {value} HP",
                ItemType.Weapon => $"Наносит {value} урона",
                ItemType.Armor => $"Дает {value} защиты",
                _ => "Обычный предмет"
            };
        }

        private string GetIconForType(ItemType type)
        {
            return type switch
            {
                ItemType.HealthPotion => "🧪",
                ItemType.Weapon => "⚔️",
                ItemType.Armor => "🛡️",
                _ => "📦"
            };
        }

        // Фабричные методы для создания предметов
        public static Item CreateHealthPotion(int healAmount = 50)
        {
            return new Item("Зелье здоровья", ItemType.HealthPotion, healAmount, $"Восстанавливает {healAmount} HP");
        }

        public static Item CreateHealthPotionFull()
        {
            return new Item("Великое зелье лечения", ItemType.HealthPotion, 100, "Полностью восстанавливает HP");
        }

        public static Item CreateRandomWeapon()
        {
            var weapons = new[]
            {
                new { Name = "Ржавый меч", Damage = 12 },
                new { Name = "Стальной клинок", Damage = 18 },
                new { Name = "Двуручный топор", Damage = 25 },
                new { Name = "Кинжал убийцы", Damage = 15, Desc = "Быстрое оружие" },
                new { Name = "Боевой молот", Damage = 22 },
                new { Name = "Эльфийский лук", Damage = 20 },
                new { Name = "Пламенный меч", Damage = 28 },
                new { Name = "Ледяная секира", Damage = 24 },
                new { Name = "Драконий коготь", Damage = 30 }
            };

            var random = new Random();
            var weapon = weapons[random.Next(weapons.Length)];
            return new Item(weapon.Name, ItemType.Weapon, weapon.Damage, $"Наносит {weapon.Damage} урона");
        }

        public static Item CreateRandomArmor()
        {
            var armors = new[]
            {
                new { Name = "Кожаный доспех", Defense = 5 },
                new { Name = "Кольчуга", Defense = 8 },
                new { Name = "Стальная кираса", Defense = 12 },
                new { Name = "Черный доспех", Defense = 15 },
                new { Name = "Пластинчатый доспех", Defense = 18 },
                new { Name = "Магическая мантия", Defense = 10 },
                new { Name = "Драконья чешуя", Defense = 20 },
                new { Name = "Небесная броня", Defense = 25 }
            };

            var random = new Random();
            var armor = armors[random.Next(armors.Length)];
            return new Item(armor.Name, ItemType.Armor, armor.Defense, $"Дает {armor.Defense} защиты");
        }

        public static Item CreateRandomItem()
        {
            var random = new Random();
            int type = random.Next(0, 3);

            return type switch
            {
                0 => CreateHealthPotion(random.Next(30, 81)),
                1 => CreateRandomWeapon(),
                2 => CreateRandomArmor(),
                _ => CreateHealthPotion(50)
            };
        }

        public void Apply(Player player)
        {
            switch (Type)
            {
                case ItemType.HealthPotion:
                    int newHealth = Math.Min(player.MaxHealth, player.Health + Value);
                    int healed = newHealth - player.Health;
                    player.Health = newHealth;
                    Console.WriteLine($"{Icon} Вы использовали {Name} и восстановили {healed} HP!");
                    break;
                case ItemType.Weapon:
                    Console.WriteLine($"{Icon} Найдено оружие: {Name} (Урон: {Value})");
                    break;
                case ItemType.Armor:
                    Console.WriteLine($"{Icon} Найдена броня: {Name} (Защита: {Value})");
                    break;
            }
        }

        public override string ToString()
        {
            return $"{Icon} {Name} - {Description}";
        }
    }

    // Класс для инвентаря игрока
    public class Inventory
    {
        private List<Item> _items;
        public const int MaxSize = 10;

        public IReadOnlyList<Item> Items => _items;
        public int Count => _items.Count;
        public bool IsFull => _items.Count >= MaxSize;

        public Inventory()
        {
            _items = new List<Item>();
        }

        public bool AddItem(Item item)
        {
            if (IsFull)
            {
                return false;
            }

            _items.Add(item);
            return true;
        }

        public bool RemoveItem(Item item)
        {
            return _items.Remove(item);
        }

        public Item GetItemAt(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                return _items[index];
            }
            return null;
        }

        public void Clear()
        {
            _items.Clear();
        }

        public List<Item> GetItemsByType(ItemType type)
        {
            return _items.Where(item => item.Type == type).ToList();
        }
    }
}
