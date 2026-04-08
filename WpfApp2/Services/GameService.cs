using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Models;

namespace WpfApp2.Services
{
    public class GameService
    {
        public Player Player { get; private set; }
        public Inventory Inventory { get; private set; }
        public EventLogger Logger { get; private set; }
        public CombatService Combat { get; private set; }

        private List<Enemy> _currentEnemies;
        private bool _isInCombat;
        private int _turnCount;
        private bool _playerFrozenThisTurn;

        public bool IsInCombat => _isInCombat;
        public int CurrentEnemiesCount => _currentEnemies?.Count ?? 0;

        public GameService()
        {
            Player = new Player();
            Inventory = new Inventory();
            Logger = new EventLogger();
            Combat = new CombatService(Logger);
            _currentEnemies = new List<Enemy>();
            _isInCombat = false;
            _turnCount = 0;
        }

        public void NewGame()
        {
            Player = new Player();
            Inventory = new Inventory();
            Logger.Clear();
            Combat = new CombatService(Logger);
            _currentEnemies.Clear();
            _isInCombat = false;
            _turnCount = 0;
            _playerFrozenThisTurn = false;

            // Добавляем стартовый инвентарь
            Inventory.AddItem(Item.CreateHealthPotion(30));
            Logger.AddEvent("Игра начата! Добро пожаловать в подземелье!");
            Logger.AddEvent($"У вас есть {Inventory.Count} предмет(ов) в инвентаре");
        }

        public EncounterType GenerateEncounter()
        {
            // Проверка на босса каждые 10 ходов
            if (_turnCount > 0 && _turnCount % 10 == 0)
            {
                _turnCount++;
                return EncounterType.Enemy;
            }

            _turnCount++;

            bool isEnemy = RandomService.Chance(0.5);
            return isEnemy ? EncounterType.Enemy : EncounterType.Chest;
        }

        public List<Enemy> GenerateEnemies(bool isBoss = false)
        {
            var enemies = new List<Enemy>();

            if (isBoss)
            {
                var boss = GenerateRandomBoss();
                enemies.Add(boss);
                Logger.AddEvent($"⚠️ ВАС ЖДЕТ БОСС: {boss.Name} ⚠️");
            }
            else
            {
                int enemyCount = RandomService.Next(1, 4);
                for (int i = 0; i < enemyCount; i++)
                {
                    enemies.Add(GenerateRandomEnemy());
                }
                Logger.AddEvent($"Вам противостоит {enemyCount} враг(а)!");
            }

            return enemies;
        }

        private Enemy GenerateRandomEnemy()
        {
            int type = RandomService.Next(0, 3);
            switch (type)
            {
                case 0: return Enemy.CreateGoblin();
                case 1: return Enemy.CreateSkeleton();
                default: return Enemy.CreateMage();
            }
        }

        private Boss GenerateRandomBoss()
        {
            int type = RandomService.Next(0, 4);
            switch (type)
            {
                case 0: return Boss.CreateVVG();
                case 1: return Boss.CreateKowalski();
                case 2: return Boss.CreateArchmage();
                default: return Boss.CreatePestov();
            }
        }

        public void StartCombat(List<Enemy> enemies)
        {
            _currentEnemies = enemies.Select(e => e.Clone()).ToList();
            _isInCombat = true;
            _playerFrozenThisTurn = false;
        }

        public bool PlayerAttack()
        {
            if (_playerFrozenThisTurn)
            {
                Logger.AddEvent("❄️ Вы заморожены и не можете атаковать!");
                return false;
            }

            if (_currentEnemies == null || _currentEnemies.Count == 0)
            {
                EndCombat();
                return false;
            }

            var currentEnemy = _currentEnemies[0];
            int damage = Combat.CalculatePlayerDamage(Player);
            currentEnemy.Health -= damage;

            Logger.AddEvent($"⚔️ Вы нанесли {damage} урона {currentEnemy.Name} (HP: {Math.Max(0, currentEnemy.Health)}/{currentEnemy.MaxHealth})");

            if (currentEnemy.Health <= 0)
            {
                _currentEnemies.RemoveAt(0);
                Logger.AddEvent($"✅ {currentEnemy.Name} повержен!");

                // Шанс выпадения предмета с убитого врага
                if (RandomService.Chance(0.3)) // 30% шанс дропа
                {
                    var droppedItem = Item.CreateRandomItem();
                    Logger.AddEvent($"📦 С {currentEnemy.Name} упал предмет: {droppedItem}");

                    if (Inventory.IsFull)
                    {
                        Logger.AddEvent($"⚠️ Инвентарь полон! {droppedItem.Name} утерян!");
                    }
                    else
                    {
                        Inventory.AddItem(droppedItem);
                        Logger.AddEvent($"✅ {droppedItem.Name} добавлен в инвентарь!");
                    }
                }

                if (_currentEnemies.Count == 0)
                {
                    EndCombat();
                    Player.CurrentFloor++;
                    Logger.AddEvent($"🎉 Бой окончен! Вы перешли на этаж {Player.CurrentFloor}");
                    return true;
                }
            }

            return true;
        }

