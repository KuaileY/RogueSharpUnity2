

using RogueSharp;
using UnityEngine;

public class Gold : Entry, ITreasure
{
    public int Amount { get; set; }
    public string Name { get; set; }

    public Gold(int amount)
    {
        Amount = amount;
        Name = "Gold";
    }

    public bool PickUp(Actor actor)
    {
        actor.Gold += Amount;
        Game.MessageLog.Add(string.Format("{0} picked up {1} gold", actor.Name, Amount));
        return true;
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

