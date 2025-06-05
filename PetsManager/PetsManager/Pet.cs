using System;
using System.Collections.Generic;

namespace PetSimulator
{
    public class Pet
    {
        public string Name { get; private set; }
        public PetType Type { get; private set; }
        public bool IsAlive { get; private set; }

        private Dictionary<PetStat, int> stats;

        // Events
        public event EventHandler<PetStat> StatChanged;
        public event EventHandler Died;

        public Pet(string name, PetType type)
        {
            Name = name;
            Type = type;
            IsAlive = true;

            // Initialize stats to 50
            stats = new Dictionary<PetStat, int>
            {
                { PetStat.Hunger, 50 },
                { PetStat.Sleep, 50 },
                { PetStat.Fun, 50 }
            };
        }

        public int GetStat(PetStat stat)
        {
            return stats[stat];
        }

        public void DecreaseStat(PetStat stat, int amount)
        {
            if (!IsAlive) return;

            stats[stat] = Math.Max(0, stats[stat] - amount);
            StatChanged?.Invoke(this, stat);

            CheckIfAlive();
        }

        public void IncreaseStat(PetStat stat, int amount)
        {
            if (!IsAlive) return;

            stats[stat] = Math.Min(100, stats[stat] + amount);
            StatChanged?.Invoke(this, stat);
        }

        public void UseItem(Item item)
        {
            if (!IsAlive) return;

            IncreaseStat(item.AffectedStat, item.EffectAmount);
        }

        private void CheckIfAlive()
        {
            if (stats[PetStat.Hunger] == 0 || stats[PetStat.Sleep] == 0 || stats[PetStat.Fun] == 0)
            {
                IsAlive = false;
                Died?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}