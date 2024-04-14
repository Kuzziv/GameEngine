using System;
using CharacterFactory.Models;
using CharacterFactory.Services;
using GameEnvironment.Models;
using Engine.Services;
using Logging.Services;
using Config.Services;
using GameLib.CharacterFactory.Services;
using GameLib.Lootsystem.Models;
using GameLib.Lootsystem.Services;
using System.Collections.Generic;
using System.Linq; // Added for LINQ

namespace YourNamespace
{
    class Program
    {
        static void Main(string[] args)
        {
            // Give the path to the log files
            string logDirectory = "C:\\Users\\mads-\\OneDrive\\Skrivebord\\AWSC\\GameEngine\\YamlLog\\"; // Windows

            Console.WriteLine("Hello and welcome to the game!");

            // Instantiate the ConfigLoader
            ConfigLoader configLoader = new ConfigLoader(logDirectory);

            // Instantiate the combined logger
            ILogger logger = new CombinedLogger(logDirectory);

            // Instantiate the CharacterFactory
            ICharacterFactory playerFactory = new PlayerFactory();
            ICharacterFactory npcFactory = new NpcFactory();

            // Use the CharacterFactory to create new characters
            Player hero = (Player)playerFactory.CreateCharacter("Tito", 100, 10, 5, 5, 5);
            NPC monster = (NPC)npcFactory.CreateCharacter("Goblin", 50, 5, 2, 2, 2);

            // Instantiate LootSystem
            LootSystem lootSystem = new LootSystem();

            // Create some weapons
            IItem sword = new Weapon("Sword", 50); // Added value for demonstration
            IItem axe = new Weapon("Axe", 40); // Added value for demonstration

            // Add weapons to bags
            Bag heroBag = new Bag();
            heroBag.AddItem(sword);

            Bag monsterBag = new Bag();
            monsterBag.AddItem(axe);

            // Assign bags to characters
            hero.Inventory = heroBag;
            monster.Inventory = monsterBag;

            // Output information about the created characters
            World world = new World(25, 25);
            world.GenerateTerrain();

            // Temporary game service until we move it
            GameService gameService = new GameService(world, hero, monster, logger);

            // Test LINQ methods
            TestLINQ(heroBag, monsterBag);

            while (true)
            {
                Console.Clear();
                world.RenderTerrain();
                PrintCharacter(hero);
                PrintCharacter(monster);

                ConsoleKeyInfo keyInfo = Console.ReadKey();

                gameService.MovePlayer(keyInfo.KeyChar);

                gameService.EnemyTurn();

                // Log a trace message (optional)
                logger.LogInformation("Game loop iteration complete.");
            }
        }

        static void PrintCharacter(CharacterBase character)
        {
            Console.SetCursorPosition(character.X * 3 + 1, character.Y + 1); // Adjust position for spacing
            Console.ForegroundColor = ConsoleColor.White; // Character color
            Console.Write(character is Player ? 'H' : 'M'); // 'H' for hero, 'M' for monster
        }

        static void TestLINQ(Bag heroBag, Bag monsterBag)
        {
            // Test LINQ methods for querying items in the bag
            Console.WriteLine("Items in hero's bag:");
            foreach (var item in heroBag.GetItems())
            {
                Console.WriteLine($"Name: {item.Name}, Value: {((Weapon)item).Value}"); // Casting to Weapon for demonstration
            }

            // Test LINQ methods for querying bags in the loot system
            List<Bag> bags = new List<Bag> { heroBag, monsterBag };

            Console.WriteLine("\nBags containing a Sword:");
            var bagsWithSword = bags.Where(bag => bag.GetItems().Any(item => item.Name == "Sword"));
            foreach (var bag in bagsWithSword)
            {
                Console.WriteLine($"Bag contains a Sword: {bag.GetItems().Any(item => item.Name == "Sword")}");
            }

            Console.WriteLine("\nBag with the highest total value:");
            var bagWithHighestValue = bags.OrderByDescending(bag => bag.GetTotalValue()).FirstOrDefault();
            if (bagWithHighestValue != null)
            {
                Console.WriteLine($"Bag with the highest value: {bagWithHighestValue.GetTotalValue()}");
            }

            Console.WriteLine("\nBag with the most items:");
            var bagWithMostItems = bags.OrderByDescending(bag => bag.GetItems().Count()).FirstOrDefault();
            if (bagWithMostItems != null)
            {
                Console.WriteLine($"Bag with the most items: {bagWithMostItems.GetItems().Count()}");
            }
        }
    }
}
