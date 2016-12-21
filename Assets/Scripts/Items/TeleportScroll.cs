using RogueSharp;

public class TeleportScroll : Item
{
    public TeleportScroll()
    {
        Name = "TeleportScroll";
        RemainingUses = 1;
    }

    protected override bool UseItem()
    {
        DungeonMap map = Game.DungeonMap;
        Player player = Game.Player;

        Game.MessageLog.Add(string.Format("{0} uses a {1} and reappears in another place", player.Name, Name));

        Point point = map.GetRandomLocation();

        map.SetActorPosition(player, point.X, point.Y);

        RemainingUses--;

        return true;
    }
}
