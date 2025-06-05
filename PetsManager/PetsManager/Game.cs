using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PetSimulator
{
    public class Game
    {
        private bool isRunning;
        private PetManager petManager;
        private ItemDatabase itemDatabase;
        private CancellationTokenSource cancellationTokenSource;

        // Events
        public event EventHandler<string> CommandExecuted;
        public event EventHandler<Pet> PetAdopted;
        public event EventHandler<Pet> PetDied;

        public Game()
        {
            petManager = new PetManager();
            itemDatabase = new ItemDatabase();
            cancellationTokenSource = new CancellationTokenSource();

            // Subscribe to pet manager events
            petManager.PetDied += OnPetDied;
        }

        public async Task StartAsync()
        {
            isRunning = true;

            // Start the background task for updating pet stats
            var updateTask = Task.Run(() => UpdatePetStatsAsync(cancellationTokenSource.Token));

            await RunGameLoopAsync();

            // Cancel background tasks when game ends
            cancellationTokenSource.Cancel();
            await updateTask;
        }

        private async Task RunGameLoopAsync()
        {
            while (isRunning)
            {
                ShowMainMenu();
                var choice = Console.ReadLine();
                await ProcessCommandAsync(choice);
            }
        }

        private void ShowMainMenu()
        {
            var mainMenu = new Menu("Main Menu", new List<string>
            {
                "Adopt a Pet",
                "View All Pets",
                "Feed Pet",
                "Play with Pet",
                "Put Pet to Sleep",
                "View Items",
                "About (Creator Info)",
                "Exit Game"
            });

            mainMenu.Display();
        }

        private async Task ProcessCommandAsync(string choice)
        {
            switch (choice)
            {
                case "1":
                    await AdoptPetAsync();
                    break;
                case "2":
                    ViewAllPets();
                    break;
                case "3":
                    await UseItemAsync(ItemType.Food);
                    break;
                case "4":
                    await UseItemAsync(ItemType.Toy);
                    break;
                case "5":
                    await UseItemAsync(ItemType.Bed);
                    break;
                case "6":
                    ViewItems();
                    break;
                case "7":
                    ShowAbout();
                    break;
                case "8":
                    ExitGame();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }

            CommandExecuted?.Invoke(this, $"Command {choice} executed");

            if (isRunning)
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private async Task AdoptPetAsync()
        {
            var petTypeMenu = new Menu("Choose Pet Type", Enum.GetNames(typeof(PetType)).ToList());
            petTypeMenu.Display();

            if (int.TryParse(Console.ReadLine(), out int typeChoice) &&
                typeChoice > 0 && typeChoice <= Enum.GetValues(typeof(PetType)).Length)
            {
                Console.Write("Enter a name for your pet: ");
                string name = Console.ReadLine();

                var petType = (PetType)(typeChoice - 1);
                var pet = new Pet(name, petType);

                petManager.AddPet(pet);
                PetAdopted?.Invoke(this, pet);

                Console.WriteLine($"\n{name} the {petType} has been adopted!");
                Console.WriteLine("Starting stats: Hunger: 50, Sleep: 50, Fun: 50");
            }
            else
            {
                Console.WriteLine("Invalid pet type selection.");
            }
        }

        private void ViewAllPets()
        {
            var pets = petManager.GetAllPets();

            if (!pets.Any())
            {
                Console.WriteLine("You don't have any pets yet!");
                return;
            }

            Console.WriteLine("\n=== Your Pets ===");
            foreach (var pet in pets)
            {
                Console.WriteLine($"\n{pet.Name} the {pet.Type}:");
                Console.WriteLine($"  Hunger: {pet.GetStat(PetStat.Hunger)}/100");
                Console.WriteLine($"  Sleep: {pet.GetStat(PetStat.Sleep)}/100");
                Console.WriteLine($"  Fun: {pet.GetStat(PetStat.Fun)}/100");
            }
        }

        private async Task UseItemAsync(ItemType itemType)
        {
            var pets = petManager.GetAllPets();

            if (!pets.Any())
            {
                Console.WriteLine("You don't have any pets to use items on!");
                return;
            }

            // Show pet selection menu
            var petNames = pets.Select(p => $"{p.Name} the {p.Type}").ToList();
            var petMenu = new Menu("Select a Pet", petNames);
            petMenu.Display();

            if (int.TryParse(Console.ReadLine(), out int petChoice) &&
                petChoice > 0 && petChoice <= pets.Count)
            {
                var selectedPet = pets[petChoice - 1];

                // Get available items for this pet type
                var availableItems = itemDatabase.GetItems()
                    .Where(i => i.Type == itemType &&
                               (i.CompatiblePets == null || i.CompatiblePets.Contains(selectedPet.Type)))
                    .ToList();

                if (!availableItems.Any())
                {
                    Console.WriteLine($"No {itemType} items available for {selectedPet.Type}!");
                    return;
                }

                // Show item selection menu
                var itemNames = availableItems.Select(i => i.Name).ToList();
                var itemMenu = new Menu($"Select {itemType}", itemNames);
                itemMenu.Display();

                if (int.TryParse(Console.ReadLine(), out int itemChoice) &&
                    itemChoice > 0 && itemChoice <= availableItems.Count)
                {
                    var selectedItem = availableItems[itemChoice - 1];

                    Console.WriteLine($"\nUsing {selectedItem.Name} on {selectedPet.Name}...");
                    Console.WriteLine($"This will take {selectedItem.UsageTime} seconds.");

                    // Apply item effect
                    selectedPet.UseItem(selectedItem);

                    // Wait for item usage time
                    await Task.Delay(selectedItem.UsageTime * 1000);

                    Console.WriteLine($"Done! {selectedPet.Name}'s stats have been updated.");
                }
            }
        }

        private void ViewItems()
        {
            var items = itemDatabase.GetItems();

            Console.WriteLine("\n=== Available Items ===");

            foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
            {
                Console.WriteLine($"\n{itemType}:");
                var typeItems = items.Where(i => i.Type == itemType);

                foreach (var item in typeItems)
                {
                    Console.WriteLine($"  - {item.Name}");
                    Console.WriteLine($"    Effect: +{item.EffectAmount} {item.AffectedStat}");
                    Console.WriteLine($"    Usage Time: {item.UsageTime} seconds");

                    if (item.CompatiblePets != null && item.CompatiblePets.Any())
                    {
                        Console.WriteLine($"    Compatible with: {string.Join(", ", item.CompatiblePets)}");
                    }
                }
            }
        }

        private void ShowAbout()
        {
            Console.WriteLine("\n=== About Pet Simulator ===");
            Console.WriteLine("Created by: [Ali AlMashukhi]");
            Console.WriteLine("Student Number: [2305045030]");
            Console.WriteLine("A console-based pet care simulator");
            Console.WriteLine("Version 1.0");
        }

        private void ExitGame()
        {
            Console.WriteLine("\nThanks for playing Pet Simulator!");
            Console.WriteLine("Goodbye!");
            isRunning = false;
        }

        private async Task UpdatePetStatsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(3000); // Update every 3 seconds

                var pets = petManager.GetAllPets();
                foreach (var pet in pets)
                {
                    pet.DecreaseStat(PetStat.Hunger, 1);
                    pet.DecreaseStat(PetStat.Sleep, 1);
                    pet.DecreaseStat(PetStat.Fun, 1);
                }
            }
        }

        private void OnPetDied(object sender, Pet pet)
        {
            Console.WriteLine($"\n\n!!! ALERT: {pet.Name} the {pet.Type} has died! !!!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            PetDied?.Invoke(this, pet);
        }
    }
}