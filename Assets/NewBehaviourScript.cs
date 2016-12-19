using System;
using System.Collections.Generic;
using System.Linq;
using RogueSharp;
using UnityEngine;
using Random = UnityEngine.Random;

public class NewBehaviourScript : MonoBehaviour
{
    const int _width=60;
    const int _height=45;
    private List<Rectangle> Rooms;
    public List<Item> Doors;
    private static bool _renderRequired = true;

    private Map GroundMap { get; set; }
    private Map ItemsMap { get; set; }
    

    GameObject Floor;
    GameObject Wall;
    GameObject _door;
    GameObject Player;
    GameObject[,] groundTiles;
    Item[,] ItemsTiles;

    Transform boardHolder;
    Transform itemsHolder;
    private float _lastKeyPressTime;
    public float KeyPressDelay = 0.2f;
    // Use this for initialization
    void Start ()
    {

        Init();
        CreateMap();

        UpdatePlayerFieldOfView();

        DrawGround();
        DrawItems();
    }



    void Init()
    {
        Rooms = new List<Rectangle>();
        Doors = new List<Item>();

        GroundMap = new Map(_width, _height);
        ItemsMap = new Map(_width, _height);
        groundTiles = new GameObject[_width, _height];
        ItemsTiles = new Item[_width, _height];

        Floor = Resources.Load<GameObject>(Res.GameElementPath + "Floor1");
        Wall = Resources.Load<GameObject>(Res.GameElementPath + "Wall1");
        _door = Resources.Load<GameObject>(Res.GameElementPath + "Door");


        Player = GameObject.Find("Player");

        boardHolder = new GameObject("boardHolder").transform;
        itemsHolder = new GameObject("itemsHolder").transform;
    }

