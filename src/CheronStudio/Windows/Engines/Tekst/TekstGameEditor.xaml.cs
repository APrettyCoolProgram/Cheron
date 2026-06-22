using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace CheronStudio.Engines.Tekst;

public partial class TekstGameEditor : UserControl
{
    // ── State ─────────────────────────────────────────────────────────────────

    private TekstGame _game         = new();
    private string?   _filePath;
    private TekstRoom? _selectedRoom;
    private bool      _suppressEvents;
    private bool      _isDirty;

    public bool IsDirty
    {
        get => _isDirty;
        private set
        {
            _isDirty = value;
            DirtyChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public string? FilePath => _filePath;

    public event EventHandler? DirtyChanged;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented               = true,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition      = JsonIgnoreCondition.WhenWritingNull,
    };

    // ── Constructor ───────────────────────────────────────────────────────────

    public TekstGameEditor()
    {
        InitializeComponent();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public void Load(string filePath)
    {
        var json = File.ReadAllText(filePath);
        _game     = JsonSerializer.Deserialize<TekstGame>(json, JsonOptions) ?? new TekstGame();
        _filePath = filePath;
        IsDirty   = false;
        RefreshAll();
    }

    public bool Save()
    {
        if (_filePath is null)
            return false;

        CommitCurrentRoom();
        CommitGameProperties();
        File.WriteAllText(_filePath, JsonSerializer.Serialize(_game, JsonOptions));
        IsDirty = false;
        return true;
    }

    public bool SaveAs()
    {
        var dialog = new SaveFileDialog
        {
            Title      = "Save Tekst Game As",
            Filter     = "Tekst Game Files (*.tekst)|*.tekst|All Files (*.*)|*.*",
            DefaultExt = ".tekst",
            FileName   = Path.GetFileName(_filePath) ?? "game.tekst",
        };

        if (dialog.ShowDialog() != true)
            return false;

        _filePath = dialog.FileName;
        return Save();
    }

    // ── Internal refresh ──────────────────────────────────────────────────────

    private void RefreshAll()
    {
        _suppressEvents = true;

        // Details
        TxtGameTitle.Text         = _game.Detail.GameTitle;
        TxtGameAuthor.Text        = _game.Detail.GameAuthor;
        TxtGameVersion.Text       = _game.Detail.GameVersion;
        TxtGameType.Text          = _game.Detail.GameType;
        TxtGameTypeVariation.Text = _game.Detail.GameTypeVariation;
        TxtGameGenre.Text         = _game.Detail.GameGenre;
        TxtGameDifficulty.Text    = _game.Detail.GameDifficulty;
        TxtGameDescription.Text   = _game.Detail.GameDescription;
        TxtBanner.Text            = string.Join(Environment.NewLine, _game.Title.BannerContents);

        // Win Rule
        TxtWinItemId.Text   = _game.WinRule.RequiredItemId;
        TxtWinRoomId.Text   = _game.WinRule.RequiredRoomId;
        TxtVictoryText.Text = _game.WinRule.VictoryText;

        // Rooms
        RefreshRoomList();
        RefreshStartingRoomCombo();

        _suppressEvents = false;
    }

    private void RefreshRoomList()
    {
        RoomList.Items.Clear();
        foreach (var room in _game.Rooms)
            RoomList.Items.Add(room.Id);

        if (_selectedRoom is not null && _game.Rooms.Contains(_selectedRoom))
            RoomList.SelectedItem = _selectedRoom.Id;
        else
            LoadRoomIntoEditor(null);
    }

    private void RefreshStartingRoomCombo()
    {
        CboStartingRoom.Items.Clear();
        foreach (var room in _game.Rooms)
            CboStartingRoom.Items.Add(room.Id);

        CboStartingRoom.SelectedItem = _game.StartingRoomId;
    }

    private void LoadRoomIntoEditor(TekstRoom? room)
    {
        _suppressEvents = true;
        _selectedRoom   = room;

        if (room is null)
        {
            RoomEditorPanel.IsEnabled = false;
            TxtRoomId.Text            = string.Empty;
            TxtRoomTitle.Text         = string.Empty;
            TxtRoomDescription.Text   = string.Empty;
            ExitsGrid.ItemsSource     = null;
            ItemsGrid.ItemsSource     = null;
        }
        else
        {
            RoomEditorPanel.IsEnabled = true;
            TxtRoomId.Text            = room.Id;
            TxtRoomTitle.Text         = room.Title;
            TxtRoomDescription.Text   = room.Description;
            ExitsGrid.ItemsSource     = room.Exits;
            ItemsGrid.ItemsSource     = room.Items;
        }

        _suppressEvents = false;
    }

    // ── Commit helpers ────────────────────────────────────────────────────────

    private void CommitGameProperties()
    {
        _game.Detail.GameTitle         = TxtGameTitle.Text;
        _game.Detail.GameAuthor        = TxtGameAuthor.Text;
        _game.Detail.GameVersion       = TxtGameVersion.Text;
        _game.Detail.GameType          = TxtGameType.Text;
        _game.Detail.GameTypeVariation = TxtGameTypeVariation.Text;
        _game.Detail.GameGenre         = TxtGameGenre.Text;
        _game.Detail.GameDifficulty    = TxtGameDifficulty.Text;
        _game.Detail.GameDescription   = TxtGameDescription.Text;

        _game.Title.BannerContents = [.. TxtBanner.Text.Split(Environment.NewLine)];

        _game.WinRule.RequiredItemId = TxtWinItemId.Text;
        _game.WinRule.RequiredRoomId = TxtWinRoomId.Text;
        _game.WinRule.VictoryText    = TxtVictoryText.Text;

        _game.StartingRoomId = CboStartingRoom.SelectedItem as string ?? string.Empty;
    }

    private void CommitCurrentRoom()
    {
        if (_selectedRoom is null) return;
        _selectedRoom.Description = TxtRoomDescription.Text;
        _selectedRoom.Title       = TxtRoomTitle.Text;
    }

    // ── Game property events ──────────────────────────────────────────────────

    private void GameProperty_Changed(object sender, TextChangedEventArgs e)
    {
        if (_suppressEvents) return;
        CommitGameProperties();
        IsDirty = true;
    }

    private void CboStartingRoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressEvents) return;
        _game.StartingRoomId = CboStartingRoom.SelectedItem as string ?? string.Empty;
        IsDirty = true;
    }

