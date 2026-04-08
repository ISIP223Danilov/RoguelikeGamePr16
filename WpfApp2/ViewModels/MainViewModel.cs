using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp2.Commands;
using WpfApp2.Models;
using WpfApp2.Services;


namespace WpfApp2.ViewModels
{

    public class MainViewModel : INotifyPropertyChanged
    {
        private GameService _game;
        private GamePhase _currentPhase;
        private EncounterType _currentEncounter;
        private System.Collections.Generic.List<Enemy> _currentEnemies;
        private bool _showItemChoice;
        private Item _pendingItem;
        private string _currentImage;
        private ObservableCollection<Item> _inventoryItems;

        public GamePhase CurrentPhase
        {
            get => _currentPhase;
            set { _currentPhase = value; OnPropertyChanged(); }
        }

        public EncounterType CurrentEncounter
        {
            get => _currentEncounter;
            set { _currentEncounter = value; OnPropertyChanged(); }
        }

        public bool ShowItemChoice
        {
            get => _showItemChoice;
            set { _showItemChoice = value; OnPropertyChanged(); }
        }

        public string CurrentImage
        {
            get => _currentImage;
            set { _currentImage = value; OnPropertyChanged(); }
        }

        public string EnemyInfo
        {
            get
            {
                if (_currentEnemies != null && _currentEnemies.Count > 0)
                {
                    var enemy = _currentEnemies[0];
                    return $"{enemy.Name} ❤️ {enemy.Health}/{enemy.MaxHealth} ⚔️ {enemy.Attack} 🛡️ {enemy.Defense}\n{enemy.SpecialAbility}";
                }
                return "Нет врагов";
            }
        }

        public ObservableCollection<Item> InventoryItems
        {
            get => _inventoryItems;
            set { _inventoryItems = value; OnPropertyChanged(); }
        }

        public Item PendingItem
        {
            get => _pendingItem;
            set { _pendingItem = value; OnPropertyChanged(); }
        }

        public ICommand StartGameCommand { get; set; }
        public ICommand NextTurnCommand { get; set; }
        public ICommand AttackCommand { get; set; }
        public ICommand DefendCommand { get; set; }
        public ICommand TakeItemCommand { get; set; }
        public ICommand DiscardItemCommand { get; set; }
        public ICommand RestartCommand { get; set; }
        public ICommand UseItemCommand { get; set; }

        public MainViewModel()
        {
            _game = new GameService();
            _currentPhase = GamePhase.StartMenu;
            _inventoryItems = new ObservableCollection<Item>();

            StartGameCommand = new RelayCommand(_ => StartGame());
            NextTurnCommand = new RelayCommand(_ => NextTurn());
            AttackCommand = new RelayCommand(_ => PlayerAttack());
            DefendCommand = new RelayCommand(_ => PlayerDefend());
            TakeItemCommand = new RelayCommand(_ => TakeItem());
            DiscardItemCommand = new RelayCommand(_ => DiscardItem());
            RestartCommand = new RelayCommand(_ => RestartGame());
            UseItemCommand = new RelayCommand(item => UseItem(item as Item));
        }

        private void StartGame()
        {
            _game.NewGame();
            CurrentPhase = GamePhase.Gameplay;
            UpdateInventoryDisplay();
            NextTurn();
        }

        private void NextTurn()
        {
            if (_game.IsInCombat)
                return;

            if (_game.Player.Health <= 0)
            {
                CurrentPhase = GamePhase.GameOver;
                return;
            }

            var encounter = _game.GenerateEncounter();
            CurrentEncounter = encounter;

            if (encounter == EncounterType.Enemy)
            {
                bool isBoss = _game.Player.CurrentFloor % 10 == 0;
                _currentEnemies = _game.GenerateEnemies(isBoss);
                _game.StartCombat(_currentEnemies);
                UpdateGameImage(_currentEnemies[0].Name);
                OnPropertyChanged(nameof(EnemyInfo));
            }
            else
            {
                var item = _game.GenerateChestItem();
                PendingItem = item;
                UpdateGameImage("chest");

                if (item.Type != ItemType.HealthPotion)
                {
                    ShowItemChoice = true;
                }
                else
                {
                    _game.ApplyItemFromChest(item);
                    if (_game.Inventory.AddItem(item))
                    {
                        _game.Logger.AddEvent($"✅ {item.Name} добавлен в инвентарь!");
                        UpdateInventoryDisplay();
                    }
                    NextTurn();
                }
            }
        }

