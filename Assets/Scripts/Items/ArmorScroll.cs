
public class ArmorScroll : Item
{
    public ArmorScroll()
    {
        Name = "ArmorScroll";
        RemainingUses = 1;
    }

    protected override bool UseItem()
    {
        Player player = Game.Player;

        if (player.Body == BodyEquipment.None())
        {
            Game.MessageLog.Add(string.Format("{0} is not wearing any body armor to enhance", player.Name));
        }
        else if (player.Defense >= 8)
        {
            Game.MessageLog.Add(string.Format("{0} cannot enhance their {1} any more", player.Name, player.Body.Name));
        }
        else
        {
            Game.MessageLog.Add(string.Format("{0} uses a {1} to enhance their {2}", player.Name, Name, player.Body.Name));
            player.Body.Defense += 1;
            RemainingUses--;
        }

        return true;
    }
}

