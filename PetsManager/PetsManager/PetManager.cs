using System;
using System.Collections.Generic;
using System.Linq;

namespace PetSimulator
{
    public class PetManager
    {
        private List<Pet> pets;

        // Events
        public event EventHandler<Pet> PetAdded;
        public event EventHandler<Pet> PetRemoved;
        public event EventHandler<Pet> PetDied;

        public PetManager()
        {
            pets = new List<Pet>();
        }

        public void AddPet(Pet pet)
        {
            pets.Add(pet);

            // Subscribe to pet's death event
            pet.Died += OnPetDied;

            PetAdded?.Invoke(this, pet);
        }

        public void RemovePet(Pet pet)
        {
            if (pets.Remove(pet))
            {
                pet.Died -= OnPetDied;
                PetRemoved?.Invoke(this, pet);
            }
        }

        public List<Pet> GetAllPets()
        {
            return pets.Where(p => p.IsAlive).ToList();
        }

        public List<Pet> GetPetsByType(PetType type)
        {
            return pets.Where(p => p.IsAlive && p.Type == type).ToList();
        }

        public Pet GetPetByName(string name)
        {
            return pets.FirstOrDefault(p => p.IsAlive && p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public List<Pet> GetPetsNeedingAttention(PetStat stat, int threshold = 20)
        {
            return pets.Where(p => p.IsAlive && p.GetStat(stat) < threshold).ToList();
        }

        private void OnPetDied(object sender, EventArgs e)
        {
            var pet = sender as Pet;
            if (pet != null)
            {
                RemovePet(pet);
                PetDied?.Invoke(this, pet);
            }
        }
    }
}