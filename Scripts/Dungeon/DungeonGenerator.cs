using System;
using System.Collections.Generic;

namespace NightFall.Scripts.Dungeon;

public static class DungeonGenerator
{
    private const int MinimumChoiceRooms = 4;
    private const int AdditionalChoiceRooms = 3;

    public static IReadOnlyList<RoomType> Generate(ulong seed)
    {
        Random random = new(unchecked((int)(seed ^ (seed >> 32))));
        int choiceRoomCount =
            MinimumChoiceRooms + random.Next(AdditionalChoiceRooms + 1);

        List<RoomType> rooms = [RoomType.Start];
        RoomType previousRoom = RoomType.Start;
        int shopCount = 0;

        for (int index = 0; index < choiceRoomCount; index++)
        {
            RoomType room = ChooseRoomType(random, previousRoom, shopCount);
            rooms.Add(room);
            previousRoom = room;

            if (room == RoomType.Shop)
            {
                shopCount++;
            }
        }

        rooms.Add(RoomType.Boss);
        return rooms;
    }

    private static RoomType ChooseRoomType(
        Random random,
        RoomType previousRoom,
        int shopCount)
    {
        int roll = random.Next(100);

        if (previousRoom == RoomType.Shop)
        {
            return roll < 70 ? RoomType.Combat : RoomType.Elite;
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
}