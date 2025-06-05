using System.Collections.Generic;
using System.Linq;

namespace PetSimulator
{
    public class ItemDatabase
    {
        private List<Item> items;

        public ItemDatabase()
        {
            InitializeItems();
        }

        private void InitializeItems()
        {
            items = new List<Item>
            {
                // Food items
                new Item("Premium Pet Food", ItemType.Food, PetStat.Hunger, 30, 2),
                new Item("Basic Pet Food", ItemType.Food, PetStat.Hunger, 15, 1),
                new Item("Gourmet Meal", ItemType.Food, PetStat.Hunger, 50, 4),
                new Item("Fish Flakes", ItemType.Food, PetStat.Hunger, 20, 1, new List<PetType> { PetType.Fish }),
                new Item("Bird Seeds", ItemType.Food, PetStat.Hunger, 25, 2, new List<PetType> { PetType.Bird }),
                new Item("Bone Treat", ItemType.Food, PetStat.Hunger, 35, 3, new List<PetType> { PetType.Dog }),
                
                // Toy items
                new Item("Ball", ItemType.Toy, PetStat.Fun, 25, 3),
                new Item("Squeaky Toy", ItemType.Toy, PetStat.Fun, 30, 2, new List<PetType> { PetType.Dog, PetType.Cat }),
                new Item("Laser Pointer", ItemType.Toy, PetStat.Fun, 40, 4, new List<PetType> { PetType.Cat }),
                new Item("Exercise Wheel", ItemType.Toy, PetStat.Fun, 35, 5, new List<PetType> { PetType.Hamster }),
                new Item("Mirror", ItemType.Toy, PetStat.Fun, 20, 2, new List<PetType> { PetType.Bird }),
                new Item("Aquarium Decoration", ItemType.Toy, PetStat.Fun, 15, 1, new List<PetType> { PetType.Fish }),
                
                // Bed items
                new Item("Soft Cushion", ItemType.Bed, PetStat.Sleep, 25, 3),
                new Item("Luxury Pet Bed", ItemType.Bed, PetStat.Sleep, 40, 5),
                new Item("Simple Blanket", ItemType.Bed, PetStat.Sleep, 15, 2),
                new Item("Bird Nest", ItemType.Bed, PetStat.Sleep, 30, 3, new List<PetType> { PetType.Bird }),
                new Item("Hamster House", ItemType.Bed, PetStat.Sleep, 35, 4, new List<PetType> { PetType.Hamster }),
                new Item("Aquarium Cave", ItemType.Bed, PetStat.Sleep, 20, 2, new List<PetType> { PetType.Fish })
            };
        }

        public List<Item> GetItems()
        {
            return items.ToList(); // Return a copy to prevent external modification
        }

        public List<Item> GetItemsByType(ItemType type)
        {
            return items.Where(i => i.Type == type).ToList();
        }

        public List<Item> GetItemsForPet(PetType petType)
        {
            return items.Where(i => i.CompatiblePets == null || i.CompatiblePets.Contains(petType)).ToList();
        }
    }
}