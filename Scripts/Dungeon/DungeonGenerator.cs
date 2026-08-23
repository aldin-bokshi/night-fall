using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace NightFall.Scripts.Dungeon;

public static class DungeonGenerator
{
    private const int MinimumChoiceRooms = 4;
    private const int AdditionalChoiceRooms = 3;

    public static IReadOnlyList<RoomType> Generate(ulong seed)
    {
        DeterministicRandom random = new(seed);

        int roomCount = GetRoomCount(random);

        List<RoomType> rooms = [RoomType.Start];

        GenerateChoiceRooms(
            random,
            rooms,
            roomCount);

        rooms.Add(RoomType.Boss);

        return rooms;
    }

    private static int GetRoomCount(
        DeterministicRandom random)
    {
        return MinimumChoiceRooms +
               random.Next(AdditionalChoiceRooms + 1);
    }

    private static void GenerateChoiceRooms(
        DeterministicRandom random,
        List<RoomType> rooms,
        int roomCount)
    {
        RoomType previousRoom = RoomType.Start;
        int shopCount = 0;

        for (int index = 0; index < roomCount; index++)
        {
            RoomType room = ChooseRoomType(
                random,
                previousRoom,
                shopCount);

            rooms.Add(room);

            previousRoom = room;

            if (room == RoomType.Shop)
            {
                shopCount++;
            }
        }
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

        if (roll < 70)
        {
            return RoomType.Combat;
        }

        return RoomType.Elite;
    }

    private sealed class DeterministicRandom(ulong seed)
    {
        private readonly byte[] _seedBytes =
            BitConverter.GetBytes(seed);

        private ulong _counter;

        public int Next(int maxExclusive)
        {
            if (maxExclusive <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxExclusive),
                    "maxExclusive must be greater than zero.");
            }

            ulong value = GetUInt64();

            return (int)(value % (ulong)maxExclusive);
        }

        private ulong GetUInt64()
        {
            byte[] counterBytes =
                BitConverter.GetBytes(_counter++);

            byte[] input = new byte[
                _seedBytes.Length + counterBytes.Length];

            Buffer.BlockCopy(
                _seedBytes,
                0,
                input,
                0,
                _seedBytes.Length);

            Buffer.BlockCopy(
                counterBytes,
                0,
                input,
                _seedBytes.Length,
                counterBytes.Length);

            byte[] hash = SHA256.HashData(input);

            return BitConverter.ToUInt64(hash, 0);
        }
    }
}