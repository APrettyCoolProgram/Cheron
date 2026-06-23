using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace CheronStudio.TekstEngine;

public partial class TekstEngineWindow : Window
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    private string? _currentCartridgePath;

    public TekstEngineWindow()
    {
        InitializeComponent();
        //NewCartridge();
    }

    private void btnLoadCartridge_Click(object sender, RoutedEventArgs e) => LoadCartridge();

    private void btnNewCartridge_Click(object sender, RoutedEventArgs e) => NewCartridge();

    private void btnSaveCartridge_Click(object sender, RoutedEventArgs e) => SaveCartridge();

    private void btnPlayCartridge_Click(object sender, RoutedEventArgs e)
        => System.Windows.MessageBox.Show("Play is not implemented yet.", "Tekst Engine", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

    private void LoadCartridge()
    {
        using var dialog = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = "Select a folder containing Tekst cartridge data",
            UseDescriptionForTitle = true,
            ShowNewFolderButton = false
        };

        if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        {
            return;
        }

        string? cartridgePath = Directory.EnumerateFiles(dialog.SelectedPath, "*.tekst", SearchOption.TopDirectoryOnly)
            .FirstOrDefault();

        if (cartridgePath is null)
        {
            System.Windows.MessageBox.Show(
                "No *.tekst file was found in the selected folder.",
                "Tekst Engine",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Warning);
            return;
        }

        string json = File.ReadAllText(cartridgePath, Encoding.UTF8);

        TekstCartridgeDocument? cartridge = JsonSerializer.Deserialize<TekstCartridgeDocument>(json, _jsonOptions);

        if (cartridge is null)
        {
            System.Windows.MessageBox.Show(
                "The cartridge file could not be read.",
                "Tekst Engine",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
            return;
        }

        PopulateEditor(cartridge);
        _currentCartridgePath = cartridgePath;
        Title = $"Cheron Studio: Tekst Engine - {Path.GetFileName(cartridgePath)}";
    }

    private void NewCartridge()
    {
        PopulateEditor(new TekstCartridgeDocument
        {
            GameEngine = "Tekst",
            GameTypeVariation = "A",
            GameColorScheme = "B,W",
            Title = new TekstTitle(),
            WinRule = new TekstWinRule(),
            Rooms = []
        });

        _currentCartridgePath = null;
        Title = "Cheron Studio: Tekst Engine - New Cartridge";
    }

    private void SaveCartridge()
    {
        TekstCartridgeDocument cartridge;

        try
        {
            cartridge = ReadEditor();
        }
        catch (JsonException ex)
        {
            System.Windows.MessageBox.Show(
                $"Rooms JSON is invalid:\n\n{ex.Message}",
                "Tekst Engine",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Warning);
            return;
        }

        string? targetPath = _currentCartridgePath;

        if (string.IsNullOrWhiteSpace(targetPath))
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Tekst cartridge (*.tekst)|*.tekst",
                DefaultExt = ".tekst",
                FileName = "NewCartridge.tekst"
            };

            if (saveDialog.ShowDialog() != true)
            {
                return;
            }

            targetPath = saveDialog.FileName;
        }

        string json = JsonSerializer.Serialize(cartridge, _jsonOptions);
        File.WriteAllText(targetPath, json, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        _currentCartridgePath = targetPath;
        Title = $"Cheron Studio: Tekst Engine - {Path.GetFileName(targetPath)}";

        System.Windows.MessageBox.Show(
            "Cartridge saved.",
            "Tekst Engine",
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Information);
    }

    private void PopulateEditor(TekstCartridgeDocument cartridge)
    {
        txtGameTitle.Text = cartridge.GameTitle;
        txtGameAuthor.Text = cartridge.GameAuthor;
        txtGameVersion.Text = cartridge.GameVersion;
        txtGameDescription.Text = cartridge.GameDescription;
        txtGameEngine.Text = cartridge.GameEngine;
        txtGameType.Text = cartridge.GameType;
        txtGameTypeVariation.Text = cartridge.GameTypeVariation;
        txtGameColorScheme.Text = cartridge.GameColorScheme;
        txtGameGenre.Text = cartridge.GameGenre;
        txtGameDifficulty.Text = cartridge.GameDifficulty;
        txtTextGameType.Text = cartridge.TextGameType;
        txtStartingRoomId.Text = cartridge.StartingRoomId;

        txtBanner.Text = string.Join(Environment.NewLine, cartridge.Title.Banner);
        txtIntro.Text = cartridge.Title.Intro;
        txtHelpPrompt.Text = cartridge.Title.HelpPrompt;

        txtRequiredItemId.Text = cartridge.WinRule.RequiredItemId;
        txtRequiredRoomId.Text = cartridge.WinRule.RequiredRoomId;
        txtVictoryText.Text = cartridge.WinRule.VictoryText;

        txtRooms.Text = JsonSerializer.Serialize(cartridge.Rooms, _jsonOptions);
    }

    private TekstCartridgeDocument ReadEditor()
    {
        List<string> banner = txtBanner.Text
            .Split([Environment.NewLine], StringSplitOptions.None)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();

        List<TekstRoom> rooms = string.IsNullOrWhiteSpace(txtRooms.Text)
            ? []
            : JsonSerializer.Deserialize<List<TekstRoom>>(txtRooms.Text, _jsonOptions) ?? [];

        return new TekstCartridgeDocument
        {
            GameTitle = txtGameTitle.Text,
            GameAuthor = txtGameAuthor.Text,
            GameVersion = txtGameVersion.Text,
            GameDescription = txtGameDescription.Text,
            GameEngine = txtGameEngine.Text,
            GameType = txtGameType.Text,
            GameTypeVariation = txtGameTypeVariation.Text,
            GameColorScheme = txtGameColorScheme.Text,
            GameGenre = txtGameGenre.Text,
            GameDifficulty = txtGameDifficulty.Text,
            TextGameType = txtTextGameType.Text,
            StartingRoomId = txtStartingRoomId.Text,
            Title = new TekstTitle
            {
                Banner = banner,
                Intro = txtIntro.Text,
                HelpPrompt = txtHelpPrompt.Text
            },
            WinRule = new TekstWinRule
            {
                RequiredItemId = txtRequiredItemId.Text,
                RequiredRoomId = txtRequiredRoomId.Text,
                VictoryText = txtVictoryText.Text
            },
            Rooms = rooms
        };
    }
}