        public void EnemyTurn()
        {
            if (_currentEnemies == null || _currentEnemies.Count == 0)
                return;

            var currentEnemy = _currentEnemies[0];

            // Проверка заморозки
            if (Combat.CheckFreeze(currentEnemy))
            {
                _playerFrozenThisTurn = true;
                return;
            }

            // Проверка защиты игрока
            bool dodged = false;
            if (Combat.IsDefending())
            {
                dodged = Combat.TryDefend();
                Combat.ResetDefense();
            }

            if (!dodged)
            {
                int damage = Combat.CalculateEnemyDamage(currentEnemy, Player);
                Player.Health -= damage;
                Logger.AddEvent($"💔 {currentEnemy.Name} нанес {damage} урона! Ваше HP: {Player.Health}/{Player.MaxHealth}");

                if (Player.Health <= 0)
                {
                    Logger.AddEvent("💀 ВЫ ПОГИБЛИ! 💀");
                }
            }
        }

        public void PlayerDefend()
        {
            if (_playerFrozenThisTurn)
            {
                Logger.AddEvent("❄️ Вы заморожены и не можете защищаться!");
                return;
            }
            Combat.TryDefend();
            Logger.AddEvent("🛡️ Вы приготовились защищаться!");
        }

        private void EndCombat()
        {
            _isInCombat = false;
            _currentEnemies.Clear();
            Combat.ResetDefense();
            _playerFrozenThisTurn = false;
        }

        public Item GenerateChestItem()
        {
            var item = Item.CreateRandomItem();
            Logger.AddEvent($"🎁 Вы нашли в сундуке: {item}");
            return item;
        }

        public void ApplyItemFromChest(Item item)
        {
            switch (item.Type)
            {
                case ItemType.HealthPotion:
                    item.Apply(Player);
                    break;
                case ItemType.Weapon:
                    Logger.AddEvent($"⚔️ Текущее оружие: {Player.CurrentWeapon.Name} (Урон: {Player.CurrentWeapon.Damage})");
                    Logger.AddEvent($"📦 Новое оружие: {item.Name} (Урон: {item.Value})");
                    break;
                case ItemType.Armor:
                    Logger.AddEvent($"🛡️ Текущий доспех: {Player.CurrentArmor.Name} (Защита: {Player.CurrentArmor.Defense})");
                    Logger.AddEvent($"📦 Новый доспех: {item.Name} (Защита: {item.Value})");
                    break;
            }
        }

        public void EquipWeapon(Item weaponItem)
        {
            var newWeapon = new Weapon(weaponItem.Name, weaponItem.Value);
            Player.CurrentWeapon = newWeapon;
            Logger.AddEvent($"⚔️ Экипировано новое оружие: {weaponItem.Name} (Урон: {weaponItem.Value})");
        }

        public void EquipArmor(Item armorItem)
        {
            var newArmor = new Armor(armorItem.Name, armorItem.Value);
            Player.CurrentArmor = newArmor;
            Logger.AddEvent($"🛡️ Экипирован новый доспех: {armorItem.Name} (Защита: {armorItem.Value})");
        }

        public bool UseItemFromInventory(int itemIndex)
        {
            var item = Inventory.GetItemAt(itemIndex);
            if (item == null) return false;

            switch (item.Type)
            {
                case ItemType.HealthPotion:
                    item.Apply(Player);
                    Inventory.RemoveItem(item);
                    Logger.AddEvent($"💚 Использовано {item.Name}. HP: {Player.Health}/{Player.MaxHealth}");
                    return true;

                case ItemType.Weapon:
                    EquipWeapon(item);
                    Inventory.RemoveItem(item);
                    return true;

                case ItemType.Armor:
                    EquipArmor(item);
                    Inventory.RemoveItem(item);
                    return true;

                default:
                    return false;
            }
        }

        public List<Item> GetInventoryItems()
        {
            return Inventory.Items.ToList();
        }
    }
}
