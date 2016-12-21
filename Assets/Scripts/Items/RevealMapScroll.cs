
public class RevealMapScroll : Item
{
    public RevealMapScroll()
    {
        Name = "MagicMap";
        RemainingUses = 1;
    }

    protected override bool UseItem()
    {
        DungeonMap map = Game.DungeonMap;

        Game.MessageLog.Add(string.Format("{0} reads a {1} and gains knowledge of the surrounding area", Game.Player.Name, Name));

        foreach (RogueSharp.Cell cell in map.GetAllCells())
        {
            if (cell.IsWalkable)
            {
                map.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
            }
        }

        RemainingUses--;

        return true;
    }
}
