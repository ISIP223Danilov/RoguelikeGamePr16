using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Models;

namespace WpfApp2.Services
{
    public class CombatService
    {
        private readonly EventLogger _logger;
        private bool _isDefending;

        public CombatService(EventLogger logger)
        {
            _logger = logger;
            _isDefending = false;
        }

        public int CalculatePlayerDamage(Player player)
        {
            int baseDamage = player.CurrentWeapon.Damage;
            int finalDamage = baseDamage + RandomService.Next(-3, 6);
            return Math.Max(1, finalDamage);
        }

        public int CalculateEnemyDamage(Enemy enemy, Player player)
        {
            int damage = enemy.Attack;

            // Проверка крита
            if (RandomService.Chance(enemy.CritChance))
            {
                damage *= 2;
                _logger.AddEvent($"{enemy.Name} нанес критический удар!");
            }

            // Проверка игнорирования защиты
            if (!enemy.IgnoresArmor)
            {
                int defense = player.CurrentArmor.Defense;
                int blocked = RandomService.Next((int)(defense * 0.7), defense + 1);
                damage = Math.Max(1, damage - blocked);
                _logger.AddEvent($"Блок поглотил {blocked} урона");
            }
            else
            {
                _logger.AddEvent($"{enemy.Name} игнорирует вашу защиту!");
            }

            return Math.Max(1, damage);
        }

        public bool TryDefend()
        {
            _isDefending = true;
            bool dodged = RandomService.Chance(0.4);
            if (dodged)
            {
                _logger.AddEvent("Вы уклонились от атаки!");
            }
            return dodged;
        }

        public bool IsDefending()
        {
            return _isDefending;
        }

        public void ResetDefense()
        {
            _isDefending = false;
        }

        public bool CheckFreeze(Enemy enemy)
        {
            if (RandomService.Chance(enemy.FreezeChance))
            {
                _logger.AddEvent($"{enemy.Name} заморозил вас! Вы пропускаете ход!");
                return true;
            }
            return false;
        }
    }
}
