using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;

namespace NightFall.Scripts.Dungeon;

public static class DungeonGenerator
{
    private const int MinimumChoiceRooms = 4;
    private const int AdditionalChoiceRooms = 3;

    private static readonly GridPosition[] Directions =
    [
        new(1, 0),
        new(-1, 0),
        new(0, 1),
        new(0, -1)
    ];

    public static IReadOnlyList<DungeonRoom> Generate(
        ulong seed,
        IReadOnlyDictionary<RoomType, (int Width, int Height)> sizes)
    {
        DeterministicRandom random = new(seed);
        Dictionary<GridPosition, DungeonRoom> rooms = [];

        AddRoom(
            rooms,
            RoomType.Start,
            new GridPosition(0, 0),
            sizes);

        int roomCount =
            MinimumChoiceRooms +
            random.Next(AdditionalChoiceRooms + 1);

        if (!GeneratePath(
                random,
                rooms,
                new GridPosition(0, 0),
                roomCount,
                sizes))
        {
            throw new InvalidOperationException(
                "Unable to generate a valid dungeon path.");
        }

        AddBossSection(
            random,
            rooms,
            sizes);

        return [.. rooms.Values];
    }

    private static bool GeneratePath(
        DeterministicRandom random,
        Dictionary<GridPosition, DungeonRoom> rooms,
        GridPosition current,
        int roomsRemaining,
        IReadOnlyDictionary<RoomType, (int Width, int Height)> sizes)
    {
        if (roomsRemaining == 0)
        {
            return true;
        }

        List<GridPosition> directions =
            GetDirections(random, current);

        foreach (GridPosition direction in directions)
        {
            DungeonRoom currentRoom = rooms[current];

            GridPosition position =
                GetAdjacentPosition(
                    currentRoom,
                    direction,
                    RoomType.Combat,
                    sizes);

            int shopCount =
                CountRooms(rooms, RoomType.Shop);

            RoomType type =
                ChooseRoomType(
                    random,
                    currentRoom.Type,
                    shopCount);

            if (!CanPlaceRoom(
                    rooms,
                    position,
                    type,
                    sizes))
            {
                continue;
            }

            AddRoom(
                rooms,
                type,
                position,
                sizes);

            if (GeneratePath(
                    random,
                    rooms,
                    position,
                    roomsRemaining - 1,
                    sizes))
            {
                return true;
            }

            rooms.Remove(position);
        }

        return false;
    }

    private static List<GridPosition> GetDirections(
        DeterministicRandom random,
        GridPosition current)
    {
        List<GridPosition> directions = [];

        foreach (GridPosition direction in Directions)
        {
            if (current == new GridPosition(0, 0) &&
                (direction.X < 0 || direction.Y < 0))
            {
                continue;
            }

            directions.Add(direction);
        }

        Shuffle(random, directions);

        return directions;
    }

