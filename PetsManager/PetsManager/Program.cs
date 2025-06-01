using System;
using System.Threading.Tasks;

namespace PetSimulator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to Pet Simulator!");
            Console.WriteLine("=========================");

            var game = new Game();
            await game.StartAsync();
        }
    }
}