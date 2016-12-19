using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueSharp.DiceNotation;
using UnityEngine;

public class CommandSystem
{
    public bool IsPlayerTurn { get; set; }

    public bool MovePlayer(Direction direction)
    {
        int x;
        int y;
        switch (direction)
        {
            case Direction.Up:
                {
                    x = Game.Player.X;
                    y = Game.Player.Y - 1;
                    break;
                }
            case Direction.Down:
                {
                    x = Game.Player.X;
                    y = Game.Player.Y + 1;
                    break;
                }
            case Direction.Left:
                {
                    x = Game.Player.X - 1;
                    y = Game.Player.Y;
                    break;
                }
            case Direction.Right:
                {
                    x = Game.Player.X + 1;
                    y = Game.Player.Y;
                    break;
                }
            default:
                {
                    return false;
                }
        }
        if (Game.DungeonMap.SetActorPosition(Game.Player, x, y))
        {
            return true;
        }
        Monster monster = Game.DungeonMap.GetMonsterAt(x, y);

        if (monster != null)
        {
            Attack(Game.Player, monster);
            return true;
        }

        return false;
    }

    public void Attack(Actor attacker, Actor defender)
    {
        StringBuilder attackMessage = new StringBuilder();
        StringBuilder defenseMessage = new StringBuilder();

        int hits = ResolveAttack(attacker, defender, attackMessage);

        int blocks = ResolveDefense(defender, hits, attackMessage, defenseMessage);

        Game.MessageLog.Add(attackMessage.ToString());
        if (!string.IsNullOrEmpty(defenseMessage.ToString()))
        {
            Game.MessageLog.Add(defenseMessage.ToString());
        }

        int damage = hits - blocks;

        ResolveDamage(defender, damage);
        attacker.go.GetComponent<Animator>().SetTrigger("Attack");
    }

    private static int ResolveAttack(Actor attacker, Actor defender, StringBuilder attackMessage)
    {
        int hits = 0;

        attackMessage.AppendFormat("{0} attacks {1} and rolls: ", attacker.Name, defender.Name);
        DiceExpression attackDice = new DiceExpression().Dice(attacker.Attack, 100);

        DiceResult attackResult = attackDice.Roll();
        foreach (TermResult termResult in attackResult.Results)
        {
            attackMessage.Append(termResult.Value + ", ");
            if (termResult.Value >= 100 - attacker.AttackChance)
            {
                hits++;
            }
        }

        return hits;
    }

    private static int ResolveDefense(Actor defender, int hits, StringBuilder attackMessage, StringBuilder defenseMessage)
    {
        int blocks = 0;

        if (hits > 0)
        {
            attackMessage.AppendFormat("scoring {0} hits.", hits);
            defenseMessage.AppendFormat("  {0} defends and rolls: ", defender.Name);
            DiceExpression defenseDice = new DiceExpression().Dice(defender.Defense, 100);

            DiceResult defenseRoll = defenseDice.Roll();
            foreach (TermResult termResult in defenseRoll.Results)
            {
                defenseMessage.Append(termResult.Value + ", ");
                if (termResult.Value >= 100 - defender.DefenseChance)
                {
                    blocks++;
                }
            }
            defenseMessage.AppendFormat("resulting in {0} blocks.", blocks);
        }
        else
        {
            attackMessage.Append("and misses completely.");
        }

        return blocks;
    }

    private static void ResolveDamage(Actor defender, int damage)
    {
        if (damage > 0)
        {
            defender.Health = defender.Health - damage;

            Game.MessageLog.Add(string.Format("  {0} was hit for {1} damage", defender.Name, damage));

            defender.go.GetComponent<Animator>().SetTrigger("Hit");
            if (defender.Health <= 0)
            {
                ResolveDeath(defender);
            }
        }
        else
        {
            Game.MessageLog.Add(string.Format("  {0} blocked all damage", defender.Name));
        }
    }

    private static void ResolveDeath(Actor defender)
    {
        if (defender is Player)
        {
            Game.MessageLog.Add(string.Format("  {0} was killed, GAME OVER MAN!", defender.Name));
        }
        else if (defender is Monster)
        {
//             if (defender.Head != null && defender.Head != HeadEquipment.None())
//             {
//                 Game.DungeonMap.AddTreasure(defender.X, defender.Y, defender.Head);
//             }
//             if (defender.Body != null && defender.Body != BodyEquipment.None())
//             {
//                 Game.DungeonMap.AddTreasure(defender.X, defender.Y, defender.Body);
//             }
//             if (defender.Hand != null && defender.Hand != HandEquipment.None())
//             {
//                 Game.DungeonMap.AddTreasure(defender.X, defender.Y, defender.Hand);
//             }
//             if (defender.Feet != null && defender.Feet != FeetEquipment.None())
//             {
//                 Game.DungeonMap.AddTreasure(defender.X, defender.Y, defender.Feet);
//             }
//             Game.DungeonMap.AddGold(defender.X, defender.Y, defender.Gold);
            Game.DungeonMap.RemoveMonster((Monster)defender);

            Game.MessageLog.Add(string.Format("  {0} died and dropped {1} gold", defender.Name, defender.Gold));
        }
    }

    public void MoveMonster(Monster monster, RogueSharp.Cell cell)
    {
        if (cell.X > monster.X)
            monster.go.transform.localScale = new Vector3(-1, 1, 1);
        else if (cell.X < monster.X)
            monster.go.transform.localScale = new Vector3(1, 1, 1);

        if (!Game.DungeonMap.SetActorPosition(monster, cell.X, cell.Y))
        {
            if (Game.Player.X == cell.X && Game.Player.Y == cell.Y)
            {
                Attack(monster, Game.Player);
            }
        }
    }

    public void ActivateMonsters()
    {
        IScheduleable scheduleable = Game.SchedulingSystem.Get();
        if (scheduleable is Player)
        {
            IsPlayerTurn = true;
            Game.SchedulingSystem.Add(Game.Player);
        }
        else
        {
            Monster monster = scheduleable as Monster;

            if (monster != null)
            {
                monster.PerformAction(this);
                Game.SchedulingSystem.Add(monster);
            }

            ActivateMonsters();
        }
    }

    public void EndPlayerTurn()
    {
        IsPlayerTurn = false;
    }

}

