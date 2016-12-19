using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueSharp;
using UnityEngine;


public class Actor:Item, IScheduleable
{
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
        get { return _attack; }
        set { _attack = value; }
    }

    public int AttackChance
    {
        get { return _attackChance; }
        set { _attackChance = value; }
    }
    public int Awareness
    {
        get { return _awareness; }
        set { _awareness = value; }
    }
    public int Defense
    {
        get { return _defense; }
        set { _defense = value; }
    }
    public int DefenseChance
    {
        get { return _defenseChance; }
        set { _defenseChance = value; }
    }
    public int Gold
    {
        get { return _gold; }
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
        get { return _maxHealth; }
        set{_maxHealth = value;}
    }

    public string Name
    {
        get{return _name;}
        set{_name = value;}
    }

    public int Speed
    {
        get{return _speed;}
        set{_speed = value;}
    }

    public int X { get; set; }
    public int Y { get; set; }

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

