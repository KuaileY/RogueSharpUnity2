using RogueSharp.DiceNotation;

public class Whetstone : Item
{
    public Whetstone()
    {
        Name = "Whetstone";
        RemainingUses = 5;
    }

    protected override bool UseItem()
    {
        Player player = Game.Player;

        if (player.Hand == HandEquipment.None())
        {
            Game.MessageLog.Add(string.Format("{0} is not holding anything they can sharpen", player.Name));
        }
        else if (player.AttackChance >= 80)
        {
            Game.MessageLog.Add(string.Format("{0} cannot make their {1} any sharper", player.Name, player.Hand.Name));
        }
        else
        {
            Game.MessageLog.Add(string.Format("{0} uses a {1} to sharper their {player.Hand.Name}", player.Name, Name));
            player.Hand.AttackChance += Dice.Roll("1D3");
            RemainingUses--;
        }

        return true;
    }
}