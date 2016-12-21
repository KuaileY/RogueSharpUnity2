
using System.Collections.Generic;
using System.Linq;
using RogueSharp;
using UnityEngine;

public class DungeonMap:Map
{
    public List<Monster> _monsters;
    public List<TreasurePile> _treasurePiles;
    public List<Rectangle> Rooms;
    public List<Entry> Doors;
    public Stairs StairsUp;
    public Stairs StairsDown;

    public DungeonMap()
    {
        _monsters = new List<Monster>();
        _treasurePiles = new List<TreasurePile>();
        Game.SchedulingSystem.Clear();

        Rooms = new List<Rectangle>();
        Doors = new List<Entry>();
    }

    public void AddMonster(Monster monster)
    {
        monster.go= GameObject.Instantiate<GameObject>(Game.Items[monster.Name], new Vector2(monster.X, monster.Y), Quaternion.identity);
        monster.go.transform.SetParent(Game.itemsHolder);
        monster.go.GetComponent<Renderer>().enabled = false;
        _monsters.Add(monster);
        Game.ItemsTiles[monster.X, monster.Y] = monster;
        SetIsWalkable(monster.X, monster.Y, false);
        Game.SchedulingSystem.Add(monster);
    }

    public void RemoveMonster(Monster monster)
    {
        GameObject.Destroy(monster.go);
        Game.ItemsTiles[monster.X, monster.Y] = null;
        _monsters.Remove(monster);
        SetIsWalkable(monster.X, monster.Y, true);
        Game.SchedulingSystem.Remove(monster);
    }

    public Monster GetMonsterAt(int x, int y)
    {
        // BUG: This should be single except sometiems monsters occupy the same space.
        return _monsters.FirstOrDefault(m => m.X == x && m.Y == y);
    }

    public IEnumerable<Point> GetMonsterLocations()
    {
        return _monsters.Select(m => new Point
        {
            X = m.X,
            Y = m.Y
        });
    }

    public IEnumerable<Point> GetMonsterLocationsInFieldOfView()
    {
        return _monsters.Where(monster => IsInFov(monster.X, monster.Y))
            .Select(m => new Point { X = m.X, Y = m.Y });
    }

    public Point GetRandomLocation()
    {
        int roomNumber = Game.Random.Next(0, Rooms.Count - 1);
        Rectangle randomRoom = Rooms[roomNumber];

        if (!DoesRoomHaveWalkableSpace(randomRoom))
        {
            GetRandomLocation();
        }

        return GetRandomLocationInRoom(randomRoom);
    }

    public Point GetRandomLocationInRoom(Rectangle room)
    {
        int x = Game.Random.Next(1, room.Width - 2) + room.X;
        int y = Game.Random.Next(1, room.Height - 2) + room.Y;
        if (!IsWalkable(x, y))
        {
            GetRandomLocationInRoom(room);
        }
        return new Point(x, y);
    }

    public void AddTreasure(int x, int y, ITreasure treasure)
    {
        _treasurePiles.Add(new TreasurePile(x, y, treasure));
    }

