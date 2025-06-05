using System;
using System.Collections.Generic;

namespace PetSimulator
{
    public class Menu
    {
        private string title;
        private List<string> options;

        public Menu(string title, List<string> options)
        {
            this.title = title;
            this.options = options;
        }

        public void Display()
        {
            Console.WriteLine($"\n=== {title} ===");

            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i]}");
            }

            Console.Write("\nEnter your choice: ");
        }
    }
}