    void CreateMap()
    {
        for (int i = 0; i < 20; i++)
        {
            int roomWidth = Random.Range(5, 7);
            int roomHeight = Random.Range(5, 7);
            int roomXPosition = Random.Range(0, 29- roomWidth);
            int roomYPosition = Random.Range(0, 29- roomHeight);

            var newRoom = new Rectangle(roomXPosition, roomYPosition, roomWidth, roomHeight);
            bool newRoomIntersects = Rooms.Any(room => newRoom.Intersects(room));
            if (!newRoomIntersects)
            {
                Rooms.Add(newRoom);
            }
        }

        foreach (var room in Rooms)
        {
            CreateRoom(room);
        }

        for (int r = 0; r < Rooms.Count; r++)
        {
            if (r == 0)
            {
                continue;
            }

            int previousRoomCenterX = Rooms[r - 1].Center.X;
            int previousRoomCenterY = Rooms[r - 1].Center.Y;
            int currentRoomCenterX = Rooms[r].Center.X;
            int currentRoomCenterY = Rooms[r].Center.Y;

            if (Random.Range(0, 2) == 0)
            {
                CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, previousRoomCenterY);
                CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, currentRoomCenterX);
            }
            else
            {
                CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, previousRoomCenterX);
                CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, currentRoomCenterY);
            }
        }

        foreach (Rectangle room in Rooms)
        {
            CreateDoors(room);
        }

        PlacePlayer();
    }

    void PlacePlayer()
    {
        Player.transform.position = new Vector2(Rooms[0].Center.X, Rooms[0].Center.Y);
        UpdatePlayerFieldOfView();
    }

    void CreateRoom(Rectangle room)
    {
        for (int x = room.Left + 1; x < room.Right; x++)
        {
            for (int y = room.Top + 1; y < room.Bottom; y++)
            {
                GroundMap.SetCellProperties(x, y, true, true);
                ItemsMap.SetCellProperties(x, y, true, true);
            }
        }
    }

    private void CreateDoors(Rectangle room)
    {
        int xMin = room.Left;
        int xMax = room.Right;
        int yMin = room.Top;
        int yMax = room.Bottom;

        List<Cell> borderCells = ItemsMap.GetCellsAlongLine(xMin, yMin, xMax, yMin).ToList();
        borderCells.AddRange(ItemsMap.GetCellsAlongLine(xMin, yMin, xMin, yMax));
        borderCells.AddRange(ItemsMap.GetCellsAlongLine(xMin, yMax, xMax, yMax));
        borderCells.AddRange(ItemsMap.GetCellsAlongLine(xMax, yMin, xMax, yMax));

        foreach (Cell cell in borderCells)
        {
            if (IsPotentialDoor(cell))
            {
                GroundMap.SetCellProperties(cell.X, cell.Y, false, true);
                ItemsMap.SetCellProperties(cell.X, cell.Y, false, true);
                var door = new Item();
                var instance = Instantiate<GameObject>(_door, new Vector2(cell.X, cell.Y), Quaternion.identity);
                door.Stat = "close";
                door.go = instance;
                instance.transform.SetParent(itemsHolder);
                instance.GetComponent<Renderer>().enabled = false;
                ItemsTiles[cell.X, cell.Y] = door;
                Doors.Add(door);
            }
        }
    }

    private void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition)
    {
        for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
        {
            GroundMap.SetCellProperties(x, yPosition, true, true);
            ItemsMap.SetCellProperties(x, yPosition, true, true);
        }
    }

    private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
    {
        for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
        {
            GroundMap.SetCellProperties(xPosition, y, true, true);
            ItemsMap.SetCellProperties(xPosition, y, true, true);
        }
    }

    void UpdatePlayerFieldOfView()
    {
        GroundMap.ComputeFov((int)Player.transform.position.x, (int)Player.transform.position.y, 8,true);
        ItemsMap.ComputeFov((int)Player.transform.position.x, (int)Player.transform.position.y, 8,true);

        foreach (var cell in GroundMap.GetAllCells())
        {
            if (GroundMap.IsInFov(cell.X, cell.Y))
            {
                GroundMap.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
            }
        }

        foreach (var cell in ItemsMap.GetAllCells())
        {
            if (ItemsMap.IsInFov(cell.X, cell.Y))
            {
                ItemsMap.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
            }
        }
    }

    void DrawGround()
    {
        foreach (var cell in GroundMap.GetAllCells())
        {
            if (!cell.IsExplored)
                continue;

            GameObject tileType;
            if (GroundMap.IsInFov(cell.X, cell.Y))
            {
                if (cell.IsWalkable)
                    tileType = Floor;
                else
                    tileType = Wall;
                if (groundTiles[cell.X, cell.Y] == null)
                {
                    GameObject instance = GameObject.Instantiate<GameObject>(tileType, new Vector2(cell.X, cell.Y),
                        Quaternion.identity);
                    instance.transform.SetParent(boardHolder);
                    groundTiles[cell.X, cell.Y] = instance;
                }
                groundTiles[cell.X, cell.Y].GetComponent<Renderer>().material.color = Color.white;
            }
            else
            {
                if (cell.IsWalkable)
                    tileType = Floor;
                else
                    tileType = Wall;
                if (groundTiles[cell.X, cell.Y] == null)
                {
                    GameObject instance = GameObject.Instantiate<GameObject>(tileType, new Vector2(cell.X, cell.Y),
                        Quaternion.identity);
                    instance.transform.SetParent(boardHolder);
                    groundTiles[cell.X, cell.Y] = instance;
                }
                groundTiles[cell.X, cell.Y].GetComponent<Renderer>().material.color = Color.gray;
            }
        }
    }

    void DrawItems()
    {
        foreach (var cell in ItemsMap.GetAllCells())
        {
            if (!cell.IsExplored)
                continue;

            if (ItemsMap.IsInFov(cell.X, cell.Y))
            {
                if (ItemsTiles[cell.X, cell.Y] != null)
                    ItemsTiles[cell.X, cell.Y].go.GetComponent<Renderer>().enabled = true;
            }
            else
            {
                if (ItemsTiles[cell.X, cell.Y] != null)
                    ItemsTiles[cell.X, cell.Y].go.GetComponent<Renderer>().enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool didPlayerAct = false;
        if (Input.anyKeyDown || Time.time - _lastKeyPressTime > KeyPressDelay)
        {
            var playerTransform = Player.transform;
            _lastKeyPressTime = Time.time;
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            if (horizontal > 0)
            {
                playerTransform.localScale = new Vector3(1, 1, 1);
                didPlayerAct = MovePlayer(Direction.Right);
            }
            else if (horizontal < 0)
            {
                playerTransform.localScale = new Vector3(-1, 1, 1);
                didPlayerAct = MovePlayer(Direction.Left);
            }
            else if (vertical > 0)
            {
                didPlayerAct = MovePlayer(Direction.Down);
            }
            else if (vertical < 0)
            {
                didPlayerAct = MovePlayer(Direction.Up);
            }

            if (didPlayerAct)
            {
                _renderRequired = true;
            }
        }
    }

    private void LateUpdate()
    {
        if (_renderRequired)
        {
            DrawGround();
            DrawItems();
            _renderRequired = false;
        }
    }

    public bool MovePlayer(Direction direction)
    {
        int x;
        int y;
        var pos = Player.transform.position;
        switch (direction)
        {
            case Direction.Up:
                {
                    x = (int)pos.x;
                    y = (int)pos.y - 1;
                    break;
                }
            case Direction.Down:
                {
                    x = (int)pos.x;
                    y = (int)pos.y + 1;
                    break;
                }
            case Direction.Left:
                {
                    x = (int)pos.x - 1;
                    y = (int)pos.y;
                    break;
                }
            case Direction.Right:
                {
                    x = (int)pos.x + 1;
                    y = (int)pos.y;
                    break;
                }
            default:
                {
                    return false;
                }
        }
        if (SetActorPosition(Player, x, y))
        {
            return true;
        }
        return false;
    }

    bool SetActorPosition(GameObject actor, int x, int y)
    {
        if (ItemsMap.GetCell(x, y).IsWalkable)
        {
            SetIsWalkable((int)actor.transform.position.x, (int)actor.transform.position.y, true);
            actor.transform.position = new Vector2(x, y);
            SetIsWalkable((int)actor.transform.position.x, (int)actor.transform.position.y, false);
            OpenDoor(actor, x, y);
            if (actor.name == "Player")
            {
                UpdatePlayerFieldOfView();
            }
            return true;
        }
        return false;
    }

    void SetIsWalkable(int x, int y, bool isWalkable)
    {
        var cell = ItemsMap.GetCell(x, y);
        ItemsMap.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
    }

    bool IsPotentialDoor(Cell cell)
    {
        if (!cell.IsWalkable)
        {
            return false;
        }

        Cell right = GroundMap.GetCell(cell.X + 1, cell.Y);
        Cell left = GroundMap.GetCell(cell.X - 1, cell.Y);
        Cell top = GroundMap.GetCell(cell.X, cell.Y - 1);
        Cell bottom = GroundMap.GetCell(cell.X, cell.Y + 1);

        if (GetDoor(cell.X, cell.Y) != null ||
            GetDoor(right.X, right.Y) != null ||
            GetDoor(left.X, left.Y) != null ||
            GetDoor(top.X, top.Y) != null ||
            GetDoor(bottom.X, bottom.Y) != null)
        {
            return false;
        }

        if (right.IsWalkable && left.IsWalkable && !top.IsWalkable && !bottom.IsWalkable)
        {
            return true;
        }
        if (!right.IsWalkable && !left.IsWalkable && top.IsWalkable && bottom.IsWalkable)
        {
            return true;
        }
        return false;
    }

    public Item GetDoor(int x, int y)
    {
        return Doors.SingleOrDefault(d => d.go.transform.position.x == x && d.go.transform.position.y == y);
    }

    void OpenDoor(GameObject actor, int x, int y)
    {
        var door = GetDoor(x, y);
        if (door != null && door.Stat == "close")
        {
            Debug.Log("cat");
            door.Stat = "open";
            var ItemCell = ItemsMap.GetCell(x, y);
            var GroundCell = ItemsMap.GetCell(x, y);
            GroundMap.SetCellProperties(x, y, true, true, GroundCell.IsExplored);
            ItemsMap.SetCellProperties(x, y, true, true, ItemCell.IsExplored);
            door.go.GetComponent<Animator>().SetTrigger("Open");
        }
    }

    public class Item
    {
        public GameObject go;
        public string Stat;
    }

}
