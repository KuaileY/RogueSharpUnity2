
using System;
using UnityEngine;
using UnityEngine.UI;

public class Player : Actor
{
    public IAbility QAbility { get; set; }
    public IAbility WAbility { get; set; }
    public IAbility EAbility { get; set; }
    public IAbility RAbility { get; set; }

    public Item Item1 { get; set; }
    public Item Item2 { get; set; }
    public Item Item3 { get; set; }
    public Item Item4 { get; set; }
    public Player()
    {
        QAbility = new DoNothing();
        WAbility = new DoNothing();
        EAbility = new DoNothing();
        RAbility = new DoNothing();

        Item1 = new NoItem();
        Item2 = new NoItem();
        Item3 = new NoItem();
        Item4 = new NoItem();
    }

    public bool AddAbility(IAbility ability)
    {
        if (QAbility is DoNothing)
        {
            QAbility = ability;
        }
        else if (WAbility is DoNothing)
        {
            WAbility = ability;
        }
        else if (EAbility is DoNothing)
        {
            EAbility = ability;
        }
        else if (RAbility is DoNothing)
        {
            RAbility = ability;
        }
        else
        {
            return false;
        }

        return true;
    }

    public bool AddItem(Item item)
    {
        if (Item1 is NoItem)
        {
            Item1 = item;
        }
        else if (Item2 is NoItem)
        {
            Item2 = item;
        }
        else if (Item3 is NoItem)
        {
            Item3 = item;
        }
        else if (Item4 is NoItem)
        {
            Item4 = item;
        }
        else
        {
            return false;
        }

        return true;
    }

    public void DrawStats()
    {
        var texts = Game.PlayerStat.GetComponentsInChildren<Text>();
        foreach (var text in texts)
        {
            switch (text.gameObject.name)
            {
                case "name":
                    text.text = String.Format("Name:   {0}", Name);
                    break;
                case "health":
                    text.text = String.Format("Health: {0}/{1}", Health, MaxHealth);
                    break;
                case "attack":
                    text.text = String.Format("Attack: {0}/({1})%", Attack, AttackChance);
                    break;
                case "defense":
                    text.text = String.Format("Defense:{0}/({1})%", Defense, DefenseChance);
                    break;
                case "gold":
                    text.text = String.Format("Gold:   {0}", Gold);
                    break;

            }

        }
    }

    public void DrawInventoryE()
    {

        var texts = Game.EquipmentItems.GetComponentsInChildren<Text>();
        foreach (var text in texts)
        {
            Color color = Color.black;
            switch (text.gameObject.name)
            {
                case "head":
                    color = Head == HeadEquipment.None() ? Swatch.DbOldStone : Swatch.DbLight;
                    text.text = String.Format("Head: {0}", Head.Name);
                    text.color = color;
                    break;
                case "body":
                    color = Body == BodyEquipment.None() ? Swatch.DbOldStone : Swatch.DbLight;
                    text.text = String.Format("Body: {0}", Body.Name);
                    text.color = color;
                    break;
                case "hand":
                    color = Hand == HandEquipment.None() ? Swatch.DbOldStone : Swatch.DbLight;
                    text.text = String.Format("Hand: {0}", Hand.Name);
                    text.color = color;
                    break;
                case "feet":
                    color = Feet == FeetEquipment.None() ? Swatch.DbOldStone : Swatch.DbLight;
                    text.text = String.Format("Feet: {0}", Feet.Name);
                    text.color = color;
                    break;

            }
        }
    }

    public void DrawInventoryA()
    {
        var transforms = Game.AbilitiesItems.GetComponentsInChildren<Transform>();

        foreach (var trans in transforms)
        {
            switch (trans.gameObject.name)
            {
                case "h":
                    DrawAbility(QAbility, trans, "H");
                    break;
                case "j":
                    DrawAbility(WAbility, trans, "J");
                    break;
                case "k":
                    DrawAbility(EAbility, trans, "K");
                    break;
                case "l":
                    DrawAbility(RAbility, trans, "L");
                    break;

            }
        }
    }

    public void DrawInventoryI()
    {
        var texts = Game.ItemItems.GetComponentsInChildren<Text>();

        foreach (var text in texts)
        {
            switch (text.gameObject.name)
            {
                case "one":
                    DrawItem(Item1, text, "1");
                    break;
                case "two":
                    DrawItem(Item2, text, "2");
                    break;
                case "three":
                    DrawItem(Item3, text, "3");
                    break;
                case "four":
                    DrawItem(Item4, text, "4");
                    break;

            }
        }
    }

    private void DrawItem(Item item, Text text, string name)
    {
        Color color = item is NoItem ? Swatch.DbOldStone : Swatch.DbLight;
        text.text = String.Format("{1}-{0}", item.Name, name);
        text.color = color;
    }

    private void DrawAbility(IAbility ability, Transform trans, string name)
    {

        Color highlightTextColor = Swatch.DbOldStone;
        if (!(ability is DoNothing))
        {
            if (ability.TurnsUntilRefreshed == 0)
            {
                highlightTextColor = Swatch.DbLight;
            }
            else
            {
                highlightTextColor = Swatch.DbSkin;
            }
        }
        var text = trans.GetComponentInChildren<Text>();
        text.text = String.Format("{1}-{0}", ability.Name, name);
        text.color = highlightTextColor;

        if (ability.TurnsToRefresh > 0)
        {
            trans.GetChild(0).gameObject.SetActive(true);
            trans.GetChild(0).gameObject.GetComponentInChildren<Scrollbar>().size =
                (float)ability.TurnsUntilRefreshed / (float)ability.TurnsToRefresh;
        }
        else
        {
            trans.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void Tick()
    {
        getTick(QAbility);
        getTick(WAbility);
        getTick(EAbility);
        getTick(RAbility);
    }

    private void getTick(IAbility ablity)
    {
        if (ablity != null)
            ablity.Tick();
    }

}