        private void PlayerAttack()
        {
            if (!_game.IsInCombat) return;

            _game.PlayerAttack();
            UpdateInventoryDisplay();

            if (_game.Player.Health <= 0)
            {
                CurrentPhase = GamePhase.GameOver;
                return;
            }

            if (_game.IsInCombat)
            {
                _game.EnemyTurn();
                UpdateInventoryDisplay();
                if (_game.Player.Health <= 0)
                {
                    CurrentPhase = GamePhase.GameOver;
                }
            }

            if (_currentEnemies != null && _currentEnemies.Count > 0)
                OnPropertyChanged(nameof(EnemyInfo));
        }

        private void PlayerDefend()
        {
            if (!_game.IsInCombat) return;

            _game.PlayerDefend();
            _game.EnemyTurn();
            UpdateInventoryDisplay();

            if (_game.Player.Health <= 0)
            {
                CurrentPhase = GamePhase.GameOver;
            }

            if (_currentEnemies != null && _currentEnemies.Count > 0)
                OnPropertyChanged(nameof(EnemyInfo));
        }

        private void TakeItem()
        {
            if (PendingItem.Type == ItemType.Weapon)
            {
                _game.EquipWeapon(PendingItem);
            }
            else if (PendingItem.Type == ItemType.Armor)
            {
                _game.EquipArmor(PendingItem);
            }

            if (!_game.Inventory.IsFull)
            {
                _game.Inventory.AddItem(PendingItem);
                _game.Logger.AddEvent($"✅ {PendingItem.Name} добавлен в инвентарь!");
                UpdateInventoryDisplay();
            }
            else
            {
                _game.Logger.AddEvent($"⚠️ Инвентарь полон! {PendingItem.Name} утерян!");
            }

            ShowItemChoice = false;
            PendingItem = null;
            NextTurn();
        }

        private void DiscardItem()
        {
            _game.Logger.AddEvent($"❌ Вы выбросили {PendingItem.Name}");
            ShowItemChoice = false;
            PendingItem = null;
            NextTurn();
        }

        private void RestartGame()
        {
            _game.NewGame();
            CurrentPhase = GamePhase.Gameplay;
            UpdateInventoryDisplay();
            NextTurn();
        }

        private void UseItem(Item item)
        {
            if (item == null) return;

            var items = _game.GetInventoryItems();
            int index = items.IndexOf(item);

            if (index >= 0 && _game.UseItemFromInventory(index))
            {
                _game.Logger.AddEvent($"🎒 Использован предмет: {item.Name}");
                UpdateInventoryDisplay();
                OnPropertyChanged(nameof(Player));
            }
        }

        private void UpdateInventoryDisplay()
        {
            InventoryItems.Clear();
            var items = _game.GetInventoryItems();
            foreach (var item in items)
            {
                InventoryItems.Add(item);
            }
        }

        private void UpdateGameImage(string imageType)
        {
            // Используем эмодзи вместо картинок
            switch (imageType.ToLower())
            {
                case "goblin":
                case "ввг":
                    CurrentImage = "👺";
                    break;
                case "skeleton":
                case "ковальский":
                case "пестов":
                    CurrentImage = "💀";
                    break;
                case "mage":
                case "архимаг":
                    CurrentImage = "🧙";
                    break;
                case "chest":
                    CurrentImage = "📦";
                    break;
                default:
                    CurrentImage = "👾";
                    break;
            }
        }

        public Player Player => _game.Player;
        public EventLogger Logger => _game.Logger;
        public bool IsInCombat => _game.IsInCombat;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
