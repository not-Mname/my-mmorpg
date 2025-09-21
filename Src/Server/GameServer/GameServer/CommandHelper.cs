using GameServer.Managers;
using System;

namespace GameServer
{
    class CommandHelper
    {
        public static void Run()
        {
            bool run = true;
            while (run)
            {
                Console.Write(">");
                try
                {
                    string line = Console.ReadLine().ToLower().Trim();
                    string[] args = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    switch (args[0])
                    {
                        case "exit":
                            run = false;
                            break;
                        case "addExp":
                            AddExp(int.Parse(args[1]), int.Parse(args[2]));
                            break;
                        default:
                            Help();
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public static void Help()
        {
            Console.Write(@"
        Help:
            exit                                Exit Game Server
            addExp <characterId> <exp>          Add exp to character
            help                                Show Help
        ");
        }

        public static void AddExp(int characterId, int exp)
        {
            var character = CharacterManager.Instance.GetCharacter(characterId);
            if (character == null)
            {
                Console.WriteLine("Character {0} not found.", characterId);
                return;
            }
            character.AddExp(exp);
        }
    }
}
