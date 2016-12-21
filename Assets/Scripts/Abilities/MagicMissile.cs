using RogueSharp;

public class MagicMissile : Ability, ITargetable
{
    private readonly int _attack;
    private readonly int _attackChance;

    public MagicMissile(int attack, int attackChance)
    {
        Name = "MagicMissile";
        TurnsToRefresh = 10;
        TurnsUntilRefreshed = 0;
        _attack = attack;
        _attackChance = attackChance;
    }

    protected override bool PerformAbility()
    {
        return Game.TargetingSystem.SelectMonster(this);
    }

    public void SelectTarget(Point target)
    {
        DungeonMap map = Game.DungeonMap;
        Player player = Game.Player;
        Monster monster = map.GetMonsterAt(target.X, target.Y);
        if (monster != null)
        {
            Game.MessageLog.Add(string.Format("{0} casts a {1} at {2}", player.Name, Name, monster.Name));
            Actor magicMissleActor = new Actor
            {
                Attack = _attack,
                AttackChance = _attackChance,
                Name = Name
            };
            Game.CommandSystem.Attack(magicMissleActor, monster);
        }
    }
}

