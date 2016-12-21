using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueSharp;
using UnityEngine;


public class Actor:Entry, IScheduleable
{
    public Actor()
    {
        Head = HeadEquipment.None();
        Body = BodyEquipment.None();
        Hand = HandEquipment.None();
        Feet = FeetEquipment.None();
    }

    public HeadEquipment Head { get; set; }
    public BodyEquipment Body { get; set; }
    public HandEquipment Hand { get; set; }
    public FeetEquipment Feet { get; set; }

    private int _attack;
    private int _attackChance;
    private int _awareness;
    private int _defense;
    private int _defenseChance;
    private int _gold;
    private int _health;
    private int _maxHealth;
    private string _name;
    private int _speed;

    public int Attack
    {
        get { return _attack + Head.Attack + Body.Attack + Hand.Attack + Feet.Attack; }
        set { _attack = value; }
    }

    public int AttackChance
    {
        get { return _attackChance + Head.AttackChance + Body.AttackChance + Hand.AttackChance + Feet.AttackChance; }
        set { _attackChance = value; }
    }
    public int Awareness
    {
        get { return _awareness + Head.Awareness + Body.Awareness + Hand.Awareness + Feet.Awareness; }
        set { _awareness = value; }
    }
    public int Defense
    {
        get { return _defense + Head.Defense + Body.Defense + Hand.Defense + Feet.Defense; }
        set { _defense = value; }
    }
    public int DefenseChance
    {
        get { return _defenseChance + Head.DefenseChance + Body.DefenseChance + Hand.DefenseChance + Feet.DefenseChance; }
        set { _defenseChance = value; }
    }
    public int Gold
    {
        get { return _gold + Head.Gold + Body.Gold + Hand.Gold + Feet.Gold; }
        set { _gold = value; }
    }

    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
        }
    }

    public int MaxHealth
    {
        get { return _maxHealth + Head.MaxHealth + Body.MaxHealth + Hand.MaxHealth + Feet.MaxHealth; }
        set{_maxHealth = value;}
    }

    public string Name
    {
        get{return _name;}
        set{_name = value;}
    }

    public int Speed
    {
        get{return _speed + Head.Speed + Body.Speed + Hand.Speed + Feet.Speed; }
        set{_speed = value;}
    }

    public void Draw(IMap map)
    {
        if (!map.GetCell(X, Y).IsExplored)
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

    public int Time
    {
        get
        {
            return Speed;
        }
    }

}

