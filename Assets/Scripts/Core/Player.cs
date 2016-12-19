
using System;
using UnityEngine.UI;

public class Player : Actor
{
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

}