    private static void Shuffle(
        DeterministicRandom random,
        List<GridPosition> values)
    {
        for (int i = values.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);

            (values[i], values[j]) =
                (values[j], values[i]);
        }
    }

    private static GridPosition GetAdjacentPosition(
        DungeonRoom currentRoom,
        GridPosition direction,
        RoomType targetType,
        IReadOnlyDictionary<RoomType, (int Width, int Height)> sizes)
    {
        var targetSize = sizes[targetType];

        return new GridPosition(
            currentRoom.Position.X +
            GetPositionOffset(
                direction.X,
                currentRoom.Width,
                targetSize.Width),
            currentRoom.Position.Y +
            GetPositionOffset(
                direction.Y,
                currentRoom.Height,
                targetSize.Height));
    }

    private static int GetPositionOffset(
        int direction,
        int currentSize,
        int targetSize)
    {
        return direction switch
        {
            1 => currentSize,
            -1 => -targetSize,
            _ => 0
        };
    }

    private static bool CanPlaceRoom(
        Dictionary<GridPosition, DungeonRoom> rooms,
        GridPosition position,
        RoomType type,
        IReadOnlyDictionary<RoomType, (int Width, int Height)> sizes)
    {
        if (!sizes.TryGetValue(type, out var size))
        {
            throw new InvalidOperationException(
                $"No size was provided for {type}.");
        }

        return !rooms.Values.Any(room =>
            RectanglesOverlap(
                position,
                size.Width,
                size.Height,
                room.Position,
                room.Width,
                room.Height));
    }

    private static bool RectanglesOverlap(
        GridPosition firstPosition,
        int firstWidth,
        int firstHeight,
        GridPosition secondPosition,
        int secondWidth,
        int secondHeight)
    {
        return firstPosition.X <
               secondPosition.X + secondWidth &&
               firstPosition.X + firstWidth >
               secondPosition.X &&
               firstPosition.Y <
               secondPosition.Y + secondHeight &&
               firstPosition.Y + firstHeight >
               secondPosition.Y;
    }

    private static void AddBossSection(
        DeterministicRandom random,
        Dictionary<GridPosition, DungeonRoom> rooms,
        IReadOnlyDictionary<RoomType, (int Width, int Height)> sizes)
    {
        List<BossCandidate> candidates =
            FindBossCandidates(rooms, sizes);

        if (candidates.Count == 0)
        {
            throw new InvalidOperationException(
                "Unable to generate a valid Shop-Boss-Shop section.");
        }

        BossCandidate selected =
            candidates[random.Next(candidates.Count)];

        AddRoom(
            rooms,
            RoomType.Shop,
            selected.ShopBefore,
            sizes);

        AddRoom(
            rooms,
            RoomType.Boss,
            selected.Boss,
            sizes);

        AddRoom(
            rooms,
            RoomType.Shop,
            selected.ShopAfter,
            sizes);
    }

    private static List<BossCandidate> FindBossCandidates(
        Dictionary<GridPosition, DungeonRoom> rooms,
        IReadOnlyDictionary<RoomType, (int Width, int Height)> sizes)
    {
        List<BossCandidate> candidates = [];

        foreach (DungeonRoom anchor in rooms.Values)
        {
            foreach (GridPosition direction in Directions)
            {
                BossCandidate? candidate =
                    TryCreateBossCandidate(
                        rooms,
                        anchor,
                        direction,
                        sizes);

                if (candidate != null)
                {
                    candidates.Add(candidate);
                }
            }
        }

        return candidates;
    }

    private static BossCandidate? TryCreateBossCandidate(
        Dictionary<GridPosition, DungeonRoom> rooms,
        DungeonRoom anchor,
        GridPosition direction,
        IReadOnlyDictionary<RoomType, (int Width, int Height)> sizes)
    {
        GridPosition shopBefore =
            GetAdjacentPosition(anchor, direction, RoomType.Shop, sizes);

        if (!CanPlaceRoom(
                rooms,
                shopBefore,
                RoomType.Shop,
                sizes)
        ) return null;

        Dictionary<GridPosition, DungeonRoom> testRooms =
            CreateTestRooms(
                rooms,
                RoomType.Shop,
                shopBefore,
                sizes);

        DungeonRoom temporaryShop =
            testRooms[shopBefore];

        GridPosition boss =
            GetAdjacentPosition(
                temporaryShop,
                direction,
                RoomType.Boss,
                sizes);

        if (!CanPlaceRoom(
                testRooms,
                boss,
                RoomType.Boss,
                sizes)
        ) return null;

        AddRoom(testRooms, RoomType.Boss, boss, sizes);

        GridPosition shopAfter =
            GetAdjacentPosition(
                testRooms[boss],
                direction,
                RoomType.Shop,
                sizes);

        if (!CanPlaceRoom(
                testRooms,
                shopAfter,
                RoomType.Shop,
                sizes))
        {
            return null;
        }

        return new BossCandidate(
            shopBefore,
            boss,
            shopAfter);
    }

    private static Dictionary<GridPosition, DungeonRoom> CreateTestRooms(
        Dictionary<GridPosition, DungeonRoom> rooms,
        RoomType type,
        GridPosition position,
        IReadOnlyDictionary<RoomType, (int Width, int Height)> sizes)
    {
        Dictionary<GridPosition, DungeonRoom> result =
            new(rooms);

        AddRoom(
            result,
            type,
            position,
            sizes);

        return result;
    }

    private static void AddRoom(
        Dictionary<GridPosition, DungeonRoom> rooms,
        RoomType type,
        GridPosition position,
        IReadOnlyDictionary<RoomType, (int Width, int Height)> sizes)
    {
        var size = sizes[type];

        rooms[position] =
            new DungeonRoom(
                type,
                position,
                size.Width,
                size.Height);
    }

    private static int CountRooms(
        Dictionary<GridPosition, DungeonRoom> rooms,
        RoomType type)
    {
        int count = 0;

        foreach (DungeonRoom room in rooms.Values)
        {
            if (room.Type == type)
            {
                count++;
            }
        }

        return count;
    }

    private static RoomType ChooseRoomType(
        DeterministicRandom random,
        RoomType previousRoom,
        int shopCount)
    {
        int roll = random.Next(100);

        if (previousRoom == RoomType.Shop)
        {
            return roll < 70
                ? RoomType.Combat
                : RoomType.Elite;
        }

        if (shopCount == 0 && roll < 20)
        {
            return RoomType.Shop;
        }

        return roll < 70
            ? RoomType.Combat
            : RoomType.Elite;
    }

    private sealed record BossCandidate(
        GridPosition ShopBefore,
        GridPosition Boss,
        GridPosition ShopAfter);

    private sealed class DeterministicRandom(ulong seed)
    {
        private readonly byte[] _seedBytes =
            BitConverter.GetBytes(seed);

        private ulong _counter;

        public int Next(int maxExclusive)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(
                maxExclusive);

            byte[] counter =
                BitConverter.GetBytes(
                    _counter++);

            byte[] input =
                new byte[
                    _seedBytes.Length +
                    counter.Length];

            Buffer.BlockCopy(
                _seedBytes,
                0,
                input,
                0,
                _seedBytes.Length);

            Buffer.BlockCopy(
                counter,
                0,
                input,
                _seedBytes.Length,
                counter.Length);

            byte[] hash =
                SHA256.HashData(input);

            return (int)(
                BitConverter.ToUInt64(
                    hash,
                    0) %
                (ulong)maxExclusive);
        }
    }
}