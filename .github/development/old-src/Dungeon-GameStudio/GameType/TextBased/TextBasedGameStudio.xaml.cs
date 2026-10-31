// 260130_code
// 260130_documentation

using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace DungineStudio.GameType.TextBased;

/// <summary>Interaction logic for TextBasedWindow.xaml</summary>
public partial class TextBasedWindow : Window
{
    public string BoxArtFileName { get; set; }

    public string LableArtFileName { get; set; }

    public TextGameWorldModel GameWorld { get; set; }

    //private TextGameWorldModel _world;
    //private Location? _currentLocation;
    //private Item? _currentItem;
    //private string? _gameJsonPath;
    //private string? _boxArtFileName;
    //private string? _labelArtFileName;

    public TextBasedWindow()
    {
        InitializeComponent();

        BoxArtFileName   = string.Empty;
        LableArtFileName = string.Empty;

        GameWorld = new TextGameWorldModel();
    }



    private void ChooseBoxart()
    {
        var fileDialog = new OpenFileDialog
        {
            Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All files (*.*)|*.*"
        };

        BoxArtFileName = fileDialog.ShowDialog() == true ? fileDialog.FileName : string.Empty;


        var z = 5;
        //if (string.IsNullOrWhiteSpace(_gameJsonPath))
        //{
        //    var sfd = new SaveFileDialog
        //    {
        //        Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
        //        FileName = "game.json",
        //        DefaultExt = ".json"
        //    };

        //    if (sfd.ShowDialog() != true)
        //        return;
        //    _gameJsonPath = sfd.FileName;
        //}

        //var destFolder = System.IO.Path.GetDirectoryName(_gameJsonPath!)!;
        //Directory.CreateDirectory(destFolder);

        //var ext = System.IO.Path.GetExtension(srcPath);
        //var destFileName = "boxart" + ext;
        //var destPath = System.IO.Path.Combine(destFolder, destFileName);
        //File.Copy(srcPath, destPath, true);
        //_boxArtFileName = destFileName;

        //// Load into UI image control if present
        //try
        //{
        //    LoadImageToControl(imgCartridge, destPath);
        //}
        //catch { }

        //UpdateGameJsonFile();
    }


















    private void ChooseLabel()
    {
        //var ofd = new OpenFileDialog
        //{
        //    Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All files (*.*)|*.*"
        //};

        //if (ofd.ShowDialog() != true)
        //    return;

        //var srcPath = ofd.FileName;
        //if (string.IsNullOrWhiteSpace(srcPath) || !File.Exists(srcPath))
        //    return;

        //if (string.IsNullOrWhiteSpace(_gameJsonPath))
        //{
        //    var sfd = new SaveFileDialog
        //    {
        //        Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
        //        FileName = "game.json",
        //        DefaultExt = ".json"
        //    };

        //    if (sfd.ShowDialog() != true)
        //        return;
        //    _gameJsonPath = sfd.FileName;
        //}

        //var destFolder = System.IO.Path.GetDirectoryName(_gameJsonPath!)!;
        //Directory.CreateDirectory(destFolder);

        //var ext = System.IO.Path.GetExtension(srcPath);
        //var destFileName = "label" + ext;
        //var destPath = System.IO.Path.Combine(destFolder, destFileName);
        //File.Copy(srcPath, destPath, true);
        //_labelArtFileName = destFileName;

        //try
        //{
        //    LoadImageToControl(imgLabel, destPath);
        //}
        //catch { }

        //UpdateGameJsonFile();
    }

    private void UpdateGameJsonFile()
    {
        ////if (string.IsNullOrWhiteSpace(_gameJsonPath))
        ////    return;

        ////// Load existing data if present
        ////Dictionary<string, string?> data = new();
        ////if (File.Exists(_gameJsonPath))
        ////{
        ////    try
        ////    {
        ////        var existing = JsonSerializer.Deserialize<Dictionary<string, string?>>(File.ReadAllText(_gameJsonPath));
        ////        if (existing != null)
        ////        {
        ////            foreach (var kv in existing)
        ////                data[kv.Key] = kv.Value;
        ////        }
        ////    }
        ////    catch { }
        ////}

        ////if (!string.IsNullOrWhiteSpace(_boxArtFileName))
        ////    data["boxArt"] = _boxArtFileName;
        ////if (!string.IsNullOrWhiteSpace(_labelArtFileName))
        ////    data["labelArt"] = _labelArtFileName;
        ////if (this.FindName("txbxGameName") is TextBox tb && !string.IsNullOrWhiteSpace(tb.Text))
        ////    data["gameName"] = tb.Text;

        ////var options = new JsonSerializerOptions { WriteIndented = true };
        ////File.WriteAllText(_gameJsonPath, JsonSerializer.Serialize(data, options));
    }

