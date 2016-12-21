using System;
using System.Collections.Generic;
using System.Linq;
using RogueSharp;
using RogueSharp.Random;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
    const int _width=80;
    const int _height=60;
    
    private static int _mapLevel = 1;
    private static bool _renderRequired = true;

    public static Text text { get; set; }
    public static Dictionary<string, GameObject> Items { get; set; }
    public static DungeonMap DungeonMap { get; private set; }
    public static MessageLog MessageLog { get; private set; }
    public static CommandSystem CommandSystem { get; private set; }
    public static SchedulingSystem SchedulingSystem { get; private set; }
    public static TargetingSystem TargetingSystem { get; private set; }
    public static GameObject AbilitiesItems { get; set; }
    public static GameObject MonsterStat { get; set; }
    public static GameObject MonsterItem { get; set; }
    public static GameObject PlayerStat { get; set; }
    public static GameObject EquipmentItems { get; set; }
    public static GameObject ItemItems { get; set; }
    public static Player Player { get; set; }
    public static IRandom Random { get; private set; }



    public static GameObject[,] groundTiles;
    public static Entry[,] ItemsTiles;

    public static Transform boardHolder;
    public static Transform itemsHolder;
    public static Transform effectsHolder;
    private float _lastKeyPressTime;
    public float KeyPressDelay = 0.2f;
    // Use this for initialization
    void Start ()
    {
        Load();

        Init();

        MapGenerator mapGenerator = new MapGenerator(_width, _height, 20, 13, 7, _mapLevel);
        DungeonMap = mapGenerator.CreateMap();

        DungeonMap.Draw();
    }


    // Update is called once per frame
    void Update()
    {
        bool didPlayerAct = false;
        if (Input.anyKeyDown || Time.time - _lastKeyPressTime > KeyPressDelay)
        {
            if (TargetingSystem.IsPlayerTargeting)
            {
                if (Input.anyKeyDown)
                {
                    _renderRequired = true;
                    foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
                    {
                        if (Input.GetKey(vKey))
                            TargetingSystem.HandleKey(vKey);
                    }
                }
            }
            else if (CommandSystem.IsPlayerTurn)
            {
                var playerTransform = Player.go.transform;
                _lastKeyPressTime = Time.time;
                float horizontal = Input.GetAxisRaw("Horizontal");
                float vertical = Input.GetAxisRaw("Vertical");
                if (horizontal > 0)
                {
                    playerTransform.localScale = new Vector3(1, 1, 1);
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Right);
                }
                else if (horizontal < 0)
                {
                    playerTransform.localScale = new Vector3(-1, 1, 1);
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Left);
                }
                else if (vertical > 0)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Down);
                }
                else if (vertical < 0)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Up);
                }
                else if (Input.GetKeyDown(KeyCode.Period))
                {
                    if (DungeonMap.CanMoveDownToNextLevel())
                    {
                        MapGenerator mapGenerator = new MapGenerator(_width, _height, 20, 13, 7, ++_mapLevel);
                        DungeonMap = mapGenerator.CreateMap();
                        MessageLog = new MessageLog();
                        CommandSystem = new CommandSystem();

                        didPlayerAct = true;
                    }
                }
                else
                {
                    foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
                    {
                        if (Input.GetKey(vKey))
                            didPlayerAct = CommandSystem.HandleKey(vKey);
                    }
                }


                if (didPlayerAct)
                {
                    _renderRequired = true;
                    CommandSystem.EndPlayerTurn();
                }
            }
            else
            {
                CommandSystem.ActivateMonsters();
                _renderRequired = true;
            }
        }
    }

    private void LateUpdate()
    {
        if (_renderRequired)
        {
            for (int i = 0; i < MonsterStat.transform.childCount; i++)
            {
                var go = MonsterStat.transform.GetChild(i).gameObject;
                Destroy(go);
            }

            DungeonMap.Draw();
            MessageLog.Draw();
            TargetingSystem.Draw();

            _renderRequired = false;
        }
    }

    void Load()
    {
        Items = new Dictionary<string, GameObject>();
        Items.Add("None", null);
        foreach (var item in Enum.GetValues(typeof(Res.ItemGos)))
        {
            var name = item.ToString();
            Items.Add(name, Resources.Load<GameObject>(Res.GameElementPath + name));
        }
    }

    void Init()
    {
        int seed = (int)DateTime.UtcNow.Ticks;
        Random = new DotNetRandom(seed);

        PlayerStat = GameObject.Find("playerStat");
        EquipmentItems = GameObject.Find("equipmentItems");
        AbilitiesItems = GameObject.Find("abilitiesItems");
        ItemItems = GameObject.Find("itemItems");
        MonsterStat = GameObject.Find("monsterStat");
        MonsterItem = Resources.Load<GameObject>("Prefabs/monsterItem");

        text = GameObject.Find("massageText").GetComponent<Text>();
        text.color = Color.white;

        MessageLog = new MessageLog();
        SchedulingSystem = new SchedulingSystem();
        MessageLog.Add("The rogue arrives on level 1");
        MessageLog.Add(string.Format("Level created with seed '{0}'", seed));

        groundTiles = new GameObject[_width, _height];
        ItemsTiles = new Entry[_width, _height];

        boardHolder = new GameObject("boardHolder").transform;
        itemsHolder = new GameObject("itemsHolder").transform;
        effectsHolder = new GameObject("effectsHolder").transform;

        CommandSystem = new CommandSystem();
        TargetingSystem = new TargetingSystem();
    }


    public static GameObject Floor
    {
        get { return Resources.Load<GameObject>(Res.GameElementPath + "Floor" + Random.Next(1, 8)); }    
    }

    public static GameObject Wall
    {
        get { return Resources.Load<GameObject>(Res.GameElementPath + "Wall" + Random.Next(1, 8)); }
    }

}