    public bool DoesRoomHaveWalkableSpace(Rectangle room)
    {
        for (int x = 1; x <= room.Width - 2; x++)
        {
            for (int y = 1; y <= room.Height - 2; y++)
            {
                if (IsWalkable(x + room.X, y + room.Y))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void AddPlayer(Player player)
    {
        Game.Player = player;
        SetIsWalkable(player.X, player.Y, false);
        UpdatePlayerFieldOfView();
        Game.SchedulingSystem.Add(player);
    }

    public void SetIsWalkable(int x, int y, bool isWalkable)
    {
        var cell = GetCell(x, y);
        SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
    }

    void UpdatePlayerFieldOfView()
    {
        Player player = Game.Player;
        ComputeFov((int)player.X, (int)player.Y, player.Awareness, true);

        foreach (var cell in GetAllCells())
        {
            if (IsInFov(cell.X, cell.Y))
            {
                SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
            }
        }

    }

    public Entry GetDoor(int x, int y)
    {
        return Doors.SingleOrDefault(d => d.go.transform.position.x == x && d.go.transform.position.y == y);
    }

    void OpenDoor(Actor actor, int x, int y)
    {
        var door = GetDoor(x, y);
        if (door != null && door.Stat == "close")
        {
            door.Stat = "open";
            var cell = GetCell(x, y);
            SetCellProperties(x, y, true, true, cell.IsExplored);
            door.go.GetComponent<Animator>().SetTrigger("Open");
        }
    }

    public void AddGold(int x, int y, int amount)
    {
        if (amount > 0)
        {
            var gold = new Gold(amount);
            gold.go= GameObject.Instantiate<GameObject>(Game.Items[gold.Name], new Vector2(x, y), Quaternion.identity);
            gold.go.transform.SetParent(Game.itemsHolder);
            Game.ItemsTiles[x, y] = gold;
            AddTreasure(x, y, gold);
        }
    }

    public void Draw()
    {
        DrawGround();
        DrawItems();
        StairsDown.Draw(this);

        int i = 0;
        foreach (Monster monster in _monsters)
        {
            monster.Draw(this);
            if (IsInFov(monster.X, monster.Y))
            {
                monster.DrawStats(Game.MonsterStat, Game.MonsterItem);
                i++;
            }
        }
        StairsUp.Draw(this);


        Player player = Game.Player;
        player.DrawStats();
        player.DrawInventoryE();
        player.DrawInventoryA();
        player.DrawInventoryI();
    }

    void DrawGround()
    {
        foreach (var cell in GetAllCells())
        {
            if (!cell.IsExplored)
                continue;

            GameObject tileType;
            if (IsInFov(cell.X, cell.Y))
            {
                if (cell.IsWalkable)
                    tileType = Game.Floor;
                else
                    tileType = Game.Wall;
                if (Game.groundTiles[cell.X, cell.Y] == null)
                {
                    GameObject instance = GameObject.Instantiate<GameObject>(tileType, new Vector2(cell.X, cell.Y),
                        Quaternion.identity);
                    instance.transform.SetParent(Game.boardHolder);
                    Game.groundTiles[cell.X, cell.Y] = instance;
                }
                Game.groundTiles[cell.X, cell.Y].GetComponent<Renderer>().material.color = Color.white;
            }
            else
            {
                if (cell.IsWalkable)
                    tileType = Game.Floor;
                else
                    tileType = Game.Wall;
                if (Game.groundTiles[cell.X, cell.Y] == null)
                {
                    GameObject instance = GameObject.Instantiate<GameObject>(tileType, new Vector2(cell.X, cell.Y),
                        Quaternion.identity);
                    instance.transform.SetParent(Game.boardHolder);
                    Game.groundTiles[cell.X, cell.Y] = instance;
                }
                Game.groundTiles[cell.X, cell.Y].GetComponent<Renderer>().material.color = Color.gray;
            }
        }
    }

    void DrawItems()
    {
        foreach (var cell in GetAllCells())
        {
            if (!cell.IsExplored)
                continue;

            if (IsInFov(cell.X, cell.Y))
            {
                if (Game.ItemsTiles[cell.X, cell.Y] != null)
                    Game.ItemsTiles[cell.X, cell.Y].go.GetComponent<Renderer>().enabled = true;
            }
            else
            {
                if (Game.ItemsTiles[cell.X, cell.Y] != null)
                    Game.ItemsTiles[cell.X, cell.Y].go.GetComponent<Renderer>().enabled = false;
            }
        }
    }

    public bool SetActorPosition(Actor actor, int x, int y)
    {
        if (GetCell(x, y).IsWalkable)
        {
            PickUpTreasure(actor, x, y);
            SetIsWalkable(actor.X, actor.Y, true);
            Game.ItemsTiles[actor.X, actor.Y] = null;
            Game.ItemsTiles[x, y] = actor;
            actor.X = x;
            actor.Y = y;
            actor.go.transform.position = new Vector2(x, y);
            SetIsWalkable(x, y, false);
            OpenDoor(actor, x, y);
            if (actor is Player)
            {
                UpdatePlayerFieldOfView();
            }
            return true;
        }
        return false;
    }

    public bool CanMoveDownToNextLevel()
    {
        Player player = Game.Player;

        return StairsDown.X == player.X && StairsDown.Y == player.Y;
    }

    private void PickUpTreasure(Actor actor, int x, int y)
    {
        List<TreasurePile> treasureAtLocation = _treasurePiles.Where(g => g.X == x && g.Y == y).ToList();
        foreach (TreasurePile treasurePile in treasureAtLocation)
        {
            if (treasurePile.Treasure.PickUp(actor))
            {
                GameObject.Destroy(Game.ItemsTiles[x, y].go);
                Game.ItemsTiles[x, y] = null;
                _treasurePiles.Remove(treasurePile);
            }
        }
    }

}

