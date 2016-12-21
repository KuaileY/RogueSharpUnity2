
public class HealingPotion : Item
{
    public HealingPotion()
    {
        Name = "HealingPotion";
        RemainingUses = 1;
    }

    protected override bool UseItem()
    {
        int healAmount = 15;
        Game.MessageLog.Add(string.Format("{0} consumes a {1} and recovers {2} health", Game.Player.Name, Name, healAmount));

        Heal heal = new Heal(healAmount);

        RemainingUses--;

        return heal.Perform();
    }


}