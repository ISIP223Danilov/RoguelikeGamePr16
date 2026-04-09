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
        public int Value { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }

        public Item(string name, ItemType type, int value, string description = "")
        {
            Name = name;
            Type = type;
            Value = value;
            Description = description;
            Icon = GetIconForType(type);

            if (string.IsNullOrEmpty(description))
            {
                Description = GetDefaultDescription(type, value);
            }
        }

        private string GetDefaultDescription(ItemType type, int value)
        {
            switch (type)
            {
                case ItemType.HealthPotion:
                    return "Восстанавливает " + value + " HP";
                case ItemType.Weapon:
                    return "Наносит " + value + " урона";
                case ItemType.Armor:
                    return "Дает " + value + " защиты";
                default:
                    return "Обычный предмет";
            }
        }

        private string GetIconForType(ItemType type)
        {
            switch (type)
            {
                case ItemType.HealthPotion:
                    return "🧪";
                case ItemType.Weapon:
                    return "⚔️";
                case ItemType.Armor:
                    return "🛡️";
                default:
                    return "📦";
            }
        }

        public static Item CreateHealthPotion(int healAmount = 50)
        {
            return new Item("Зелье здоровья", ItemType.HealthPotion, healAmount, "Восстанавливает " + healAmount + " HP");
        }

        public static Item CreateRandomWeapon()
        {
            Random random = new Random();
            string[] weaponNames = { "Ржавый меч", "Стальной клинок", "Двуручный топор", "Кинжал", "Боевой молот", "Эльфийский лук" };
            int[] weaponDamage = { 12, 18, 25, 15, 22, 20 };
            int index = random.Next(weaponNames.Length);
            return new Item(weaponNames[index], ItemType.Weapon, weaponDamage[index], "Наносит " + weaponDamage[index] + " урона");
        }

        public static Item CreateRandomArmor()
        {
            Random random = new Random();
            string[] armorNames = { "Кожаный доспех", "Кольчуга", "Стальная кираса", "Черный доспех", "Пластинчатый доспех" };
            int[] armorDefense = { 5, 8, 12, 15, 18 };
            int index = random.Next(armorNames.Length);
            return new Item(armorNames[index], ItemType.Armor, armorDefense[index], "Дает " + armorDefense[index] + " защиты");
        }

        public static Item CreateRandomItem()
        {
            Random random = new Random();
            int type = random.Next(0, 3);

            if (type == 0)
            {
                return CreateHealthPotion(random.Next(30, 81));
            }
            else if (type == 1)
            {
                return CreateRandomWeapon();
            }
            else
            {
                return CreateRandomArmor();
            }
        }

        public void Apply(Player player)
        {
            if (Type == ItemType.HealthPotion)
            {
                int newHealth = Math.Min(player.MaxHealth, player.Health + Value);
                player.Health = newHealth;
            }
        }

        public override string ToString()
        {
            return Icon + " " + Name + " - " + Description;
        }
    }

    public class Inventory
    {
        private List<Item> _items;
        public const int MaxSize = 10;

        public List<Item> Items
        {
            get { return _items; }
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsFull
        {
            get { return _items.Count >= MaxSize; }
        }

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
    }
}