public sealed class TekstCartridgeDocument
{
    public string GameTitle { get; set; } = string.Empty;
    public string GameAuthor { get; set; } = "---";
    public string GameVersion { get; set; } = string.Empty;
    public string GameDescription { get; set; } = string.Empty;
    public string GameEngine { get; set; } = "Tekst";
    public string GameType { get; set; } = string.Empty;
    public string GameTypeVariation { get; set; } = "A";
    public string GameColorScheme { get; set; } = "B,W";
    public string GameGenre { get; set; } = string.Empty;
    public string GameDifficulty { get; set; } = string.Empty;
    public string TextGameType { get; set; } = string.Empty;
    public TekstTitle Title { get; set; } = new();
    public TekstWinRule WinRule { get; set; } = new();
    public string StartingRoomId { get; set; } = string.Empty;
    public List<TekstRoom> Rooms { get; set; } = [];
}

public sealed class TekstTitle
{
    public List<string> Banner { get; set; } = [];
    public string Intro { get; set; } = string.Empty;
    public string HelpPrompt { get; set; } = string.Empty;
}

public sealed class TekstWinRule
{
    public string RequiredItemId { get; set; } = string.Empty;
    public string RequiredRoomId { get; set; } = string.Empty;
    public string VictoryText { get; set; } = string.Empty;
}

public sealed class TekstRoom
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<TekstItem> Items { get; set; } = [];
    public List<TekstExit> Exits { get; set; } = [];
}

public sealed class TekstItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool CanTake { get; set; } = true;
}

public sealed class TekstExit
{
    public string Direction { get; set; } = string.Empty;
    public string TargetRoomId { get; set; } = string.Empty;
    public string? MoveDescription { get; set; }
}
