namespace TextGame;

public static class CartridgeBuilder
{
    public static GameEngine BuildGameFromCartridge(Cartridge.GameCartridge cartridge)
    {
        var roomMap = new Dictionary<string, Room>();
        var itemMap = new Dictionary<string, Item>();

        // Create all items first
        foreach (var itemData in cartridge.Items)
        {
            var item = new Item(itemData.Name, itemData.Description, itemData.IsTakeable)
            {
                IsLightSource = itemData.IsLightSource
            };

            foreach (var alias in itemData.Aliases)
            {
                item.AddAlias(alias);
            }

            foreach (var action in itemData.CustomActions)
            {
                item.AddCustomAction(action.Verb, action.Response);
            }

            itemMap[itemData.Id] = item;
        }

        // Create all rooms
        foreach (var roomData in cartridge.Rooms)
        {
            var room = new Room(roomData.Name, roomData.Description, roomData.IsLit);
            roomMap[roomData.Id] = room;

            // Add items to room
            foreach (var itemId in roomData.Items)
            {
                if (itemMap.TryGetValue(itemId, out var item))
                {
                    room.AddItem(item);
                }
            }
        }

        // Connect rooms with exits
        foreach (var roomData in cartridge.Rooms)
        {
            var room = roomMap[roomData.Id];
            
            foreach (var exitData in roomData.Exits)
            {
                if (roomMap.TryGetValue(exitData.TargetRoomId, out var targetRoom))
                {
                    var direction = Enum.Parse<Direction>(exitData.Direction);
                    room.AddExit(direction, targetRoom);
                }
            }
        }

        // Get starting room
        if (!roomMap.TryGetValue(cartridge.StartingRoomId, out var startingRoom))
        {
            throw new InvalidOperationException($"Starting room '{cartridge.StartingRoomId}' not found.");
        }

        return new GameBuilder()
            .WithTitle(cartridge.Title)
            .WithIntro(cartridge.Intro)
            .WithStartingRoom(startingRoom)
            .Build();
    }
}
