using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    public class Player : INotifyPropertyChanged
    {
        private int _health;
        private int _maxHealth;
        private Weapon _currentWeapon;
        private Armor _currentArmor;
        private int _currentFloor;
        private bool _isFrozen;

        public int Health
        {
            get => _health;
            set { _health = Math.Max(0, value); OnPropertyChanged(); }
        }

        public int MaxHealth
        {
            get => _maxHealth;
            set { _maxHealth = value; OnPropertyChanged(); }
        }

        public Weapon CurrentWeapon
        {
            get => _currentWeapon;
            set { _currentWeapon = value; OnPropertyChanged(); }
        }

        public Armor CurrentArmor
        {
            get => _currentArmor;
            set { _currentArmor = value; OnPropertyChanged(); }
        }

        public int CurrentFloor
        {
            get => _currentFloor;
            set { _currentFloor = value; OnPropertyChanged(); }
        }

        public bool IsFrozen
        {
            get => _isFrozen;
            set { _isFrozen = value; OnPropertyChanged(); }
        }

        public Player()
        {
            MaxHealth = 100;
            Health = MaxHealth;
            CurrentWeapon = new Weapon("Ржавый меч", 10);
            CurrentArmor = new Armor("Кожаный доспех", 3);
            CurrentFloor = 1;
            IsFrozen = false;
        }

        public void Heal(int amount)
        {
            Health = Math.Min(MaxHealth, Health + amount);
        }

        public void HealFull()
        {
            Health = MaxHealth;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
