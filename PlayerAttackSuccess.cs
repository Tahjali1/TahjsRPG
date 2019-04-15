using Engine.Models;
using Engine;
using Engine.Factories;
using Engine.ViewModels;

namespace Engine.ViewModels
{
    public class PlayerAttackSuccess
    {
        private GameSession _gameSession;

        public PlayerAttackSuccess(GameSession gameSession)
        {
            _gameSession = gameSession;
        }

        public void AttackSuccess()
        {
            int weaponDamage =
                RandomNumberGenerator.NumberBetween(_gameSession.CurrentWeapon.MinimumDamage, _gameSession.CurrentWeapon.MaximumDamage);
            int damageDealtToMonster = weaponDamage + StatisticsCalculator.AbilityScoreCalculator(_gameSession.CurrentPlayer.Strength);

            _gameSession.PlayerAttackSuccessNotification(_gameSession.CurrentMonster.Name, damageDealtToMonster);
            _gameSession.CurrentMonster.TakeDamage(damageDealtToMonster);
        }

        public void AttackCurrentMonster() // This is an On Click Event in xaml.cs
        {
            if (_gameSession.CurrentWeapon == null)
            {
                _gameSession.NullWeaponSelectionNotification();
            }

            //Initiative Calculation (Happens each round)
            int PlayerInitiative = RandomNumberGenerator.NumberBetween(0, 20) +
                                   StatisticsCalculator.AbilityScoreCalculator(_gameSession.CurrentPlayer.Dexterity);
            int monsterInitiative = RandomNumberGenerator.NumberBetween(0, 20) +
                                    StatisticsCalculator.AbilityScoreCalculator(_gameSession.CurrentMonster.Dexterity);


            
            //Player's Accuracy Calculations

            _gameSession.CurrentPlayer.ToHit(RandomNumberGenerator.NumberBetween(0,20) + StatisticsCalculator.AbilityScoreCalculator(_gameSession.CurrentPlayer.Dexterity), _gameSession.CurrentMonster.ArmorClass);

            // The Condition where the player can hit monster

            if (_gameSession.CurrentPlayer.AttackSuccess)
            {
                
                ViewModels.AttackSuccess();

                if (_gameSession.CurrentMonster.IsDead)
                {
                    _gameSession.CurrentPlayer.AddExperience(_gameSession.CurrentMonster.RewardExperiencePoints);
                    _gameSession.CurrentPlayer.ReceiveGold(_gameSession.CurrentMonster.Gold);
                    foreach(GameItem gameItem in _gameSession.CurrentMonster.Inventory)
                    {
                        _gameSession.CurrentPlayer.AddItemToInventory(gameItem);
                    }

                    _gameSession.GetMonsterAtLocation();
                    return;
                }
                else
                {
                    _gameSession.MonsterAttackSuccess();
                }
            }
            else
            {
                _gameSession.PlayerAttackFailureNotification(_gameSession.CurrentWeapon.Name);

                _gameSession.CurrentMonster.ToHit(RandomNumberGenerator.NumberBetween(0, 20), _gameSession.CurrentPlayer.ArmorClass);

                if (_gameSession.CurrentMonster.AttackSuccess)
                {
                    _gameSession.MonsterAttackSuccess();
                }
                else
                {
                    _gameSession.MonsterAttackFailureNotification(_gameSession.CurrentMonster.Name);
                }
            }
        }
    }
}