
using RogueSharp;
using UnityEngine;

public class Ability :Entry, IAbility, ITreasure
{
    public string Name { get; protected set; }

    public int TurnsToRefresh { get; protected set; }

    public int TurnsUntilRefreshed { get; protected set; }

    public bool Perform()
    {
        if (TurnsUntilRefreshed > 0)
        {
            return false;
        }

        TurnsUntilRefreshed = TurnsToRefresh;

        return PerformAbility();
    }

    protected virtual bool PerformAbility()
    {
        return false;
    }


    public void Tick()
    {
        if (TurnsUntilRefreshed > 0)
        {
            TurnsUntilRefreshed--;
        }
    }

    public bool PickUp(Actor actor)
    {
        Player player = actor as Player;

        if (player != null)
        {
            if (player.AddAbility(this))
            {
                Game.MessageLog.Add(string.Format("{0} learned the {1} ability", actor.Name, Name));
                return true;
            }
        }

        return false;
    }

    public void Draw(Map map)
    {
        if (!map.IsExplored(X, Y))
        {
            return;
        }

        if (map.IsInFov(X, Y))
        {
            if (go == null)
                go = GameObject.Instantiate<GameObject>(Game.Items[Name], new Vector2(X, Y), Quaternion.identity);
            go.GetComponent<Renderer>().enabled = true;
        }
        else
        {
            if (go == null)
                go = GameObject.Instantiate<GameObject>(Game.Items[Name], new Vector2(X, Y), Quaternion.identity);
            go.GetComponent<Renderer>().enabled = false;
        }
    }
}
