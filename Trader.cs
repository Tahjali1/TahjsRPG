namespace Engine.Models
{
    public class Trader : LivingEntity
    {
        public Trader(string name) : base(name, maximumHitPoints:9999, currentHitPoints:9999, strength:1, 
            dexterity:1, armorClass:9999, gold:9999, level:1)
        {
        }
    }
}