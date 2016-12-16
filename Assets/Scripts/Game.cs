using System;
using System.Collections.Generic;
using System.Linq;
using RogueSharp;
using RogueSharp.MapCreation;
using RogueSharp.Random;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static readonly int BoardWidth = 40;
    public static readonly int BoardHeight = 30;
    public static readonly int _maxRooms = 15;
    public static readonly int _roomMaxSize = 7;
    public static readonly int _roomMinSize = 5;
    private float _lastKeyPressTime;

    public float KeyPressDelay = 0.2f;
    public GameObject Floor;
    public GameObject Wall;

    public Map Map;
    public GameObject[,] Tiles;
    public List<Rectangle> Rooms;

    public static IRandom Random { get; private set; }
    public static GameObject Player { get; set; }
    // Use this for initialization
    void Start ()
	{
        int seed = (int)DateTime.UtcNow.Ticks;
        Random = new DotNetRandom(seed);
        Rooms = new List<RogueSharp.Rectangle>();
        Player = GameObject.Find("Player");
        Tiles = new GameObject[BoardWidth, BoardHeight];

        CreateMap();
        
        Draw();
        _lastKeyPressTime = Time.time;
    }

	void Update () {
        Transform playerTransform = Player.transform;
	    if (Input.anyKeyDown || Time.time - _lastKeyPressTime > KeyPressDelay)
	    {
            _lastKeyPressTime = Time.time;
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

	        if (horizontal > 0)
	        {
                playerTransform.localScale = new Vector3(1, 1, 1);
                var pos = playerTransform.position + new Vector3(1, 0, 0);
                SetActorPosition(playerTransform, pos);
            }
            else if (horizontal < 0)
            {
                playerTransform.localScale = new Vector3(-1, 1, 1);
                var pos= playerTransform.position + new Vector3(-1, 0, 0);
                SetActorPosition(playerTransform, pos);
            }
            else if (vertical > 0)
            {
                var pos = playerTransform.position + new Vector3(0, 1, 0);
                SetActorPosition(playerTransform, pos);
            }
            else if (vertical < 0)
            {
                var pos=playerTransform.position +new Vector3(0, -1, 0);
                SetActorPosition(playerTransform, pos);
            }
        }
    }

    public void SetActorPosition(Transform actor, Vector3 pos)
    {
        if (Map.GetCell((int)pos.x, (int)pos.y).IsWalkable)
        {
            SetIsWalkable((int)actor.position.x, (int)actor.position.y, true);
            actor.position = pos;
            SetIsWalkable((int)actor.position.x, (int)actor.position.y, false);

            if (actor == Player.transform)
                PlacePlayer();
        }
    }

    public void SetIsWalkable(int x, int y, bool isWalkable)
    {
        Cell cell = Map.GetCell(x, y);
        Map.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
    }

    private void CreateRoom(Rectangle room)
    {
        for (int x = room.Left + 1; x < room.Right; x++)
        {
            for (int y = room.Top + 1; y < room.Bottom; y++)
            {
                Map.SetCellProperties(x, y, true, true, true);
            }
        }
    }

    void CreateMap()
    {
        Tiles = new GameObject[BoardWidth, BoardHeight];
        Map = new Map();

        Map.Initialize(BoardWidth, BoardHeight);
        for (int r = 0; r < _maxRooms; r++)
        {
            int roomWidth = Game.Random.Next(_roomMinSize, _roomMaxSize);
            int roomHeight = Game.Random.Next(_roomMinSize, _roomMaxSize);
            int roomXPosition = Game.Random.Next(0, BoardWidth - roomWidth - 1);
            int roomYPosition = Game.Random.Next(0, BoardHeight - roomHeight - 1);
            var newRoom = new Rectangle(roomXPosition, roomYPosition, roomWidth, roomHeight);

            bool newRoomIntersects = Rooms.Any(room => newRoom.Intersects(room));

            if (!newRoomIntersects)
            {
                Rooms.Add(newRoom);
            }
        }

        for (int r = 1; r < Rooms.Count; r++)
        {
            // For all remaing rooms get the center of the room and the previous room
            int previousRoomCenterX = Rooms[r - 1].Center.X;
            int previousRoomCenterY = Rooms[r - 1].Center.Y;
            int currentRoomCenterX = Rooms[r].Center.X;
            int currentRoomCenterY = Rooms[r].Center.Y;

            // Give a 50/50 chance of which 'L' shaped connecting hallway to tunnel out
            if (Game.Random.Next(1, 2) == 1)
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

        int x = Rooms[0].Center.X;
        int y = Rooms[0].Center.Y;
        Player.transform.position = new Vector2(x, y);

        foreach (var room in Rooms)
        {
            CreateRoom(room);
        }

        PlacePlayer();
    }

    void PlacePlayer()
    {
        Map.ComputeFov((int)Player.transform.position.x, (int)Player.transform.position.y, 10, true);
        foreach (Cell cell in Map.GetAllCells())
        {
            if (Map.IsInFov(cell.X, cell.Y))
            {
                Map.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
            }
        }
        Draw();
    }

    private void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition)
    {
        for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
        {
            Map.SetCellProperties(x, yPosition, true, true);
        }
    }

    private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
    {
        for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
        {
            Map.SetCellProperties(xPosition, y, true, true);
        }
    }

    private void Draw()
    {
        Transform boardHolder = GameObject.Find("Board").transform;

        foreach (var cell in Map.GetAllCells())
        {
            if (!cell.IsExplored)
                continue;

            GameObject tileType;
            if (Map.IsInFov(cell.X, cell.Y))
            {
                if (cell.IsWalkable)
                    tileType = Floor;
                else
                    tileType = Wall;
                if (Tiles[cell.X, cell.Y] == null)
                {

                    GameObject instance = Instantiate<GameObject>(tileType, new Vector2(cell.X, cell.Y),
                        Quaternion.identity);
                    instance.transform.SetParent(boardHolder);
                    Tiles[cell.X, cell.Y] = instance;
                }
                Tiles[cell.X, cell.Y].GetComponent<Renderer>().material.color = Color.white;

            }
            else
            {
                if (cell.IsWalkable)
                    tileType = Floor;
                else
                    tileType = Wall;
                if (Tiles[cell.X, cell.Y] == null)
                {

                    GameObject instance = Instantiate<GameObject>(tileType, new Vector2(cell.X, cell.Y),
                        Quaternion.identity);
                    instance.transform.SetParent(boardHolder);
                    Tiles[cell.X, cell.Y] = instance;
                }
                Tiles[cell.X, cell.Y].GetComponent<Renderer>().material.color = Color.gray;
            }

        }
    }

}
