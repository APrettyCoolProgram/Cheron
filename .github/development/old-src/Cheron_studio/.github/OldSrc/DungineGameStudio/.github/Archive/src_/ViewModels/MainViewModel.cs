using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using DungineStudio.Models;
using Microsoft.Win32;

namespace DungineStudio.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string? _boxArtTempPath;
        private string? _labelArtTempPath;
        private CartridgeMetadata? _loadedMetadata;
        private GameWorld? _currentWorld;
        private Location? _selectedLocation;
        private Item? _selectedItem;
        private string? _cartridgePath;
        private bool _hasUnsavedChanges;

        public MainViewModel()
        {
            Locations = new ObservableCollection<Location>();
            Items = new ObservableCollection<Item>();
            Exits = new ObservableCollection<ExitViewModel>();

            NewGameCommand = new RelayCommand(_ => NewGame());
            OpenCartridgeCommand = new RelayCommand(_ => OpenCartridge());
            SaveCartridgeCommand = new RelayCommand(_ => SaveCartridge(), _ => CurrentWorld != null);
            AddLocationCommand = new RelayCommand(_ => AddLocation());
            DeleteLocationCommand = new RelayCommand(_ => DeleteLocation(), _ => SelectedLocation != null);
            AddItemCommand = new RelayCommand(_ => AddItem(), _ => SelectedLocation != null);
            DeleteItemCommand = new RelayCommand(_ => DeleteItem(), _ => SelectedItem != null);
            AddExitCommand = new RelayCommand(_ => AddExit(), _ => SelectedLocation != null);
            DeleteExitCommand = new RelayCommand(_ => DeleteExit(SelectedExit), _ => SelectedExit != null);
        }

        private void ChooseBoxArt()
        {
            var dlg = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp;*.gif" };
            if (dlg.ShowDialog() == true)
            {
                _boxArtTempPath = dlg.FileName;
                OnPropertyChanged(nameof(BoxArtFullPath));
                HasUnsavedChanges = true;
            }
        }

        private void ChooseLabelArt()
        {
            var dlg = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp;*.gif" };
            if (dlg.ShowDialog() == true)
            {
                _labelArtTempPath = dlg.FileName;
                OnPropertyChanged(nameof(LabelArtFullPath));
                HasUnsavedChanges = true;
            }
        }

        public ObservableCollection<Location> Locations { get; }
        public ObservableCollection<Item> Items { get; }
        public ObservableCollection<ExitViewModel> Exits { get; }

        public GameWorld? CurrentWorld
        {
            get => _currentWorld;
            set
            {
                if (SetProperty(ref _currentWorld, value))
                {
                    OnPropertyChanged(nameof(Title));
                    OnPropertyChanged(nameof(StartLocationId));
                    OnPropertyChanged(nameof(Genre));
                    RefreshLocations();
                }
            }
        }

        public string Title
        {
            get => CurrentWorld?.Title ?? string.Empty;
            set
            {
                if (CurrentWorld != null && CurrentWorld.Title != value)
                {
                    CurrentWorld.Title = value;
                    OnPropertyChanged();
                    HasUnsavedChanges = true;
                }
            }
        }

        public string StartLocationId
        {
            get => CurrentWorld?.StartLocationId ?? string.Empty;
            set
            {
                if (CurrentWorld != null && CurrentWorld.StartLocationId != value)
                {
                    CurrentWorld.StartLocationId = value;
                    OnPropertyChanged();
                    HasUnsavedChanges = true;
                }
            }
        }

        public string Genre
        {
            get => CurrentWorld?.Genre ?? string.Empty;
            set
            {
                if (CurrentWorld != null && CurrentWorld.Genre != value)
                {
                    CurrentWorld.Genre = value;
                    OnPropertyChanged();
                    HasUnsavedChanges = true;
                }
            }
        }

        public Location? SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                if (SetProperty(ref _selectedLocation, value))
                {
                    OnPropertyChanged(nameof(LocationId));
                    OnPropertyChanged(nameof(LocationName));
                    OnPropertyChanged(nameof(LocationDescription));
                    RefreshItems();
                    RefreshExits();
                }
            }
        }

        public string LocationId
        {
            get => SelectedLocation?.Id ?? string.Empty;
            set
            {
                if (SelectedLocation != null && SelectedLocation.Id != value)
                {
                    SelectedLocation.Id = value;
                    OnPropertyChanged();
                    HasUnsavedChanges = true;
                }
            }
        }

        public string LocationName
        {
            get => SelectedLocation?.Name ?? string.Empty;
            set
            {
                if (SelectedLocation != null && SelectedLocation.Name != value)
                {
                    SelectedLocation.Name = value;
                    OnPropertyChanged();
                    HasUnsavedChanges = true;
                }
            }
        }

        public string LocationDescription
        {
            get => SelectedLocation?.Description ?? string.Empty;
            set
            {
                if (SelectedLocation != null && SelectedLocation.Description != value)
                {
                    SelectedLocation.Description = value;
                    OnPropertyChanged();
                    HasUnsavedChanges = true;
                }
            }
        }

        public Item? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    OnPropertyChanged(nameof(ItemId));
                    OnPropertyChanged(nameof(ItemName));
                    OnPropertyChanged(nameof(ItemDescription));
                    OnPropertyChanged(nameof(ItemIsPortable));
                    OnPropertyChanged(nameof(ItemAliases));
                }
            }
        }

        public string ItemId
        {
            get => SelectedItem?.Id ?? string.Empty;
            set
            {
                if (SelectedItem != null && SelectedItem.Id != value)
                {
                    SelectedItem.Id = value;
                    OnPropertyChanged();
                    HasUnsavedChanges = true;
                }
            }
        }

        public string ItemName
        {
            get => SelectedItem?.Name ?? string.Empty;
            set
            {
                if (SelectedItem != null && SelectedItem.Name != value)
                {
                    SelectedItem.Name = value;
                    OnPropertyChanged();
                    HasUnsavedChanges = true;
                }
            }
        }

        public string ItemDescription
        {
            get => SelectedItem?.Description ?? string.Empty;
            set
            {
                if (SelectedItem != null && SelectedItem.Description != value)
                {
                    SelectedItem.Description = value;
                    OnPropertyChanged();
                    HasUnsavedChanges = true;
                }
            }
        }

        public bool ItemIsPortable
        {
            get => SelectedItem?.IsPortable ?? true;
            set
            {
                if (SelectedItem != null && SelectedItem.IsPortable != value)
                {
                    SelectedItem.IsPortable = value;
                    OnPropertyChanged();
                    HasUnsavedChanges = true;
                }
            }
        }

        public string ItemAliases
        {
            get => SelectedItem?.Aliases != null ? string.Join(", ", SelectedItem.Aliases) : string.Empty;
            set
            {
                if (SelectedItem != null)
                {
                    var aliases = value.Split(',').Select(a => a.Trim()).Where(a => !string.IsNullOrEmpty(a)).ToList();
                    SelectedItem.Aliases = aliases.Any() ? aliases : null;
                    OnPropertyChanged();
                    HasUnsavedChanges = true;
                }
            }
        }

        private ExitViewModel? _selectedExit;
        public ExitViewModel? SelectedExit
        {
            get => _selectedExit;
            set => SetProperty(ref _selectedExit, value);
        }

        public bool HasUnsavedChanges
        {
            get => _hasUnsavedChanges;
            set => SetProperty(ref _hasUnsavedChanges, value);
        }

        public string? BoxArtFullPath =>
            !string.IsNullOrEmpty(_boxArtTempPath) ? _boxArtTempPath :
            (!string.IsNullOrEmpty(_cartridgePath) && _loadedMetadata?.BoxArt != null ? Path.Combine(_cartridgePath, _loadedMetadata.BoxArt) : null);

        public string? LabelArtFullPath =>
            !string.IsNullOrEmpty(_labelArtTempPath) ? _labelArtTempPath :
            (!string.IsNullOrEmpty(_cartridgePath) && _loadedMetadata?.LabelArt != null ? Path.Combine(_cartridgePath, _loadedMetadata.LabelArt) : null);


        public ICommand NewGameCommand { get; }
        public ICommand OpenCartridgeCommand { get; }
        public ICommand SaveCartridgeCommand { get; }
        public ICommand ChooseBoxArtCommand { get; }
        public ICommand ChooseLabelArtCommand { get; }
        public ICommand AddLocationCommand { get; }
        public ICommand DeleteLocationCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand DeleteItemCommand { get; }
        public ICommand AddExitCommand { get; }
        public ICommand DeleteExitCommand { get; }

        private void NewGame()
        {
            if (HasUnsavedChanges)
            {
                var result = MessageBox.Show("You have unsaved changes. Do you want to continue?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes) return;
            }

            CurrentWorld = new GameWorld
            {
                Title = "New Adventure",
                StartLocationId = "start",
                Genre = "Adventure"
            };
            _cartridgePath = null;
            HasUnsavedChanges = false;
        }

        private void OpenCartridge()
        {
            if (HasUnsavedChanges)
            {
                var result = MessageBox.Show("You have unsaved changes. Do you want to continue?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes) return;
            }

            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Open Cartridge (world.json)",
                Filter = "World JSON|world.json|All Files|*.*",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var json = File.ReadAllText(dialog.FileName);
                    var world = JsonSerializer.Deserialize<GameWorld>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    if (world != null)
                    {
                        CurrentWorld = world;
                        _cartridgePath = Path.GetDirectoryName(dialog.FileName);
                        // try load cartridge.json metadata
                        try
                        {
                            var metaPath = Path.Combine(_cartridgePath!, "cartridge.json");
                            if (File.Exists(metaPath))
                            {
                                var metaJson = File.ReadAllText(metaPath);
                                _loadedMetadata = JsonSerializer.Deserialize<CartridgeMetadata>(metaJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                            }
                        }
                        catch { }
                        HasUnsavedChanges = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load cartridge: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveCartridge()
        {
            if (CurrentWorld == null) return;

            if (string.IsNullOrEmpty(_cartridgePath))
            {
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Title = "Save Cartridge",
                    Filter = "World JSON|world.json",
                    FileName = "world.json",
                    InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
                };

                if (dialog.ShowDialog() == true)
                {
                    _cartridgePath = Path.GetDirectoryName(dialog.FileName);
                }
                else
                {
                    return;
                }
            }

            try
            {
                var worldPath = Path.Combine(_cartridgePath!, "world.json");
                var json = JsonSerializer.Serialize(CurrentWorld, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(worldPath, json);
                // save cartridge metadata and copy any selected images into the cartridge folder
                try
                {
                    var meta = _loadedMetadata ?? new CartridgeMetadata();

                    if (!string.IsNullOrEmpty(_boxArtTempPath))
                    {
                        var destName = "box" + Path.GetExtension(_boxArtTempPath);
                        var destPath = Path.Combine(_cartridgePath!, destName);
                        File.Copy(_boxArtTempPath, destPath, true);
                        meta.BoxArt = destName;
                    }

                    if (!string.IsNullOrEmpty(_labelArtTempPath))
                    {
                        var destName = "label" + Path.GetExtension(_labelArtTempPath);
                        var destPath = Path.Combine(_cartridgePath!, destName);
                        File.Copy(_labelArtTempPath, destPath, true);
                        meta.LabelArt = destName;
                    }

                    var metaPath = Path.Combine(_cartridgePath!, "cartridge.json");
                    var metaJson = JsonSerializer.Serialize(meta, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(metaPath, metaJson);
                    _loadedMetadata = meta;
                    _boxArtTempPath = null;
                    _labelArtTempPath = null;
                    OnPropertyChanged(nameof(BoxArtFullPath));
                    OnPropertyChanged(nameof(LabelArtFullPath));
                }
                catch { }
                // create a README.md similar to HauntedMansion structure
                try
                {
                    var readmePath = Path.Combine(_cartridgePath!, "README.md");
                    var title = !string.IsNullOrEmpty(CurrentWorld.Title) ? CurrentWorld.Title : Path.GetFileName(_cartridgePath!);
                    var content = $"# {title} - {CurrentWorld.Genre} Adventure\n\n" +
                                  $"**Genre:** {CurrentWorld.Genre}  \n\n" +
                                  "## Story Overview\n\n" +
                                  "Describe your game here.\n\n" +
                                  "## Key Locations\n\n" +
                                  "- Entrance Hall\n\n" +
                                  "## Items & Puzzles\n\n" +
                                  "- Add items and puzzles here.\n\n" +
                                  "## Atmosphere\n\n" +
                                  "Describe the atmosphere.\n\n" +
                                  "## Content Warning\n\n" +
                                  "Describe any content warnings.\n";

                    File.WriteAllText(readmePath, content);
                }
                catch { }
                HasUnsavedChanges = false;
                MessageBox.Show("Cartridge saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save cartridge: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddLocation()
        {
            if (CurrentWorld == null) return;

            var newLocation = new Location
            {
                Id = $"location_{CurrentWorld.Locations.Count + 1}",
                Name = "New Location",
                Description = "A new location."
            };

            CurrentWorld.Locations.Add(newLocation);
            RefreshLocations();
            SelectedLocation = newLocation;
            HasUnsavedChanges = true;
        }

        private void DeleteLocation()
        {
            if (CurrentWorld == null || SelectedLocation == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete '{SelectedLocation.Name}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                CurrentWorld.Locations.Remove(SelectedLocation);
                RefreshLocations();
                SelectedLocation = null;
                HasUnsavedChanges = true;
            }
        }

        private void AddItem()
        {
            if (SelectedLocation == null) return;

            var newItem = new Item
            {
                Id = $"item_{SelectedLocation.Items.Count + 1}",
                Name = "New Item",
                Description = "A new item."
            };

            SelectedLocation.Items.Add(newItem);
            RefreshItems();
            SelectedItem = newItem;
            HasUnsavedChanges = true;
        }

        private void DeleteItem()
        {
            if (SelectedLocation == null || SelectedItem == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete '{SelectedItem.Name}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                SelectedLocation.Items.Remove(SelectedItem);
                RefreshItems();
                SelectedItem = null;
                HasUnsavedChanges = true;
            }
        }

        private void AddExit()
        {
            if (SelectedLocation == null) return;

            var dialog = new Views.AddExitDialog();
            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.Direction) && !string.IsNullOrWhiteSpace(dialog.TargetLocationId))
            {
                SelectedLocation.Exits[dialog.Direction] = dialog.TargetLocationId;
                RefreshExits();
                HasUnsavedChanges = true;
            }
        }

        private void DeleteExit(ExitViewModel? exit)
        {
            if (SelectedLocation == null || exit == null) return;

            SelectedLocation.Exits.Remove(exit.Direction);
            RefreshExits();
            HasUnsavedChanges = true;
        }

        private void RefreshLocations()
        {
            Locations.Clear();
            if (CurrentWorld?.Locations != null)
            {
                foreach (var location in CurrentWorld.Locations)
                {
                    Locations.Add(location);
                }
            }
        }

        private void RefreshItems()
        {
            Items.Clear();
            if (SelectedLocation?.Items != null)
            {
                foreach (var item in SelectedLocation.Items)
                {
                    Items.Add(item);
                }
            }
        }

        private void RefreshExits()
        {
            Exits.Clear();
            if (SelectedLocation?.Exits != null)
            {
                foreach (var exit in SelectedLocation.Exits)
                {
                    Exits.Add(new ExitViewModel { Direction = exit.Key, TargetLocationId = exit.Value });
                }
            }
        }
    }

    public class ExitViewModel
    {
        public string Direction { get; set; } = string.Empty;
        public string TargetLocationId { get; set; } = string.Empty;
    }
}
