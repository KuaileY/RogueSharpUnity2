
using RogueSharp;
using UnityEngine;

public class Stairs
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsUp { get; set; }
    public GameObject go { get; private set; }

    public void Draw(IMap map)
    {
        if (!map.GetCell(X, Y).IsExplored)
        {
            return;
        }
        if (map.IsInFov(X, Y))
        {
            if (IsUp)
            {
                if (go == null)
                    go = GameObject.Instantiate<GameObject>(Game.Items["Up"], new Vector2(X, Y), Quaternion.identity);
            }
            else
            {
                if (go == null)
                    go = GameObject.Instantiate<GameObject>(Game.Items["Down"], new Vector2(X, Y), Quaternion.identity);
            }
            go.GetComponent<Renderer>().enabled = true;
        }
        else
        {
            if (IsUp)
            {
                if (go == null)
                    go = GameObject.Instantiate<GameObject>(Game.Items["Up"], new Vector2(X, Y), Quaternion.identity);
            }
            else
            {
                if (go == null)
                    go = GameObject.Instantiate<GameObject>(Game.Items["Down"], new Vector2(X, Y), Quaternion.identity);
            }
            go.GetComponent<Renderer>().enabled = false;
        }

        go.transform.SetParent(Game.itemsHolder);
    }
}
