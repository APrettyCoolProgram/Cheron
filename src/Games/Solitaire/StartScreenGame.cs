// 261101_code
// 261101_documentation

using System;
using System.Collections.Generic;

namespace Cheron.Games.Solitaire
{
    public class StartScreenGame
    {
        private List<SolitaireVariant> availableVariants = new List<SolitaireVariant> { SolitaireVariant.Klondike, SolitaireVariant.FreeCell, SolitaireVariant.Spider };
        private SolitaireVariant selectedVariant;

        public StartScreenGame()
        {
            selectedVariant = SolitaireVariant.Klondike;
        }

        public void ShowStartScreen()
        {
            Console.Clear();
            Console.WriteLine("Welcome to Solitaire!");
            Console.WriteLine("Select a variant:");

            for (int i = 0; i < availableVariants.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableVariants[i]}");
            }

            Console.Write("Enter your choice (1-" + availableVariants.Count + "): ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= availableVariants.Count)
            {
                selectedVariant = availableVariants[choice - 1];
                StartGame();
            }
            else
            {
                Console.WriteLine("Invalid choice. Please try again.");
                ShowStartScreen();
            }
        }

        private void StartGame()
        {
            Console.Clear();
            Console.WriteLine($"Starting {selectedVariant} Solitaire...");
            // Initialize and start the game based on the selected variant
            // This is a placeholder for actual game initialization logic
            Console.WriteLine("Game started. Enjoy playing!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}