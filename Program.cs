using System;
using CharacterFactory.Models;
using CharacterFactory.Services;
using GameEnvironment.Models;
using Engine.Services;
using Logging.Services;
using Config.Services;

namespace YourNamespace
{
    class Program
    {
        static void Main(string[] args)
        {
            // Give the path to the log files
            string logDirectory = "/home/tito/Projects/EngineGame/YamlLog";

            Console.WriteLine("Hello and welcome to the game!");

            // Instantiate the ConfigLoader
            ConfigLoader configLoader = new ConfigLoader(logDirectory);

            // Instantiate the combined logger
            ILogger logger = new CombinedLogger(logDirectory);

            // Instantiate the CharacterFactory
            ICharacterFactory characterFactory = new CharacterFactory.Services.CharacterFactory(logger);

            // Use the CharacterFactory to create new characters
            Hero hero = characterFactory.CreateHero("Tito", 100, 10, 5, 0, 0);
            Monster monster = characterFactory.CreateMonster("Durax", 50, 8, 3, 5, 5);

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
            Console.Write(character is Hero ? 'H' : 'M'); // 'H' for hero, 'M' for monster
        }
    }
}