    private void LoadImageToControl(System.Windows.Controls.Image control, string path)
    {
        //var bi = new BitmapImage();
        //using (var fs = File.OpenRead(path))
        //{
        //    bi.BeginInit();
        //    bi.CacheOption = BitmapCacheOption.OnLoad;
        //    bi.StreamSource = fs;
        //    bi.EndInit();
        //    bi.Freeze();
        //}
        //control.Source = bi;
    }

    private void BtnAddLocation_Click(object sender, RoutedEventArgs e)
    {
        ////var newLocation = new Location
        ////{
        ////    Id = $"location_{_world.Locations.Count + 1}",
        ////    Name = "New Location",
        ////    Description = "Description here..."
        ////};

        ////_world.Locations.Add(newLocation);
        ////RefreshLocationsList();
        ////lstLocations.SelectedItem = newLocation;
    }

    private void BtnRemoveLocation_Click(object sender, RoutedEventArgs e)
    {
        ////if (lstLocations.SelectedItem is Location location)
        ////{
        ////    _world.Locations.Remove(location);
        ////    RefreshLocationsList();
        ////    ClearLocationEditor();
        ////}
    }

    private void LstLocations_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ////if (lstLocations.SelectedItem is Location location)
        ////{
        ////    _currentLocation = location;
        ////    LoadLocationToEditor(location);
        ////    RefreshItemsList();
        ////}
    }

    private void LoadLocationToEditor(Location location)
    {
        //txtLocationId.TextChanged -= LocationField_Changed;
        //txtLocationName.TextChanged -= LocationField_Changed;
        //txtLocationDescription.TextChanged -= LocationField_Changed;
        //txtLocationExits.TextChanged -= LocationField_Changed;

        //txtLocationId.Text = location.Id;
        //txtLocationName.Text = location.Name;
        //txtLocationDescription.Text = location.Description;

        //var exitsText = string.Join(Environment.NewLine,
        //    location.Exits.Select(kvp => $"{kvp.Key}:{kvp.Value}"));
        //txtLocationExits.Text = exitsText;

        //txtLocationId.TextChanged += LocationField_Changed;
        //txtLocationName.TextChanged += LocationField_Changed;
        //txtLocationDescription.TextChanged += LocationField_Changed;
        //txtLocationExits.TextChanged += LocationField_Changed;
    }

    private void LocationField_Changed(object sender, RoutedEventArgs e)
    {
        ////if (_currentLocation == null)
        ////    return;

        ////_currentLocation.Id = txtLocationId.Text;
        ////_currentLocation.Name = txtLocationName.Text;
        ////_currentLocation.Description = txtLocationDescription.Text;

        ////_currentLocation.Exits.Clear();
        ////var lines = txtLocationExits.Text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        ////foreach (var line in lines)
        ////{
        ////    var parts = line.Split(':');
        ////    if (parts.Length == 2)
        ////    {
        ////        _currentLocation.Exits[parts[0].Trim()] = parts[1].Trim();
        ////    }
        ////}

        ////RefreshLocationsList();
    }

    private void ClearLocationEditor()
    {
        ////txtLocationId.Clear();
        ////txtLocationName.Clear();
        ////txtLocationDescription.Clear();
        ////txtLocationExits.Clear();
        ////_currentLocation = null;
    }

    private void RefreshLocationsList()
    {
        ////var selectedLocation = lstLocations.SelectedItem;
        ////lstLocations.ItemsSource = null;
        ////lstLocations.ItemsSource = _world.Locations;
        ////lstLocations.SelectedItem = selectedLocation;
    }

    private void BtnAddItem_Click(object sender, RoutedEventArgs e)
    {
        ////if (_currentLocation == null)
        ////{
        ////    MessageBox.Show("Please select a location first.", "No Location Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
        ////    return;
        ////}

        ////var newItem = new Item
        ////{
        ////    Id = $"item_{_currentLocation.Items.Count + 1}",
        ////    Name = "New Item",
        ////    Description = "Description here..."
        ////};

        ////_currentLocation.Items.Add(newItem);
        ////RefreshItemsList();
        ////lstItems.SelectedItem = newItem;
    }

    private void BtnRemoveItem_Click(object sender, RoutedEventArgs e)
    {
        ////if (_currentLocation != null && lstItems.SelectedItem is Item item)
        ////{
        ////    _currentLocation.Items.Remove(item);
        ////    RefreshItemsList();
        ////    ClearItemEditor();
        ////}
    }

    private void LstItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ////if (lstItems.SelectedItem is Item item)
        ////{
        ////    _currentItem = item;
        ////    LoadItemToEditor(item);
        ////}
    }

    private void LoadItemToEditor(Item item)
    {
        //txtItemId.TextChanged -= ItemField_Changed;
        //txtItemName.TextChanged -= ItemField_Changed;
        //txtItemDescription.TextChanged -= ItemField_Changed;
        //txtItemAliases.TextChanged -= ItemField_Changed;
        //chkIsPortable.Checked -= ItemField_Changed;
        //chkIsPortable.Unchecked -= ItemField_Changed;
        //chkIsContainer.Checked -= ItemField_Changed;
        //chkIsContainer.Unchecked -= ItemField_Changed;

        //txtItemId.Text = item.Id;
        //txtItemName.Text = item.Name;
        //txtItemDescription.Text = item.Description;
        //chkIsPortable.IsChecked = item.IsPortable ?? false;
        //chkIsContainer.IsChecked = item.IsContainer ?? false;

        //if (item.Aliases != null && item.Aliases.Count > 0)
        //{
        //    txtItemAliases.Text = string.Join(", ", item.Aliases);
        //}
        //else
        //{
        //    txtItemAliases.Text = string.Empty;
        //}

        //txtItemId.TextChanged += ItemField_Changed;
        //txtItemName.TextChanged += ItemField_Changed;
        //txtItemDescription.TextChanged += ItemField_Changed;
        //txtItemAliases.TextChanged += ItemField_Changed;
        //chkIsPortable.Checked += ItemField_Changed;
        //chkIsPortable.Unchecked += ItemField_Changed;
        //chkIsContainer.Checked += ItemField_Changed;
        //chkIsContainer.Unchecked += ItemField_Changed;
    }

    private void ItemField_Changed(object sender, RoutedEventArgs e)
    {
        ////if (_currentItem == null)
        ////    return;

        ////_currentItem.Id = txtItemId.Text;
        ////_currentItem.Name = txtItemName.Text;
        ////_currentItem.Description = txtItemDescription.Text;
        ////_currentItem.IsPortable = chkIsPortable.IsChecked == true ? true : null;
        ////_currentItem.IsContainer = chkIsContainer.IsChecked == true ? true : null;

        ////var aliasesText = txtItemAliases.Text.Trim();
        ////if (!string.IsNullOrWhiteSpace(aliasesText))
        ////{
        ////    _currentItem.Aliases = aliasesText.Split(',')
        ////        .Select(a => a.Trim())
        ////        .Where(a => !string.IsNullOrWhiteSpace(a))
        ////        .ToList();
        ////}
        ////else
        ////{
        ////    _currentItem.Aliases = null;
        ////}

        ////RefreshItemsList();
    }

    private void ClearItemEditor()
    {
        ////txtItemId.Clear();
        ////txtItemName.Clear();
        ////txtItemDescription.Clear();
        ////txtItemAliases.Clear();
        ////chkIsPortable.IsChecked = false;
        ////chkIsContainer.IsChecked = false;
        ////_currentItem = null;
    }

    private void RefreshItemsList()
    {
        ////if (_currentLocation == null)
        ////    return;

        ////var selectedItem = lstItems.SelectedItem;
        ////lstItems.ItemsSource = null;
        ////lstItems.ItemsSource = _currentLocation.Items;
        ////lstItems.SelectedItem = selectedItem;
    }

    private void ExportWorld()
    {
        //_world.Genre = (cmbGenre.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
        //_world.StartLocationId = txtStartLocationId.Text;

        //var saveFileDialog = new SaveFileDialog
        //{
        //    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
        //    FileName = "world.json",
        //    DefaultExt = ".json"
        //};

        //if (saveFileDialog.ShowDialog() == true)
        //{
        //    try
        //    {
        //        var options = new JsonSerializerOptions
        //        {
        //            WriteIndented = true,
        //            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        //        };

        //        var json = JsonSerializer.Serialize(_world, options);
        //        File.WriteAllText(saveFileDialog.FileName, json);

        //        MessageBox.Show("World saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error saving world: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}
    }

    private void btnChooseBoxart_Click(object sender, RoutedEventArgs e) => ChooseBoxart();

    private void btnChooseLabel_Click(object sender, RoutedEventArgs e) => ChooseLabel();

    private void btnExportWorld_Click(object sender, RoutedEventArgs e) => ExportWorld();
}
