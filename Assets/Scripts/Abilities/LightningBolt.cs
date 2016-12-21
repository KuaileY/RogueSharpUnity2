using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueSharp;

public class LightningBolt : Ability, ITargetable
{
    private readonly int _attack;
    private readonly int _attackChance;

    public LightningBolt(int attack, int attackChance)
    {
        Name = "LightningBolt";
        TurnsToRefresh = 40;
        TurnsUntilRefreshed = 0;
        _attack = attack;
        _attackChance = attackChance;
    }

    protected override bool PerformAbility()
    {
        return Game.TargetingSystem.SelectLine(this);
    }

    public void SelectTarget(Point target)
    {
        DungeonMap map = Game.DungeonMap;
        Player player = Game.Player;
        Game.MessageLog.Add(string.Format("{0} casts a {1}", player.Name, Name));

        Actor lightningBoltActor = new Actor
        {
            Attack = _attack,
            AttackChance = _attackChance,
            Name = Name
        };
        foreach (RogueSharp.Cell cell in map.GetCellsAlongLine(player.X, player.Y, target.X, target.Y))
        {
            if (cell.IsWalkable)
            {
                continue;
            }

            if (cell.X == player.X && cell.Y == player.Y)
            {
                continue;
            }

            Monster monster = map.GetMonsterAt(cell.X, cell.Y);
            if (monster != null)
            {
                Game.CommandSystem.Attack(lightningBoltActor, monster);
            }
            else
            {
                // We hit a wall so stop the bolt
                // TODO: consider having bolts and fireballs destroy walls and leave rubble
                return;
            }
        }
    }
}