using System.Collections.Generic;

namespace PetSimulator
{
    public class Item
    {
        public string Name { get; set; }
        public ItemType Type { get; set; }
        public PetStat AffectedStat { get; set; }
        public int EffectAmount { get; set; }
        public int UsageTime { get; set; } // in seconds
        public List<PetType> CompatiblePets { get; set; } // null means compatible with all

        public Item(string name, ItemType type, PetStat affectedStat, int effectAmount, int usageTime, List<PetType> compatiblePets = null)
        {
            Name = name;
            Type = type;
            AffectedStat = affectedStat;
            EffectAmount = effectAmount;
            UsageTime = usageTime;
            CompatiblePets = compatiblePets;
        }
    }
}