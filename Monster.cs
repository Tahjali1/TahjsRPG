namespace Engine.Models
{
    public class Monster : LivingEntity
    {
        public string ImageName { get; }
        public int MinimumDamage { get; }
        public int MaximumDamage { get; }

        public int RewardExperiencePoints { get; private set; }

        public Monster(string name, string imageName,
            int maximumHitPoints, int currentHitPoints, 
            int strength, int dexterity, int minimumDamage, int maxmumDamage, 
            int armorClass, int rewardExperiencePoints, int gold) :
            base(name, maximumHitPoints, currentHitPoints, strength, dexterity, armorClass, gold)
        {
            
            ImageName = $"/Engine;component/Images/Monsters/{imageName}";
            MinimumDamage = minimumDamage;
            MaximumDamage = maxmumDamage;
            RewardExperiencePoints = rewardExperiencePoints;
        }
    }
}