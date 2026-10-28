using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using TextGame.Models;

namespace Ubiquitous.GameInterface.CartridgeCreator
{
    /// <summary>
    /// Window for creating new TextGame cartridges with full room and item editing
    /// </summary>
    public partial class CartridgeCreatorWindow : Window
    {
        private ObservableCollection<RoomViewModel> _rooms = new();
        private ObservableCollection<ItemViewModel> _items = new();
        private RoomViewModel? _selectedRoom;
        private ItemViewModel? _selectedItem;

        /// <summary>
        /// View model for a room in the editor
        /// </summary>
        public class RoomViewModel
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public ObservableCollection<ExitViewModel> Exits { get; set; } = new();
            public ObservableCollection<string> ItemIds { get; set; } = new();
        }

        /// <summary>
        /// View model for an exit in the editor
        /// </summary>
        public class ExitViewModel
        {
            public string Direction { get; set; } = string.Empty;
            public string DestinationRoomId { get; set; } = string.Empty;
        }

        /// <summary>
        /// View model for an item in the editor
        /// </summary>
        public class ItemViewModel
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public bool CanTake { get; set; } = true;
            public string UseDescription { get; set; } = "Nothing happens.";
        }

        /// <summary>
        /// Initializes a new instance of the CartridgeCreatorWindow
        /// </summary>
        public CartridgeCreatorWindow()
        {
            InitializeComponent();
            RoomListBox.ItemsSource = _rooms;
            ItemListBox.ItemsSource = _items;
            
            AddDefaultRoom();
        }

        /// <summary>
        /// Adds a default starting room
        /// </summary>
        private void AddDefaultRoom()
        {
            var defaultRoom = new RoomViewModel
            {
                Id = "starting_room",
                Name = "Starting Room",
                Description = "You find yourself at the beginning of your adventure..."
            };
            _rooms.Add(defaultRoom);
            StartingRoomIdComboBox.ItemsSource = _rooms.Select(r => r.Id).ToList();
            StartingRoomIdComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the Add Room button click
        /// </summary>
        private void AddRoomButton_Click(object sender, RoutedEventArgs e)
        {
            var roomNumber = _rooms.Count + 1;
            var newRoom = new RoomViewModel
            {
                Id = $"room_{roomNumber}",
                Name = $"Room {roomNumber}",
                Description = "A new room waiting to be described..."
            };
            _rooms.Add(newRoom);
            RoomListBox.SelectedItem = newRoom;
            UpdateStartingRoomComboBox();
        }

        /// <summary>
        /// Handles room selection changes
        /// </summary>
        private void RoomListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoomListBox.SelectedItem is RoomViewModel room)
            {
                _selectedRoom = room;
                ShowRoomEditor(room);
            }
        }

        /// <summary>
        /// Shows the room editor for the selected room
        /// </summary>
        private void ShowRoomEditor(RoomViewModel room)
        {
            RoomEditorPanel.Children.Clear();

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            int row = 0;

            // Room ID
            AddEditorRow(grid, row++, "Room ID:", CreateTextBox(room.Id, text => room.Id = text));

            // Room Name
            AddEditorRow(grid, row++, "Name:", CreateTextBox(room.Name, text => room.Name = text));

            // Description
            var descBox = CreateMultilineTextBox(room.Description, text => room.Description = text);
            AddEditorRow(grid, row++, "Description:", descBox);

            // Exits Section
            var exitsHeader = new TextBlock
            {
                Text = "Exits",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 20, 0, 10)
            };
            Grid.SetRow(exitsHeader, row++);
            Grid.SetColumnSpan(exitsHeader, 2);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.Children.Add(exitsHeader);

            // Exits List
            var exitsPanel = new StackPanel();
            foreach (var exit in room.Exits)
            {
                var exitPanel = CreateExitEditor(exit, room);
                exitsPanel.Children.Add(exitPanel);
            }

            var addExitButton = new Button
            {
                Content = "+ Add Exit",
                Background = System.Windows.Media.Brushes.Green,
                Foreground = System.Windows.Media.Brushes.White,
                Padding = new Thickness(10, 5, 10, 5),
                Margin = new Thickness(0, 5, 0, 0),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            addExitButton.Click += (s, e) =>
            {
                var newExit = new ExitViewModel { Direction = "north", DestinationRoomId = "" };
                room.Exits.Add(newExit);
                ShowRoomEditor(room);
            };
            exitsPanel.Children.Add(addExitButton);

            Grid.SetRow(exitsPanel, row++);
            Grid.SetColumn(exitsPanel, 1);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.Children.Add(exitsPanel);

            // Items Section
            var itemsHeader = new TextBlock
            {
                Text = "Items in this Room",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 20, 0, 10)
            };
            Grid.SetRow(itemsHeader, row++);
            Grid.SetColumnSpan(itemsHeader, 2);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.Children.Add(itemsHeader);

            var itemsComboBox = new ComboBox
            {
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(63, 63, 70)),
                Foreground = System.Windows.Media.Brushes.White,
                Padding = new Thickness(8),
                Margin = new Thickness(0, 5, 0, 5)
            };
            itemsComboBox.ItemsSource = _items.Select(i => i.Id).ToList();
            
            var addItemToRoomButton = new Button
            {
                Content = "Add Item to Room",
                Background = System.Windows.Media.Brushes.Green,
                Foreground = System.Windows.Media.Brushes.White,
                Padding = new Thickness(10, 5, 10, 5),
                Margin = new Thickness(10, 5, 0, 0),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            addItemToRoomButton.Click += (s, e) =>
            {
                if (itemsComboBox.SelectedItem is string itemId && !string.IsNullOrEmpty(itemId))
                {
                    if (!room.ItemIds.Contains(itemId))
                    {
                        room.ItemIds.Add(itemId);
                        ShowRoomEditor(room);
                    }
                }
            };

            var itemSelectionPanel = new StackPanel { Orientation = Orientation.Horizontal };
            itemSelectionPanel.Children.Add(itemsComboBox);
            itemSelectionPanel.Children.Add(addItemToRoomButton);

            Grid.SetRow(itemSelectionPanel, row++);
            Grid.SetColumn(itemSelectionPanel, 1);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.Children.Add(itemSelectionPanel);

            // Current Items List
            if (room.ItemIds.Any())
            {
                var currentItemsPanel = new StackPanel { Margin = new Thickness(0, 5, 0, 0) };
                foreach (var itemId in room.ItemIds.ToList())
                {
                    var itemPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 2, 0, 2) };
                    var itemText = new TextBlock
                    {
                        Text = itemId,
                        Foreground = System.Windows.Media.Brushes.White,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 0, 10, 0)
                    };
                    var removeButton = new Button
                    {
                        Content = "Remove",
                        Background = System.Windows.Media.Brushes.Red,
                        Foreground = System.Windows.Media.Brushes.White,
                        Padding = new Thickness(5, 2, 5, 2),
                        Cursor = System.Windows.Input.Cursors.Hand
                    };
                    var currentItemId = itemId;
                    removeButton.Click += (s, e) =>
                    {
                        room.ItemIds.Remove(currentItemId);
                        ShowRoomEditor(room);
                    };
                    itemPanel.Children.Add(itemText);
                    itemPanel.Children.Add(removeButton);
                    currentItemsPanel.Children.Add(itemPanel);
                }

                Grid.SetRow(currentItemsPanel, row++);
                Grid.SetColumn(currentItemsPanel, 1);
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.Children.Add(currentItemsPanel);
            }

            // Delete Room Button
            var deleteButton = new Button
            {
                Content = "Delete Room",
                Background = System.Windows.Media.Brushes.Red,
                Foreground = System.Windows.Media.Brushes.White,
                Padding = new Thickness(20, 10, 20, 10),
                Margin = new Thickness(0, 30, 0, 0),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            deleteButton.Click += (s, e) => DeleteRoom(room);
            Grid.SetRow(deleteButton, row++);
            Grid.SetColumnSpan(deleteButton, 2);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.Children.Add(deleteButton);

            RoomEditorPanel.Children.Add(grid);
        }

        /// <summary>
        /// Creates an exit editor panel
        /// </summary>
        private StackPanel CreateExitEditor(ExitViewModel exit, RoomViewModel room)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 5, 0, 5)
            };

            var directionBox = new TextBox
            {
                Text = exit.Direction,
                Width = 100,
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(63, 63, 70)),
                Foreground = System.Windows.Media.Brushes.White,
                Padding = new Thickness(5),
                Margin = new Thickness(0, 0, 10, 0)
            };
            directionBox.TextChanged += (s, e) => exit.Direction = directionBox.Text;

            var destComboBox = new ComboBox
            {
                Width = 200,
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(63, 63, 70)),
                Foreground = System.Windows.Media.Brushes.White,
                Padding = new Thickness(5),
                Margin = new Thickness(0, 0, 10, 0),
                IsEditable = true
            };
            destComboBox.ItemsSource = _rooms.Select(r => r.Id).ToList();
            destComboBox.Text = exit.DestinationRoomId;
            destComboBox.SelectionChanged += (s, e) =>
            {
                if (destComboBox.SelectedItem is string roomId)
                    exit.DestinationRoomId = roomId;
            };
            destComboBox.LostFocus += (s, e) => exit.DestinationRoomId = destComboBox.Text;

            var removeButton = new Button
            {
                Content = "Remove",
                Background = System.Windows.Media.Brushes.Red,
                Foreground = System.Windows.Media.Brushes.White,
                Padding = new Thickness(10, 5, 10, 5),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            removeButton.Click += (s, e) =>
            {
                room.Exits.Remove(exit);
                ShowRoomEditor(room);
            };

            panel.Children.Add(new TextBlock { Text = "Direction:", Foreground = System.Windows.Media.Brushes.White, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 5, 0) });
            panel.Children.Add(directionBox);
            panel.Children.Add(new TextBlock { Text = "To:", Foreground = System.Windows.Media.Brushes.White, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 5, 0) });
            panel.Children.Add(destComboBox);
            panel.Children.Add(removeButton);

            return panel;
        }

        /// <summary>
        /// Handles the Add Item button click
        /// </summary>
        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            var itemNumber = _items.Count + 1;
            var newItem = new ItemViewModel
            {
                Id = $"item_{itemNumber}",
                Name = $"item {itemNumber}",
                Description = "A new item waiting to be described...",
                CanTake = true,
                UseDescription = "Nothing happens."
            };
            _items.Add(newItem);
            ItemListBox.SelectedItem = newItem;
        }

        /// <summary>
        /// Handles item selection changes
        /// </summary>
        private void ItemListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemListBox.SelectedItem is ItemViewModel item)
            {
                _selectedItem = item;
                ShowItemEditor(item);
            }
        }

        /// <summary>
        /// Shows the item editor for the selected item
        /// </summary>
        private void ShowItemEditor(ItemViewModel item)
        {
            ItemEditorPanel.Children.Clear();

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            int row = 0;

            AddEditorRow(grid, row++, "Item ID:", CreateTextBox(item.Id, text => item.Id = text));
            AddEditorRow(grid, row++, "Name:", CreateTextBox(item.Name, text => item.Name = text));
            AddEditorRow(grid, row++, "Description:", CreateMultilineTextBox(item.Description, text => item.Description = text));

            var canTakeCheckBox = new CheckBox
            {
                IsChecked = item.CanTake,
                Foreground = System.Windows.Media.Brushes.White,
                VerticalAlignment = VerticalAlignment.Center
            };
            canTakeCheckBox.Checked += (s, e) => item.CanTake = true;
            canTakeCheckBox.Unchecked += (s, e) => item.CanTake = false;
            AddEditorRow(grid, row++, "Can Take:", canTakeCheckBox);

            AddEditorRow(grid, row++, "Use Description:", CreateMultilineTextBox(item.UseDescription, text => item.UseDescription = text));

            var deleteButton = new Button
            {
                Content = "Delete Item",
                Background = System.Windows.Media.Brushes.Red,
                Foreground = System.Windows.Media.Brushes.White,
                Padding = new Thickness(20, 10, 20, 10),
                Margin = new Thickness(0, 20, 0, 0),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            deleteButton.Click += (s, e) => DeleteItem(item);
            Grid.SetRow(deleteButton, row++);
            Grid.SetColumnSpan(deleteButton, 2);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.Children.Add(deleteButton);

            ItemEditorPanel.Children.Add(grid);
        }

        /// <summary>
        /// Adds a row to the editor grid
        /// </summary>
        private void AddEditorRow(Grid grid, int row, string label, UIElement control)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var labelBlock = new TextBlock
            {
                Text = label,
                FontSize = 14,
                Foreground = System.Windows.Media.Brushes.White,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 10, 0, 5)
            };
            Grid.SetRow(labelBlock, row);
            Grid.SetColumn(labelBlock, 0);
            grid.Children.Add(labelBlock);

            control.SetValue(Grid.RowProperty, row);
            control.SetValue(Grid.ColumnProperty, 1);
            control.SetValue(FrameworkElement.MarginProperty, new Thickness(10, 5, 0, 5));
            grid.Children.Add(control);
        }

        /// <summary>
        /// Creates a text box with change handler
        /// </summary>
        private TextBox CreateTextBox(string initialValue, Action<string> onChanged)
        {
            var textBox = new TextBox
            {
                Text = initialValue,
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(63, 63, 70)),
                Foreground = System.Windows.Media.Brushes.White,
                Padding = new Thickness(8),
                FontSize = 14
            };
            textBox.TextChanged += (s, e) => onChanged(textBox.Text);
            return textBox;
        }

        /// <summary>
        /// Creates a multiline text box with change handler
        /// </summary>
        private TextBox CreateMultilineTextBox(string initialValue, Action<string> onChanged)
        {
            var textBox = new TextBox
            {
                Text = initialValue,
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(63, 63, 70)),
                Foreground = System.Windows.Media.Brushes.White,
                Padding = new Thickness(8),
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                Height = 100,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            textBox.TextChanged += (s, e) => onChanged(textBox.Text);
            return textBox;
        }

        /// <summary>
        /// Deletes a room
        /// </summary>
        private void DeleteRoom(RoomViewModel room)
        {
            if (_rooms.Count == 1)
            {
                MessageBox.Show("Cannot delete the last room. A cartridge must have at least one room.", "Cannot Delete", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete '{room.Name}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _rooms.Remove(room);
                UpdateStartingRoomComboBox();
                RoomEditorPanel.Children.Clear();
                RoomEditorPanel.Children.Add(new TextBlock
                {
                    Text = "Select or add a room to edit",
                    Foreground = System.Windows.Media.Brushes.Gray,
                    FontSize = 16,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 50, 0, 0)
                });
            }
        }

        /// <summary>
        /// Deletes an item
        /// </summary>
        private void DeleteItem(ItemViewModel item)
        {
            var result = MessageBox.Show($"Are you sure you want to delete '{item.Name}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _items.Remove(item);
                foreach (var room in _rooms)
                {
                    room.ItemIds.Remove(item.Id);
                }
                ItemEditorPanel.Children.Clear();
                ItemEditorPanel.Children.Add(new TextBlock
                {
                    Text = "Select or add an item to edit",
                    Foreground = System.Windows.Media.Brushes.Gray,
                    FontSize = 16,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 50, 0, 0)
                });
            }
        }

        /// <summary>
        /// Updates the starting room combo box
        /// </summary>
        private void UpdateStartingRoomComboBox()
        {
            var currentSelection = StartingRoomIdComboBox.Text;
            StartingRoomIdComboBox.ItemsSource = _rooms.Select(r => r.Id).ToList();
            if (_rooms.Any(r => r.Id == currentSelection))
            {
                StartingRoomIdComboBox.Text = currentSelection;
            }
            else
            {
                StartingRoomIdComboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Handles the Create button click
        /// </summary>
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }

            try
            {
                var cartridge = CreateCartridgeData();
                SaveCartridge(cartridge);

                MessageBox.Show(
                    $"Cartridge '{cartridge.Title}' has been created successfully!\n\n" +
                    $"Rooms: {cartridge.Rooms.Count}\n" +
                    $"Items: {cartridge.Items.Count}",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                ReturnToMainWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error creating cartridge: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Validates user input
        /// </summary>
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Please enter a game title.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                MainTabControl.SelectedIndex = 0;
                TitleTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(AuthorTextBox.Text))
            {
                MessageBox.Show("Please enter an author name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                MainTabControl.SelectedIndex = 0;
                AuthorTextBox.Focus();
                return false;
            }

            if (_rooms.Count == 0)
            {
                MessageBox.Show("Please add at least one room.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                MainTabControl.SelectedIndex = 1;
                return false;
            }

            var startingRoomId = StartingRoomIdComboBox.Text;
            if (!_rooms.Any(r => r.Id == startingRoomId))
            {
                MessageBox.Show("The starting room ID must match one of your room IDs.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                MainTabControl.SelectedIndex = 0;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates CartridgeData from the editor data
        /// </summary>
        private CartridgeData CreateCartridgeData()
        {
            var cartridge = new CartridgeData
            {
                Title = TitleTextBox.Text.Trim(),
                Author = AuthorTextBox.Text.Trim(),
                Version = VersionTextBox.Text.Trim(),
                Description = DescriptionTextBox.Text.Trim(),
                Type = "TextGame",
                UIType = ((ComboBoxItem)UITypeComboBox.SelectedItem).Content.ToString() ?? "Basic",
                Difficulty = ((ComboBoxItem)DifficultyComboBox.SelectedItem).Content.ToString() ?? "Easy",
                Genre = ((ComboBoxItem)GenreComboBox.SelectedItem).Content.ToString() ?? "Fantasy",
                StartingRoomId = StartingRoomIdComboBox.Text.Trim(),
                Rooms = new List<RoomData>(),
                Items = new List<ItemData>()
            };

            foreach (var room in _rooms)
            {
                cartridge.Rooms.Add(new RoomData
                {
                    Id = room.Id,
                    Name = room.Name,
                    Description = room.Description,
                    Exits = room.Exits.Select(e => new ExitData
                    {
                        Direction = e.Direction,
                        DestinationRoomId = e.DestinationRoomId
                    }).ToList(),
                    ItemIds = room.ItemIds.ToList()
                });
            }

            foreach (var item in _items)
            {
                cartridge.Items.Add(new ItemData
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    CanTake = item.CanTake,
                    UseDescription = item.UseDescription
                });
            }

            return cartridge;
        }

        /// <summary>
        /// Saves the cartridge to a JSON file
        /// </summary>
        private void SaveCartridge(CartridgeData cartridge)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var cartridgeDirectory = Path.Combine(baseDirectory, "Cartridge");

            if (!Directory.Exists(cartridgeDirectory))
            {
                Directory.CreateDirectory(cartridgeDirectory);
            }

            var sanitizedTitle = string.Join("_", cartridge.Title.Split(Path.GetInvalidFileNameChars()));
            var fileName = $"{sanitizedTitle}.json";
            var folderName = $"{sanitizedTitle}.ucart";
            var targetDirectory = Path.Combine(cartridgeDirectory, folderName);

            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            var filePath = Path.Combine(targetDirectory, fileName);

            if (File.Exists(filePath))
            {
                var result = MessageBox.Show(
                    $"A cartridge with this name already exists. Do you want to overwrite it?",
                    "File Exists",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    throw new InvalidOperationException("Cartridge creation cancelled.");
                }
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(cartridge, options);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Handles the Cancel button click
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ReturnToMainWindow();
        }

        /// <summary>
        /// Returns to the main window
        /// </summary>
        private void ReturnToMainWindow()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
    }
}
