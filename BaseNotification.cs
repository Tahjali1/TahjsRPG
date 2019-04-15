using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Engine.EventArgs;
using Engine.Models;
using Engine.ViewModels;

namespace Engine
{
    public class BaseNotificationClass : INotifyPropertyChanged
    {
        public Player _currentPlayer;
        private Monster _currentMonster;
        public Weapon CurrentWeapon { get; set; }

        public bool HasMonster => CurrentMonster != null;

        public Player CurrentPlayer
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

        public Monster CurrentMonster
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
        
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<GameMessageEventArgs> OnMessageRaised;

        

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnCurrentPlayerLeveledUp(object sender, System.EventArgs eventArgs)
        {
            RaiseMessage($"You have leveled up!");
            RaiseMessage($"Your Strength has risen from {CurrentPlayer.Strength - 1} to {CurrentPlayer.Strength}!");
            RaiseMessage($"Your dexterity has risen from {CurrentPlayer.Dexterity - 1} to {CurrentPlayer.Dexterity}!");
        }

        protected void RaiseMessage(string message)
        {
            OnMessageRaised?.Invoke(this, new GameMessageEventArgs(message));
        }

        //Quest Messages
        //I should add some guard clauses here to check that the arguments are actual strings, and that they are separated by spaces
        //I should also add some output to the console(?) for the case when the function runs properly

        protected void QuestAcceptanceNotification(string questName, string questDescription)
        {
            RaiseMessage($"\rYou recieved the following quest: {questName}\r");
            RaiseMessage(questDescription);
        }

        protected void QuestCompletionRequirementsNotification(int questCompletionItemQuantity, 
            string questCompletionItemName)
        {
            RaiseMessage($"Return with {questCompletionItemQuantity} {questCompletionItemName} and you will " +
                         $"receive the following:");
        }

        protected void QuestCompletionGoldAndEXPNotification(int questRewardExperiencePoints,
            int questRewardGold)
        {
            RaiseMessage($"\r{questRewardExperiencePoints} experience points");
            RaiseMessage($"{questRewardGold} gold");

        }

        protected void QuestCompletionRewardItemNotification(int questItemRewardQuantity, string questItemRewardName)
        {
            RaiseMessage($"{questItemRewardQuantity} {questItemRewardName}");
        }

        protected void QuestCompletionNotification(string currentQuestName, int questRewardExperiencePoints, 
            int questRewardGold)
        {
            RaiseMessage($"\rYou completed the '{currentQuestName}' quest. " +
                         $"You receive {questRewardExperiencePoints} experience points, {questRewardGold} gold,");
        }

        protected void QuestCompletionRewardItemNotification(string rewardItemName)
        {
            RaiseMessage($"You also get the following: {rewardItemName}");
        }


        //Battle Messages
        //I should add some guard clauses here to check that the arguments are actual strings, and that they are separated by spaces
        //I should also add some output to the console(?) for the case when the function runs properly
        
        //This function should check that CurrentMonster is not null (and raise an exception if it is) before running
        protected void MonsterEncounteredNotification()
        {
            RaiseMessage($"\rA {CurrentMonster.Name} appears!");
        }
        
        protected void NullWeaponSelectionNotification()
        {
            RaiseMessage("You must select a weapon to fight with.");
            return;
        }
        
        //This function should check if CurrentMonster is not null (and raise an exception if it is) before running. 
        protected void PlayerAttackSuccessNotification(int damageDealtToMonster)
        {
            RaiseMessage($"\rYou hit {CurrentMonster.Name} for {damageDealtToMonster} points of damage!");
        }
        
        //This function should check that both CurrentWeapon != null && CurrentWeapon.Name != null before running
        protected void PlayerAttackFailureNotification()
        {
            RaiseMessage($"\rYou attack with your {CurrentWeapon.Name}, but the attack missed!");
        }

        //This function should check if CurrentMonster is not null (and raise an exception if it is) before running. 
        protected void MonsterAttackSuccessNotification(int damageDealtToPlayer)
        {
            RaiseMessage(($"{CurrentMonster.Name} lunges forward and attacks for {damageDealtToPlayer} damage!"));
        }

        //This function should check if CurrentMonster is not null (and raise an exception if it is) before running. 
        protected void MonsterAttackFailureNotification()
        {
            RaiseMessage($"\rThe {CurrentMonster.Name}'s attack missed!");
        }

        //This function should check if CurrentPlayer.CurrentHitpoints <= 0 && CurrentPlayer.IsDead = true before running
        protected void OnCurrentPlayerKilledNotification(object sender, System.EventArgs eventArgs)
        {
            RaiseMessage($"\rYour fight left you heavily wounded. You limp back to the safety of your home.");
        }

        //This function should check if CurrentMonster.CurrentHitpoints <= 0 && CurrentMonster.IsDead = true before running
        protected void OnCurrentMonsterKilledNotification(object sender, System.EventArgs eventArgs)
        {
            RaiseMessage($"\rYou defeated the enemy!");

        }

        //This function should check if CurrentMonster.RewardExperiencePoints, CurrentMonster.Gold, and CurrentMonster.RewardItems
        //are all !null before running
        protected void MonsterLootPayoutNotification()
        {
            RaiseMessage($"You receive {CurrentMonster.RewardExperiencePoints} experience points, " +
                         $"{CurrentMonster.Gold} gold, and some loot.");
        }

    }
}
