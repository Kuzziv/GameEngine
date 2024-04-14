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

namespace YourNamespace
{
    class Program
    {
        static void Main(string[] args)
        {
            // Give the path to the log files
            //string logDirectory = "/home/tito/Projects/EngineGame/YamlLog"; // Linux
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
            IItem sword = new Weapon("Sword");
            IItem axe = new Weapon("Axe");

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
    }
}
