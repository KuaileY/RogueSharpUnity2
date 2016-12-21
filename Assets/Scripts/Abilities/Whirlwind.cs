using System.Collections.Generic;
using RogueSharp;

public class Whirlwind : Ability
{
    public Whirlwind()
    {
        Name = "Whirlwind";
        TurnsToRefresh = 20;
        TurnsUntilRefreshed = 0;
    }

    protected override bool PerformAbility()
    {
        DungeonMap map = Game.DungeonMap;
        Player player = Game.Player;

        Game.MessageLog.Add(string.Format("{0} performs a whirlwind attack against all adjacent enemies", player.Name));

        List<Point> monsterLocations = new List<Point>();

        foreach (RogueSharp.Cell cell in map.GetCellsInArea(player.X, player.Y, 1))
        {
            foreach (Point monsterLocation in map.GetMonsterLocations())
            {
                if (cell.X == monsterLocation.X && cell.Y == monsterLocation.Y)
                {
                    monsterLocations.Add(monsterLocation);
                }
            }
        }

        foreach (Point monsterLocation in monsterLocations)
        {
            Monster monster = map.GetMonsterAt(monsterLocation.X, monsterLocation.Y);
            if (monster != null)
            {
                Game.CommandSystem.Attack(player, monster);
            }
        }

        return true;
    }
}

