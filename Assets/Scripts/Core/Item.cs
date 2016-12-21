

public class Item:Entry,ITreasure
{
    public string Name { get; set; }
    public int RemainingUses { get; set; }
    public bool Use()
    {
        return UseItem();
    }

    protected virtual bool UseItem()
    {
        return false;
    }


    public bool PickUp(Actor actor)
    {
        Player player = actor as Player;
        if (player != null)
        {
            if (player.AddItem(this))
            {
                Game.MessageLog.Add(string.Format("{0} picked up {1}", actor.Name, Name));
                return true;
            }
        }
        return false;
    }
}

