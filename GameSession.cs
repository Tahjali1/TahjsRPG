using System.Linq;
using Engine.Factories;
using Engine.Models;

namespace Engine.ViewModels
{
    public class GameSession : BaseNotificationClass
    {

        #region Properties

        private Player _currentPlayer;
        private Monster _currentMonster;
        public new Weapon CurrentWeapon { get; set; }

        private Location _currentLocation;
        private Trader _currentTrader;

        public World CurrentWorld { get; }
        public BattleEngine BattleEngine { get; set; }



        
        public Location CurrentLocation
        {
            get { return _currentLocation; }
            set
            {
                _currentLocation = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(HasLocationToNorth));
                OnPropertyChanged(nameof(HasLocationToEast));
                OnPropertyChanged(nameof(HasLocationToWest));
                OnPropertyChanged(nameof(HasLocationToSouth));

                CompleteQuestsAtLocation();
                GivePlayerQuestsAtLocation();
                GetMonsterAtLocation();

                CurrentTrader = CurrentLocation.TraderHere;
            }
        }


        public Trader CurrentTrader
        {
            get { return _currentTrader; }
            set
            {
                _currentTrader = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(HasTrader));
            }
        }


        public bool HasLocationToNorth => 
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1) != null;

        public bool HasLocationToEast => 
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate) != null;

        public bool HasLocationToSouth => 
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1) != null;

        public bool HasLocationToWest => 
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate) != null;


        public bool HasTrader => CurrentTrader != null;

        #endregion

        public GameSession()
        {
            CurrentPlayer = new Player(name:"Tahj", characterClass:"Fighter", experiencePoints:0, maximumHitPoints:10, 
                currentHitPoints:10, strength:10, dexterity:10, armorClass:10, strengthAbilityScore:0, gold:10);

            if (!CurrentPlayer.Weapons.Any())
            {
                CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(1001));
            }

            CurrentWorld = WorldFactory.CreateWorld();
            CurrentLocation = CurrentWorld.LocationAt(0, 0);

            
        }


        // Location Related Functions
        public void MoveNorth()
        {
            if(HasLocationToNorth)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1);
            }
        }

        public void MoveEast()
        {
            if(HasLocationToEast)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate);
            }
        }

        public void MoveSouth()
        {
            if(HasLocationToSouth)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1);
            }
        }

        public void MoveWest()
        {
            if(HasLocationToWest)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate);
            }
        }

        // Quest Handling Functions
        private void CompleteQuestsAtLocation()
        {
            foreach(Quest quest in CurrentLocation.QuestsAvailableHere)
            {
                QuestStatus questToComplete =
                    CurrentPlayer.Quests.FirstOrDefault(q => q.PlayerQuest.ID == quest.ID &&
                                                             !q.IsCompleted);

                if(questToComplete != null)
                {
                    if(CurrentPlayer.HasAllTheseItems(quest.ItemsToComplete))
                    {
                        // Remove the quest completion items from the player's inventory
                        foreach (ItemQuantity itemQuantity in quest.ItemsToComplete)
                        {
                            for(int i = 0; i < itemQuantity.Quantity; i++)
                            {
                                CurrentPlayer.RemoveItemFromInventory(CurrentPlayer.Inventory.First(item => item.ItemTypeID == itemQuantity.ItemID));
                            }
                        }

                        //Notify Player of Quest Completion
                        QuestCompletionNotification(quest.Name, quest.RewardExperiencePoints, quest.RewardGold);

                        // Give the player the quest rewards

                        CurrentPlayer.AddExperience(quest.RewardExperiencePoints);
                        CurrentPlayer.ReceiveGold(quest.RewardGold);

                        foreach(ItemQuantity itemQuantity in quest.RewardItems)
                        {
                            GameItem rewardItem = ItemFactory.CreateGameItem(itemQuantity.ItemID);
                            QuestCompletionRewardItemNotification(rewardItem.Name);
                            CurrentPlayer.AddItemToInventory(rewardItem);
                        }
                        


                        // Mark the Quest as completed
                        questToComplete.IsCompleted = true;
                    }
                }
            }
        }

        private void GivePlayerQuestsAtLocation()
        {
            foreach(Quest quest in CurrentLocation.QuestsAvailableHere)
            {
                if(!CurrentPlayer.Quests.Any(q => q.PlayerQuest.ID == quest.ID))
                {
                    //Add quest to quest list, and give player quest description
                    CurrentPlayer.Quests.Add(new QuestStatus(quest));
                    QuestAcceptanceNotification(quest.Name, quest.Description);

                    foreach(ItemQuantity itemQuantity in quest.ItemsToComplete)
                    {
                        //Let Player know what items to bring, and in what quantity
                        QuestCompletionRequirementsNotification(itemQuantity.Quantity, ItemFactory.CreateGameItem(itemQuantity.ItemID).Name);
                    }

                    QuestCompletionGoldAndEXPNotification(quest.RewardExperiencePoints, quest.RewardGold);
                    foreach(ItemQuantity itemQuantity in quest.RewardItems)
                    {
                        QuestCompletionRewardItemNotification(itemQuantity.Quantity, 
                            ItemFactory.CreateGameItem(itemQuantity.ItemID).Name);
                    }
                }
            }
        }


        // Battle Logic Functions (This is in the Battle Engine and This Class. See if you can refactor)
        private void GetMonsterAtLocation()
        {
           CurrentMonster = CurrentLocation.GetMonster();
        }

        public void AttackCurrentMonster() // This is an On Click Event in xaml.cs
        {
            if (CurrentWeapon == null)
            {
                NullWeaponSelectionNotification();
            }

            if (BattleEngine.PlayerAccuracyCalculation())
            {
                BattleEngine.PlayerAttackDamageCalculation();
            }
            else
            {
                PlayerAttackFailureNotification();
                if (BattleEngine.MonsterAccuracyCalculation())
                {
                    BattleEngine.MonsterAttackDamageCalculation();
                }
                else
                {
                    MonsterAttackFailureNotification();
                }
            }
        }

        /*private void OnCurrentMonsterKilled(object sender, System.EventArgs eventArgs)
        {
            MonsterLootPayoutNotification();

            CurrentPlayer.AddExperience(CurrentMonster.RewardExperiencePoints);
            CurrentPlayer.ReceiveGold(CurrentMonster.Gold);

            foreach(GameItem gameItem in CurrentMonster.Inventory)
            {
                CurrentPlayer.AddItemToInventory(gameItem);
            }
        }*/
    }
}