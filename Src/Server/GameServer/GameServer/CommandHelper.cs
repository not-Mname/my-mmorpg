using GameServer.Entities;
using GameServer.Managers;
using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
                        case "add_exp":
                            AddExp(int.Parse(args[1]), int.Parse(args[2]));
                            break;
                        case "show_monster_info":
                            int? mapId = args.Length > 1 ? (int?)int.Parse(args[1]) : null;
                            int? monsterId = args.Length > 2 ? (int?)int.Parse(args[2]) : null;
                            ShowMonsterInfo(mapId, monsterId);
                            break;
                        case "show_player_info":
                            int? pMapId = args.Length > 1 ? (int?)int.Parse(args[1]) : null;
                            int? playerId = args.Length > 1 ? (int?)int.Parse(args[2]) : null;
                            ShowPlayerInfo(pMapId, playerId);
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

        private static void ShowPlayerInfo(int? mapId, int? playerId)
        {
            List<Character> res = new List<Character>();
            res = CharacterManager.Instance.Characters.Values.ToList();
            if (mapId == null && playerId != null)
            {
                res = res.Where(c => c.Id == playerId).ToList();
            }
            else if (mapId != null && playerId == null)
            {
                res = res.Where(c => c.Info.mapId == mapId).ToList();
            }
            else if (mapId != null && playerId != null)
            { 
                res = res.Where(c => c.Info.mapId == mapId && c.Id == playerId).ToList();
            }

            foreach(var cha in res)
            {
                Console.WriteLine("Name: {0}, entityId: {1}, level: {2}, exp: {3}, gold: {4}, x: {5}, y: {6}, z: {7}, direction: {8}, mapId: {9}", cha.Name, cha.entityId, cha.Level, cha.Exp, cha.Gold, cha.Position.x, cha.Position.y, cha.Position.z, cha.Direction, cha.Info.mapId);
            }
        }

        private static void ShowMonsterInfo(int? mapId, int? id)
        {
            List<int> maps = new List<int>();
            List<Monster> monsters = new List<Monster>();
            if (mapId == null)// 过滤所有地图
            {
                foreach (var kv in DataManager.Instance.Maps)
                {
                    maps.Add(kv.Key);
                }
            }
            else
            {
                maps.Add((int)mapId);
            }

            if(id == null)// 过滤所有怪物
            {
                foreach(var map in maps)
                {
                    Map m = MapManager.Instance[map];
                    monsters.AddRange(m.monsterManager.monsters.Values.ToArray());
                }
            }
            else
            {
                foreach(var map in maps)
                {
                    Map m = MapManager.Instance[map];
                    if(m.monsterManager.monsters.ContainsKey((int)id))
                    {
                        monsters.Add(m.monsterManager.monsters[(int)id]);
                    }
                }
            }

            if(monsters.Count == 0)// 没有怪物
            {
                Console.WriteLine("No monsters found.");
                return;
            }

            foreach(var monster in monsters)// 显示怪物信息
            {
                Console.WriteLine("Name: {0}, entityId: {1}", monster.Name, monster.entityId);
            }
        }

        public static void Help()
        {
            Console.Write(@"
        Help:
            exit                                        Exit Game Server
            addExp              <characterId> <exp>     Add exp to character
            show_monster_info   <mapId> <monsterId>     Show monster info by mapId and monsterId
            show_player_info    <mapId> <playerId>      Show player info by playerId
            help                                        Show Help
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