    // ── Room list events ──────────────────────────────────────────────────────

    private void RoomList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressEvents) return;
        CommitCurrentRoom();
        var selectedId = RoomList.SelectedItem as string;
        LoadRoomIntoEditor(_game.Rooms.FirstOrDefault(r => r.Id == selectedId));
    }

    private void BtnAddRoom_Click(object sender, RoutedEventArgs e)
    {
        var id   = GenerateUniqueRoomId();
        var room = new TekstRoom { Id = id, Title = "New Room" };
        _game.Rooms.Add(room);
        IsDirty = true;

        RefreshRoomList();
        RefreshStartingRoomCombo();
        RoomList.SelectedItem = id;
    }

    private void BtnDeleteRoom_Click(object sender, RoutedEventArgs e)
    {
        if (RoomList.SelectedItem is not string id) return;
        var room = _game.Rooms.FirstOrDefault(r => r.Id == id);
        if (room is null) return;

        var result = MessageBox.Show(
            $"Delete room \"{id}\"?",
            "Delete Room",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        _game.Rooms.Remove(room);
        _selectedRoom = null;
        IsDirty = true;

        RefreshRoomList();
        RefreshStartingRoomCombo();
    }

    // ── Room editor events ────────────────────────────────────────────────────

    private void TxtRoomId_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_suppressEvents || _selectedRoom is null) return;

        var newId = TxtRoomId.Text.Trim();
        if (newId == _selectedRoom.Id || string.IsNullOrWhiteSpace(newId) || _game.Rooms.Any(r => r.Id == newId))
            return;

        var oldId = _selectedRoom.Id;
        _selectedRoom.Id = newId;

        if (_game.StartingRoomId == oldId)
            _game.StartingRoomId = newId;

        foreach (var room in _game.Rooms)
            foreach (var exit in room.Exits.Where(ex => ex.TargetRoomId == oldId))
                exit.TargetRoomId = newId;

        IsDirty = true;
        RefreshRoomList();
        RefreshStartingRoomCombo();
    }

    private void RoomProperty_Changed(object sender, TextChangedEventArgs e)
    {
        if (_suppressEvents || _selectedRoom is null) return;
        CommitCurrentRoom();
        IsDirty = true;
    }

    // ── Exit events ───────────────────────────────────────────────────────────

    private void BtnAddExit_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedRoom is null) return;
        _selectedRoom.Exits.Add(new TekstExit());
        RefreshGrid(ExitsGrid, _selectedRoom.Exits);
        IsDirty = true;
    }

    private void BtnDeleteExit_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedRoom is null || ExitsGrid.SelectedItem is not TekstExit exit) return;
        _selectedRoom.Exits.Remove(exit);
        RefreshGrid(ExitsGrid, _selectedRoom.Exits);
        IsDirty = true;
    }

    private void ExitsGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (_suppressEvents) return;
        IsDirty = true;
    }

    // ── Item events ───────────────────────────────────────────────────────────

    private void BtnAddItem_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedRoom is null) return;
        _selectedRoom.Items.Add(new TekstItem { CanTake = true });
        RefreshGrid(ItemsGrid, _selectedRoom.Items);
        IsDirty = true;
    }

    private void BtnDeleteItem_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedRoom is null || ItemsGrid.SelectedItem is not TekstItem item) return;
        _selectedRoom.Items.Remove(item);
        RefreshGrid(ItemsGrid, _selectedRoom.Items);
        IsDirty = true;
    }

    private void ItemsGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (_suppressEvents) return;
        IsDirty = true;
    }

    // ── Utilities ─────────────────────────────────────────────────────────────

    private string GenerateUniqueRoomId()
    {
        int n = 1;
        while (_game.Rooms.Any(r => r.Id == $"room{n}"))
            n++;
        return $"room{n}";
    }

    private static void RefreshGrid<T>(DataGrid grid, List<T> source)
    {
        grid.ItemsSource = null;
        grid.ItemsSource = source;
    }
}

