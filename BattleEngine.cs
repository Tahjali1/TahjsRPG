using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using Engine.ViewModels;
using Engine.Factories;
using Engine.EventArgs;

namespace Engine
{
    public class BattleEngine : BaseNotificationClass
    {
        private Player _currentPlayer;
        private Monster _currentMonster;
        public new Weapon CurrentWeapon { get; }
        private Location _currentLocation;
        public World CurrentWorld { get; }


        public new Player CurrentPlayer
        {
            get { return _currentPlayer; }
            set
            {
                if(_currentPlayer != null)
                {
                    _currentPlayer.OnLeveledUp -= OnCurrentPlayerLeveledUp;
                    _currentPlayer.OnKilled -= OnCurrentPlayerKilledNotification;
                }

                _currentPlayer = value;

                if (_currentPlayer != null)
                {
                    _currentPlayer.OnLeveledUp += OnCurrentPlayerLeveledUp;
                    _currentPlayer.OnKilled += OnCurrentPlayerKilledNotification;
                }
            }
        }

        public new Monster CurrentMonster
        {
            get { return _currentMonster; }
            set
            {
                if(_currentMonster != null)
                {
                    _currentMonster.OnKilled -= OnCurrentMonsterKilledNotification;
                }
                
                _currentMonster = value;

                if(_currentMonster != null)
                {
                    MonsterEncounteredNotification();
                    _currentMonster.OnKilled += OnCurrentMonsterKilledNotification;
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(HasMonster));
            }
        }

        public Location CurrentLocation
        {
            get { return _currentLocation; }
            set { }

        }


        public BattleEngine()
        {
            CurrentPlayer = new Player(name:"Tahj", characterClass:"Fighter", experiencePoints:0, maximumHitPoints:10, 
                currentHitPoints:10, strength:10, dexterity:10, armorClass:10, strengthAbilityScore:0, gold:10);
        }
        
        private void GetMonsterAtLocation()
        {
            CurrentMonster = CurrentLocation.GetMonster();
        }


        public bool PlayerAccuracyCalculation()
        {
            int attackRoll = RandomNumberGenerator.NumberBetween(0, 20) +
                         StatisticsCalculator.AbilityScoreCalculator(CurrentPlayer.Dexterity);
            
            if (attackRoll > CurrentMonster.ArmorClass)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public void PlayerAttackDamageCalculation()
        {
            int damageDealtToMonster = RandomNumberGenerator.NumberBetween(CurrentWeapon.MinimumDamage, CurrentWeapon.MaximumDamage) 
                                       + StatisticsCalculator.AbilityScoreCalculator(CurrentPlayer.Strength);
            PlayerAttackSuccessNotification(damageDealtToMonster);

            CurrentMonster.TakeDamage(damageDealtToMonster);

            if (CurrentMonster.IsDead)
            {

                CurrentPlayer.AddExperience(CurrentMonster.RewardExperiencePoints);
                CurrentPlayer.ReceiveGold(CurrentMonster.Gold);
                foreach (GameItem gameItem in CurrentMonster.Inventory)
                {
                    CurrentPlayer.AddItemToInventory(gameItem);
                }

                GetMonsterAtLocation();
                return;
            }
            else //Monster's Turn
            {
                MonsterAccuracyCalculation();
                if (MonsterAccuracyCalculation())
                {
                    MonsterAttackDamageCalculation();
                }
                else
                {
                    MonsterAttackFailureNotification();
                }

            }
        }

        public bool MonsterAccuracyCalculation()
        {
            int attackRoll = RandomNumberGenerator.NumberBetween(0, 20) +
                             StatisticsCalculator.AbilityScoreCalculator(CurrentPlayer.Dexterity);
            
            if (attackRoll > CurrentPlayer.ArmorClass)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void MonsterAttackDamageCalculation()
        {
            int damageDealtToPlayer = RandomNumberGenerator.NumberBetween
                                          (CurrentMonster.MinimumDamage, CurrentMonster.MaximumDamage) +
                                      StatisticsCalculator.AbilityScoreCalculator(CurrentMonster.Strength);
            MonsterAttackSuccessNotification(damageDealtToPlayer);

            CurrentPlayer.TakeDamage(damageDealtToPlayer);

            if (CurrentPlayer.IsDead)
            {
                CurrentLocation = CurrentWorld.LocationAt(0, -1);
                CurrentPlayer.CompletelyHeal();
            }
        }

        private void OnCurrentMonsterKilled(object sender, System.EventArgs eventArgs)
        {
            MonsterLootPayoutNotification();

            CurrentPlayer.AddExperience(CurrentMonster.RewardExperiencePoints);
            CurrentPlayer.ReceiveGold(CurrentMonster.Gold);

            foreach(GameItem gameItem in CurrentMonster.Inventory)
            {
                CurrentPlayer.AddItemToInventory(gameItem);
            }
        }

    }
}
