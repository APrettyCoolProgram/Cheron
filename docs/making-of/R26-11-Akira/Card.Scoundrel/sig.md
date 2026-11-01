# Scoundrel Implementation Guide for Cheron

## Purpose and scope

This document is an implementation blueprint for adding **Scoundrel** to the current Cheron workspace. It is based on the source tree under `src/` and the authoritative rules in [Scoundrel-Instructions.md](Scoundrel-Instructions.md). It intentionally does **not** implement the game.

The repository has one SDK-style application project, `src/Cheron.csproj`, rather than separate `Cheron.Engine` and `Cheron.Games.Card` projects. The engine is the `Cheron.Engine` namespace in `src/Engine/`; Solitaire is the existing card-game reference implementation in `src/Games/Card/Solitaire/`.

> **Important distinction:** There is no reusable `Cheron.Games.Card` base library in this repository. `SolitaireCard`, `SolitaireSuit`, `SolitaireDealBuilder`, `SolitaireLayout`, and `SolitaireArtwork` are all Solitaire-specific. Scoundrel should follow their conventions but own analogous types in its own namespace; it must not make its rules depend on Solitaire internals.

---

## 1. Existing architecture findings

### 1.1 Engine module lifecycle

The host contract is `internal interface IGameModule : IDisposable` in [`src/Engine/IGameModule.cs`](../../../../src/Engine/IGameModule.cs). A game module must provide:

```csharp
string Title { get; }
Point PreferredWindowSize { get; }
void Initialize();
void SetViewport(GameViewport viewport);
void LoadContent(GraphicsDevice graphicsDevice);
void Update(GameTime gameTime);
void Draw(GameTime gameTime, GraphicsDevice graphicsDevice);
```

`Program.cs` creates `new GameHost(new StartScreenGame())` and calls `Run()`. `GameHost` owns the MonoGame application lifecycle:

1. `GameHost.Initialize()` calls `_gameModule.Initialize()` before graphics resources exist.
2. `GameHost.LoadContent()` computes a `GameViewport`, calls `SetViewport(viewport)`, then calls `LoadContent(GraphicsDevice)`.
3. Each frame, `GameHost.Update()` exits on `Escape`/gamepad Back, refreshes the viewport, and calls the module's `Update(gameTime)`.
4. Each draw, `GameHost.Draw()` refreshes the viewport and calls `Draw(gameTime, GraphicsDevice)`.
5. `GameHost.Dispose(bool)` calls the module's `Dispose()` exactly once before the base `Game` disposal.

`GameViewport` in [`src/Engine/GameViewport.cs`](../../../../src/Engine/GameViewport.cs) is the existing fixed-logical-canvas solution. Use `viewport.Transform` in `SpriteBatch.Begin(...)` and `viewport.ToLogical(clientPoint)` before hit testing. It accounts for DPI and preserves out-of-board coordinates so normal `Rectangle.Contains` checks reject clicks in letterbox margins.

### 1.2 Actual cartridge registration convention

There is no `Cartridge` class or registration interface. The visual cartridge picker is itself the engine module `internal sealed class StartScreenGame` in [`src/Engine/StartScreenGame.cs`](../../../../src/Engine/StartScreenGame.cs).

Its lifecycle is the practical integration reference:

- `_cartridges` is a `List<StartScreenCartridge>` initialized with labels, a `StartScreenGameKind` enum value, and visual positions (lines 20-24).
- After an inserted cartridge powers on, `TransitionToInsertedGame()` switches on `StartScreenGameKind` (lines 458-492).
- It constructs the selected `IGameModule` (`new SolitaireGame()` for Solitaire), then explicitly calls `Initialize()`, `SetViewport(new GameViewport(...))`, and `LoadContent(_pixel.GraphicsDevice)` before storing it in `_activeGame`.
- While `_activeGame` is non-null, the start screen delegates `Update`, `Draw`, `Title`, `PreferredWindowSize`, `SetViewport`, and ultimately `Dispose` to it.

For Scoundrel, the requested “cartridge” is therefore a new `StartScreenCartridge` entry plus a `StartScreenGameKind.Scoundrel` value and switch arm. Do **not** create an invented cartridge base class.

### 1.3 Representative complete Solitaire lifecycle

`internal sealed partial class SolitaireGame : IGameModule` in [`src/Games/Card/Solitaire/SolitaireGame.cs`](../../../../src/Games/Card/Solitaire/SolitaireGame.cs) is the closest architectural reference.

- **Construction:** its constructor fills a variant-strategy map and creates `SolitaireRenderer` and `SolitaireInteractionController` delegates (lines 85-94).
- **Initialization:** `Initialize()` calls `StartNewGame()` (lines 108-112). `StartNewGame()` clears all pile/state collections, resets transient interaction state, creates `SolitaireDealBuilder.CreateShuffledDeck(new Random())`, gives it to the active variant behavior, and starts deal animation (lines 1600-1618).
- **Content:** `LoadContent(GraphicsDevice)` owns and creates `SpriteBatch`, a one-pixel `Texture2D`, bitmap glyph texture, `SolitaireArtwork`, and a scissor-enabled `RasterizerState` (lines 407-416). These are module-owned; the `GraphicsDevice` is borrowed from the host.
- **Viewport/input/update:** `Update(GameTime)` advances controls, animation, and feedback; reads keyboard and mouse input; then maps client mouse coordinates through the interaction controller (lines 420-446). `SolitaireInteractionController.UpdateClientPointer` calls `_game._viewport.ToLogical(...)` before all board hit tests (lines 27-30 of [`SolitaireInteractionController.cs`](../../../../src/Games/Card/Solitaire/SolitaireInteractionController.cs)).
- **Rendering:** `Draw` delegates to `SolitaireRenderer.Draw`. Its renderer clears the device, limits drawing to `_viewport.ScissorBounds`, and begins sprite batches with the viewport transform (lines 57-80 of [`SolitaireRenderer.cs`](../../../../src/Games/Card/Solitaire/SolitaireRenderer.cs)).
- **Completion:** `_hasWon` is game state, set after valid moves; the renderer draws the completion UI. It is not a separate engine scene.
- **Disposal:** `Dispose()` releases the game-owned sounds, rasterizer state, artwork, glyph texture, pixel texture, and sprite batch, but never disposes the graphics device (lines 461-469).

Scoundrel should use the same direct module lifecycle. It does not need a scene manager, component framework, external asset loader, or an engine change.

### 1.4 Reusable patterns, not reusable card code

Existing Solitaire source establishes these conventions:

| Existing item | What Scoundrel should reuse | What it must not assume |
|---|---|---|
| `SolitaireCard` / `SolitaireSuit` | Small internal card identity object; rank 1-13 and suit enum; mutable `FaceUp` only when needed | The classes are internal and live in the Solitaire namespace; no generic card abstraction exists. |
| `SolitaireDealBuilder.CreateShuffledDeck(Random)` | `List<T>` deck whose next draw is the **end** of the list; Fisher-Yates with supplied `Random` | Its ordered deck is all 52 cards and is private; it cannot produce Scoundrel’s filtered deck. |
| `SolitaireLayout` | Constants and `Rectangle` factories centralized in one static internal class | The Solitaire board geometry is specific to seven columns/foundations. |
| `SolitaireArtwork` | Graphics-thread texture construction and an `IDisposable` owner | Its card face/back/theme are not shared public assets. |
| `SolitaireRenderer` / `SolitaireInteractionController` | Renderer/controller as nested private delegates in partial files, keeping `SolitaireGame` state private | Scoundrel should not duplicate drag/drop/foundation code. |
| `SolitaireBitmapFont` | Existing internal bitmap text utility can be used if made accessible or if Scoundrel is built in the same assembly; existing games call it directly | It is under the Solitaire namespace, so using it is a cross-game presentation dependency; evaluate whether that is acceptable before committing. |

`SolitaireCard` uses rank `1` for Ace through `13` for King (`SolitaireCard.cs`). Scoundrel’s **display rank** can follow this, but its gameplay value must map black Aces to `14`; do not store Ace as `14` in a standard-deck builder unless every label/rendering path is deliberately updated.

The old [`src/Games/Card/Solitaire/StartScreenGame.cs`](../../../../src/Games/Card/Solitaire/StartScreenGame.cs) is a console placeholder unrelated to the engine picker. Do not copy it or treat it as a cartridge integration point.

---

## 2. Scoundrel rules translated to mechanics

### 2.1 Authoritative stated rules

The rules file states:

- Single-player dungeon card game; win by resolving every dungeon card and lose immediately at health `<= 0`.
- Starting/max health is `20`.
- The dungeon is a modified standard deck: remove red J/Q/K, red Aces, and Jokers; shuffle before play.
- Black cards (Spades, Clubs) are monsters. Values are 2-10, Jack 11, Queen 12, King 13, Ace 14.
- Diamonds are weapons and Hearts are potions; available red cards have values 2-10 because their Aces and face cards were removed.
- A room is four face-up cards. Resolve any three in any order, discard resolved cards, and carry the fourth face-up card into the next room.
- Before resolving a room, the player may avoid it by putting its four cards at the dungeon bottom. Avoiding cannot occur twice in succession.
- Taking a weapon equips it and discards an already-equipped weapon. Weapon damage equals weapon value. After a weapon kills a monster of value `V`, it may only fight monsters with value `< V`.
- A monster can be fought barehanded (take full value) or with an allowed weapon (take `max(0, monsterValue - weaponValue)`).
- A potion restores its value up to 20, but only one potion heals in a room. Additional room potions may be resolved without healing.

### 2.2 Explicit state machine

Use `ScoundrelGameStatus` with at least:

```text
NotStarted -> Playing -> Won
					 -> Lost
```

While `Playing`, model room state separately:

```text
NeedRoom -> ResolvingRoom -> NeedRoom
						 -> Won / Lost
```

Recommended invariant for normal rooms:

- `RoomCards.Count == 4` when `RoomPhase == ResolvingRoom`.
- `ResolvedInRoom` is 0 through 3.
- `PotionUsedInRoom` resets to `false` whenever a new room is made or avoided.
- After each successful resolution: move that card from `RoomCards` to `DiscardPile`, increment `ResolvedInRoom`, and immediately evaluate loss. When it reaches 3, leave the remaining room card in place, reset room-local counters, and draw enough cards to restore a room of four.
- `AvoidedPreviousRoom` is set `true` only after a successful avoidance and reset `false` after the player resolves any card. This enforces “never two in a row.”

### 2.3 Rule-to-operation table

| Player operation | Preconditions | State mutation | Invalid outcome |
|---|---|---|---|
| Avoid room | Playing; room has no resolved cards; `AvoidedPreviousRoom == false` | Move all room cards to dungeon bottom in a documented order; clear room; set avoidance flag; draw next room | No state change and status text such as `CANNOT AVOID THIS ROOM`. |
| Take weapon | Selected card is diamond in room | Discard existing equipped weapon if present; remove selected card from room to discard; set equipped weapon; clear last weapon kill | No action unless selected card is a weapon. |
| Drink potion | Selected card is heart; no potion has healed in current room | Heal `min(20, Health + value)`; mark potion used; discard/remove selected card | A second potion should still be resolvable but must not heal; feedback must clearly state why. |
| Fight barehanded | Selected card is black monster | Take full monster value; discard/remove monster; evaluate defeat | No action if not a monster or game finished. |
| Fight with weapon | Selected card is black; equipped weapon exists; monster value is strictly less than last weapon kill if a kill exists | Take reduced damage; discard/remove monster; set last weapon kill to monster value; evaluate defeat | Do not remove card or damage player; explain whether no weapon or degradation rule blocks it. |

A successful monster resolution using the weapon is its “kill” for the degradation rule even if reduced damage is zero. A barehanded kill must not change weapon history.

### 2.4 Unresolved requirements: do not silently choose behavior

The supplied rules end at line 95. The following must be resolved by product/design before final implementation or be encoded as explicit, tested decisions:

1. **Deck arithmetic/Jokers:** A standard 52-card deck has no Jokers. Removing six red face cards and two red aces removes eight and leaves 44, matching the stated 44 cards. If starting with a 54-card deck and removing two Jokers too, ten cards are removed and also leaves 44. Decide whether implementations construct 52 cards with no Jokers (recommended, simplest) or construct and remove two Joker representations.
2. **Final incomplete room:** With 44 cards and “resolve three, retain one,” after fourteen full room resolutions there is one dungeon card plus one retained card. The document specifies only rooms of four and does not specify whether a final two-card room is legal, how many it must resolve, or how victory is reached.
3. **Avoidance order:** “Place the four cards ... at the bottom” does not define whether their relative top-to-bottom order is preserved or reversed. Define it and test it.
4. **Potion wording:** “Additional potions ... must be left behind or resolved without healing” does not specify a separate action to leave a potion; because exactly one room card remains after three resolutions, likely it means leave it as the carried fourth card. Confirm this interpretation.
5. **Status after a lethal action:** Defeat is immediate by rule, but UI behavior (whether the selected card is shown discarded before the loss overlay) is presentational and should be consistent and tested.
6. **Weapon selection UX:** Rules permit choosing barehanded or weapon. The UI needs an explicit choice when an eligible weapon is equipped; a click alone must not silently assume one mode.

---

## 3. Proposed source structure

Create these new files under `src/Games/Card/Scoundrel/`. Names and responsibilities deliberately mirror the current Solitaire organization without introducing a generic game framework.

```text
src/Games/Card/Scoundrel/
  ScoundrelGame.cs
  ScoundrelCard.cs
  ScoundrelSuit.cs
  ScoundrelCardType.cs
  ScoundrelGameStatus.cs
  ScoundrelDeckBuilder.cs
  ScoundrelRules.cs
  ScoundrelLayout.cs
  ScoundrelArtwork.cs
  ScoundrelTextConstants.cs
  ScoundrelColors.cs
  ScoundrelInteractionController.cs
  ScoundrelRenderer.cs
```

| File / namespace | Existing or new | Responsibility |
|---|---|---|
| `ScoundrelGame.cs`, `Cheron.Games.Card.Scoundrel` | New | `IGameModule` owner, lifecycle adapter, mutable game state, game-action orchestration, test seams. |
| `ScoundrelCard.cs` | New | Internal standard-card identity: rank, suit, `FaceUp`; derived type/value via rules. |
| `ScoundrelSuit.cs` | New | Clubs, Diamonds, Hearts, Spades enum. Avoid sharing Solitaire’s internal enum. |
| `ScoundrelCardType.cs` | New | Monster, Weapon, Potion enum for readable rules/rendering. |
| `ScoundrelGameStatus.cs` | New | NotStarted, Playing, Won, Lost. Optional `ScoundrelRoomPhase` only if phase state cannot be expressed by existing fields. |
| `ScoundrelDeckBuilder.cs` | New | Build the 44-card filtered ordered deck and Fisher-Yates shuffle using an injected `Random`. |
| `ScoundrelRules.cs` | New | Pure classification, numeric values, damage, potion cap, and weapon-eligibility operations. No `GraphicsDevice`, `MouseState`, or module mutation. |
| `ScoundrelLayout.cs` | New | Logical board/card dimensions, named rectangles for dungeon, four room slots, weapon, health, buttons, and overlays. |
| `ScoundrelArtwork.cs` | New | Own generated MonoGame textures and dispose them. Mirror `SolitaireArtwork` ownership pattern. |
| `ScoundrelTextConstants.cs` and `ScoundrelColors.cs` | New | Central UI strings and colors; follow existing `SolitaireTextConstants`/`SolitaireColors` convention. |
| `ScoundrelInteractionController.cs` | New | Partial `ScoundrelGame` file containing a nested controller that maps client to logical input, hit-tests controls/cards, and invokes action methods. |
| `ScoundrelRenderer.cs` | New | Partial `ScoundrelGame` file containing nested renderer; draws board, cards, status, feedback, and terminal overlay. |

Keep the game module as `internal sealed partial class ScoundrelGame : IGameModule`, matching `SolitaireGame`. Make the rule/deck/card types `internal` unless an external project truly needs them; `Cheron.Tests` already receives internals through line 13 of `src/Cheron.csproj`.

### 3.1 Proposed `ScoundrelGame` state

```csharp
private const int MaximumHealth = 20;
private readonly List<ScoundrelCard> _dungeon = new(); // next draw is end
private readonly List<ScoundrelCard> _room = new();
private readonly List<ScoundrelCard> _discard = new();
private readonly Random _random = new();

private ScoundrelCard? _equippedWeapon;
private int? _weaponLastKillValue;
private int _health;
private int _resolvedInRoom;
private bool _potionUsedInRoom;
private bool _avoidedPreviousRoom;
private ScoundrelGameStatus _status;

private GameViewport _viewport;
private SpriteBatch _spriteBatch = null!;
private Texture2D _pixelTexture = null!;
private Texture2D _glyphTexture = null!;
private ScoundrelArtwork _artwork = null!;
private RasterizerState _boardRasterizerState = null!;
private readonly ScoundrelRenderer _renderer;
private readonly ScoundrelInteractionController _interactionController;
```

The game needs UI-only state as well: previous keyboard/mouse state for edge detection, focused/selected room index, a pending combat-choice monster index, status/invalid-action feedback text and duration, and optional new-game/help flags if those controls are supplied. Keep those separate from rules state.

### 3.2 Data ownership and boundaries

- **Rules:** `ScoundrelRules` accepts values/cards and returns calculations/validity. It never changes collections.
- **State:** `ScoundrelGame` owns all lists and applies only successful rule results. `RoomCards`, deck, discard, weapon, room flags, and status are not duplicated in the renderer.
- **Input:** controller calls only semantic methods, for example `TryAvoidCurrentRoom()`, `TryResolvePotion(int)`, `BeginMonsterCombat(int)`, `TryFightMonsterBarehanded(int)`, and `TryFightMonsterWithWeapon(int)`.
- **Rendering:** renderer reads state, uses `ScoundrelLayout` rectangles, and never resolves a card.
- **Engine:** only `ScoundrelGame` implements `IGameModule`; no engine-specific types appear in `ScoundrelRules` or `ScoundrelDeckBuilder`.

---

## 4. Implementation blueprint and pseudocode

### 4.1 Card and deck types

```csharp
// ScoundrelCard.cs
internal sealed class ScoundrelCard(int rank, ScoundrelSuit suit)
{
	public int Rank { get; } = rank; // 1=Ace, 11=Jack, 12=Queen, 13=King
	public ScoundrelSuit Suit { get; } = suit;
	public bool FaceUp { get; set; }
}

// ScoundrelRules.cs
internal static class ScoundrelRules
{
	public static ScoundrelCardType GetCardType(ScoundrelCard card);
	public static int GetValue(ScoundrelCard card); // black Ace rank 1 => 14
	public static int GetMonsterDamage(int monsterValue, ScoundrelCard? weapon);
	public static bool CanWeaponFight(int monsterValue, int? lastWeaponKillValue);
	public static int Heal(int health, int potionValue, int maximumHealth);
}
```

Pseudocode:

```text
GetCardType(card):
  Clubs or Spades -> Monster
  Diamonds -> Weapon
  Hearts -> Potion

GetValue(card):
  if card is Monster and Rank == 1: return 14
  otherwise return Rank

CreateShuffledDeck(random):
  deck = empty capacity 44
  for suit in Clubs, Diamonds, Hearts, Spades:
	for rank in 1..13:
	  if suit is Hearts/Diamonds and (rank == 1 or rank >= 11): skip
	  add new ScoundrelCard(rank, suit)
  assert deck.Count == 44
  Fisher-Yates shuffle deck with supplied random
  return deck // RemoveAt(Count - 1) is next draw
```

The exact filtering above implements the 52-card/no-Joker interpretation. If the Joker ambiguity is resolved differently, change only deck construction and its tests.

### 4.2 Initialization and room creation

```csharp
public void Initialize() => StartNewGame();

private void StartNewGame()
{
	// clear dungeon/room/discard and transient selection/feedback
	_health = MaximumHealth;
	_equippedWeapon = null;
	_weaponLastKillValue = null;
	_resolvedInRoom = 0;
	_potionUsedInRoom = false;
	_avoidedPreviousRoom = false;
	_status = ScoundrelGameStatus.Playing;
	_dungeon.AddRange(ScoundrelDeckBuilder.CreateShuffledDeck(_random));
	DrawRoomToCapacity();
}
```

```text
DrawRoomToCapacity():
  while Room.Count < 4 and Dungeon.Count > 0:
	card = Dungeon.RemoveAt(Dungeon.Count - 1)
	card.FaceUp = true
	Room.Add(card)
  if Room is empty and Dungeon is empty:
	Status = Won
  else if Room.Count < 4:
	apply the explicitly approved final-room rule; do not invent this behavior
```

Initial state is health 20, no weapon, no last weapon kill, no potion use, no avoided-room lock, and four face-up room cards unless the deck configuration is invalid.

### 4.3 Resolving a non-monster card

```text
TryTakeWeapon(roomIndex):
  Reject unless Playing, index is room card, and card type is Weapon
  if equipped weapon exists: move it to Discard
  remove selected weapon from Room
  add selected weapon to Discard // it remains the equipped card by reference, not a room card
  EquippedWeapon = selected weapon
  WeaponLastKillValue = null
  CompleteResolution()

TryDrinkPotion(roomIndex):
  Reject unless Playing and selected room card is Potion
  remove potion from Room and add it to Discard
  if PotionUsedInRoom:
	set feedback = "POTION ALREADY USED"
  else:
	Health = Heal(Health, potion value, MaximumHealth)
	PotionUsedInRoom = true
  CompleteResolution()
```

An equipped weapon cannot simultaneously be only in discard. Choose one representation consistently. Recommended: retain `_equippedWeapon` as the authoritative live reference and do **not** add it to `_discard` until it is replaced or the game ends; only room-resolved cards go to discard. The pseudocode above must be adjusted accordingly:

```text
on take weapon: discard old equipped weapon if present; remove new weapon from Room; set EquippedWeapon=new weapon; do not add new weapon to Discard
```

This is a proposed data invariant, not a stated rule.

### 4.4 Monster combat

Use a pending combat mode so the player deliberately chooses barehanded versus weapon:

```csharp
private void BeginMonsterCombat(int roomIndex);
private bool TryFightMonsterBarehanded(int roomIndex);
private bool TryFightMonsterWithWeapon(int roomIndex);
private void CompleteResolution();
private void CheckTerminalState();
```

```text
TryFightMonsterBarehanded(index):
  monster = validate selected Room[index] is Monster and status Playing
  Health -= GetValue(monster)
  remove monster -> Discard
  CompleteResolution()
  CheckTerminalState() // status Lost immediately if health <= 0

TryFightMonsterWithWeapon(index):
  monster = validate selected Room[index] is Monster
  reject when no EquippedWeapon
  monsterValue = GetValue(monster)
  reject when WeaponLastKillValue exists and monsterValue >= WeaponLastKillValue
  Health -= max(0, monsterValue - GetValue(EquippedWeapon))
  WeaponLastKillValue = monsterValue
  remove monster -> Discard
  CompleteResolution()
  CheckTerminalState()

CompleteResolution():
  ResolvedInRoom++
  AvoidedPreviousRoom = false
  clear combat selection
  if Status != Playing: return
  if ResolvedInRoom != 3: return
  // exactly one card remains during a normal room
  ResolvedInRoom = 0
  PotionUsedInRoom = false
  DrawRoomToCapacity()
  CheckTerminalState()
```

The order of terminal evaluation should preserve the stated immediate-loss rule: a lethal combat must set `Lost` before further player interaction or automatic new-room logic. Decide and document whether resolution/discard animation occurs before the loss overlay.

### 4.5 Avoiding a room

```csharp
private bool TryAvoidCurrentRoom()
{
	if (_status != ScoundrelGameStatus.Playing ||
		_resolvedInRoom != 0 || _avoidedPreviousRoom || _room.Count != 4)
	{
		ShowInvalidAction(...);
		return false;
	}

	// Use approved, explicitly tested relative-order rule.
	foreach (var card in _room)
	{
		card.FaceUp = false; // only if avoided cards should be hidden at dungeon bottom
		_dungeon.Insert(0, card);
	}

	_room.Clear();
	_avoidedPreviousRoom = true;
	_potionUsedInRoom = false;
	DrawRoomToCapacity();
	return true;
}
```

Do not ship the shown `Insert(0, ...)` order until the product decision identifies whether it preserves original room order. The deck convention is important: the end is drawn next, so index 0 is the bottom.

### 4.6 `IGameModule` implementation

Follow the exact resource model demonstrated by `SolitaireGame`:

```csharp
public string Title => "Scoundrel";
public Point PreferredWindowSize => new(1100, 800); // proposed; finalize in ScoundrelLayout

public void SetViewport(GameViewport viewport)
{
	_viewport = viewport;
	// cancel pending click/selection when needed; no game-rule state changes
}

public void LoadContent(GraphicsDevice graphicsDevice)
{
	_spriteBatch = new SpriteBatch(graphicsDevice);
	_pixelTexture = new Texture2D(graphicsDevice, 1, 1);
	_pixelTexture.SetData([Color.White]);
	_glyphTexture = SolitaireBitmapFont.CreateGlyphTexture(graphicsDevice); // only if dependency accepted
	_artwork = new ScoundrelArtwork(graphicsDevice);
	_boardRasterizerState = new RasterizerState { ScissorTestEnable = true };
}

public void Update(GameTime gameTime)
{
	// read edge-triggered keyboard input
	// read Mouse.GetState(), map through controller to logical coordinates
	// update only UI feedback/overlay timers; rules advance only on actions
}

public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice) => _renderer.Draw(gameTime, graphicsDevice);

public void Dispose()
{
	_boardRasterizerState?.Dispose();
	_artwork?.Dispose();
	_glyphTexture?.Dispose();
	_pixelTexture?.Dispose();
	_spriteBatch?.Dispose();
}
```

### 4.7 UI, layout, and rendering

Use one fixed logical board like Solitaire (`1100 x 800` is a reasonable proposed starting canvas, not an existing Scoundrel constant). `ScoundrelLayout` should contain named geometry methods; do not hard-code hit-test rectangles in controller methods.

Recommended board regions:

- top bar: title plus `NEW`, `HELP`, and optional sound/settings controls patterned after Solitaire;
- left: face-down dungeon pile/count and discard pile/count;
- center: four named room slots in a single row, each a `Rectangle` for drawing and hit testing;
- right: equipped weapon slot, weapon power, and last-kill restriction (for example `WEAPON 8 / LAST KILL 12`);
- status panel: `HEALTH 17/20`, potion-used indicator, and consecutive-avoidance availability;
- action panel/modal: selected monster description plus `BAREHAND` and `WEAPON` controls; disabled/ineligible weapon is visibly explained;
- footer: concise keyboard/pointer instructions and transient invalid-action feedback;
- terminal overlay: `YOU ESCAPED` for Won or `YOU DIED` for Lost, plus New Game action.

`ScoundrelRenderer.Draw` should follow `SolitaireRenderer.Draw`: clear the device, save/restore scissor/rasterizer state, set `_viewport.ScissorBounds`, then begin `SpriteBatch` with `SamplerState.PointClamp`, the board rasterizer, and `_viewport.Transform`. `ScoundrelArtwork` should own all generated textures and dispose every one it creates, as `SolitaireArtwork` does.

For card drawing, reuse the **visual technique** in `SolitaireRenderer` (face texture, rank labels, suit mask, selection outline), but implement Scoundrel-specific `DrawCard(ScoundrelCard, Rectangle, bool selected)`. Type badges/colors should be supplementary; suit/rank must remain visible so rules remain understandable.

Input controller pseudocode:

```text
UpdateClientPointer(clientPosition, isDown, time):
  UpdatePointer(Game._viewport.ToLogical(clientPosition), isDown, time)

on new mouse release/click:
  if terminal overlay: route only New Game
  else if combat-choice panel: route barehand / weapon button
  else if avoid button: Game.TryAvoidCurrentRoom()
  else if card slot contains room card:
	if Monster: Game.BeginMonsterCombat(index)
	if Weapon: Game.TryTakeWeapon(index)
	if Potion: Game.TryDrinkPotion(index)

on keyboard:
  arrows or Tab move selected room/control focus
  Enter/Space activate focused card/control
  A invokes avoid only when valid
  B/W select barehand/weapon only during pending combat
  N begins a new game
```

Keyboard bindings are proposed; keep them documented in `ScoundrelTextConstants` and provide an equivalent pointer route. Do not let the renderer decide validity.

---

## 5. Engine integration changes

### Existing files to modify

1. **`src/Engine/StartScreenGame.cs`**
   - Add `using global::Cheron.Games.Card.Scoundrel;`.
   - Add a `new("Scoundrel", StartScreenGameKind.Scoundrel, new Point(...))` entry to `_cartridges`; pick a non-overlapping visual position after inspecting the current start-screen layout.
   - Add `Scoundrel` to private `StartScreenGameKind` (currently Solitaire, Tile, None as applicable).
   - Add `StartScreenGameKind.Scoundrel => new ScoundrelGame()` to `TransitionToInsertedGame()`.
   - No changes to `GameHost`, `IGameModule`, or `GameViewport` are expected.

### New files to create

Create the files in the structure table in section 3. The application project uses the SDK default compile globs, so C# files below `src/` are compiled automatically. Do not add a new project or project reference.

### Assets/resources

No content-pipeline change is required if Scoundrel follows `SolitaireArtwork` and creates textures programmatically on the graphics thread. If external images/audio are approved later, then update `src/Content/Content.mgcb` and add a documented content-loading path; this is not required for initial feature parity.

### Cartridge verification

Manually verify: the start screen shows a Scoundrel cartridge, permits it to be dragged/inserted according to existing picker behavior, powers on into `ScoundrelGame`, uses its title/window size, and disposing the application does not double-dispose resources.

---

## 6. Testing plan

### 6.1 Current test conventions

Tests are in `src/Cheron.Tests`, use xUnit 2 (`Cheron.Tests.csproj`), and reference the application project. `src/Cheron.csproj` already declares `<InternalsVisibleTo Include="Cheron.Tests" />`.

Existing tests establish two useful styles:

- [`SolitaireVariantHelpersTests.cs`](../../../../src/Cheron.Tests/SolitaireVariantHelpersTests.cs) directly tests pure deal/move helper methods using deterministic manually constructed lists.
- [`TileGameTests.cs`](../../../../src/Cheron.Tests/TileGameTests.cs) uses internal `Test*` members on `TileGame` to invoke otherwise private behavior and inspect state. This is the intended project convention; do not use reflection or make production APIs public only for tests.

The instruction mentioning `InternalsVisibleTo("Solitaire.Tests")` is inconsistent with this workspace: the actual friend assembly is `Cheron.Tests`. Follow the real project configuration unless it changes.

### 6.2 Proposed test files

```text
src/Cheron.Tests/
  ScoundrelDeckBuilderTests.cs
  ScoundrelRulesTests.cs
  ScoundrelGameTests.cs
  ScoundrelInteractionTests.cs        // only after controller test seams exist
  StartScreenGameScoundrelTests.cs    // only if start-screen seams are practical
```

### 6.3 Pure rule tests

`ScoundrelDeckBuilderTests`:

- deck contains exactly 44 non-Joker cards under approved deck decision;
- no Heart/Diamond Ace, Jack, Queen, or King exists;
- every Clubs/Spades rank 1-13 exists once;
- every Hearts/Diamonds rank 2-10 exists once;
- fixed `new Random(seed)` preserves card multiset and produces reproducible order (assert expected order only when the implementation deliberately treats it as stable);
- deck draws from `Count - 1`.

`ScoundrelRulesTests`:

- suit classification produces Monster/Weapon/Potion;
- black Ace evaluates to 14; face cards to 11/12/13; red numbered cards retain rank;
- barehanded monster damage equals monster value;
- weapon damage is clamped at zero;
- no weapon/last kill allows a weapon; last kill 12 allows only values below 12 and rejects 12/13/14;
- heal clamps at 20 (`18 + 10 == 20`).

Example pseudocode:

```csharp
[Fact]
public void CanWeaponFight_RejectsMonsterEqualToLastKill()
{
	Assert.False(ScoundrelRules.CanWeaponFight(monsterValue: 12, lastWeaponKillValue: 12));
	Assert.True(ScoundrelRules.CanWeaponFight(monsterValue: 11, lastWeaponKillValue: 12));
}
```

### 6.4 Game-state/action tests

Give `ScoundrelGame` narrow `internal Test*` members, matching the existing convention. Suggested seams:

```csharp
internal void TestStartNewGameWithDeck(IEnumerable<ScoundrelCard> drawOrder);
internal int TestGetHealth();
internal ScoundrelGameStatus TestGetStatus();
internal IReadOnlyList<ScoundrelCard> TestGetRoomCards();
internal int TestGetDungeonCount();
internal int TestGetDiscardCount();
internal ScoundrelCard? TestGetEquippedWeapon();
internal int? TestGetWeaponLastKillValue();
internal bool TestHasUsedPotionInRoom();
internal bool TestCanAvoidRoom();
internal bool TestTryAvoidCurrentRoom();
internal bool TestTryTakeWeapon(int roomIndex);
internal bool TestTryDrinkPotion(int roomIndex);
internal bool TestTryFightBarehanded(int roomIndex);
internal bool TestTryFightWithWeapon(int roomIndex);
```

Use controlled decks in **draw order compatible with end-of-list draws**. A helper should make this clear, for example reverse a human-readable next-card sequence before loading `_dungeon`.

`ScoundrelGameTests` coverage:

- initial state: health 20, four face-up room cards, no weapon, no potion use, status Playing;
- resolve any three cards in different orders and assert the fourth remains/next room is filled;
- avoid succeeds only before first resolution, returns all four to bottom with approved ordering, and immediately blocks another avoidance;
- resolving one card after an avoided room re-enables avoidance according to the final interpretation;
- equipping a weapon discards prior weapon and resets degradation state;
- first weapon kill records monster value; allowed lower monster works; equal/higher is rejected without damage/discard/mutation; barehanding an otherwise blocked monster works;
- first potion heals and marks flag; second potion is discarded but does not heal; new room resets the flag;
- healing never exceeds 20;
- lethal barehanded and lethal armed combat set Lost immediately; post-terminal actions leave all state unchanged;
- clearing every card sets Won; defeat takes precedence if lethal action occurs;
- approved final-room behavior, empty deck, and final carried card are explicit tests once the ambiguity is resolved.

`ScoundrelInteractionTests` should validate logical hit testing and semantic routing, not MonoGame rendering pixels: click each room rectangle, combat buttons, avoid control, and terminal New Game control. Use `ScoundrelLayout` bounds and `TestUpdatePointer(...)` seams, just as Solitaire exposes `TestUpdatePointer`.

### 6.5 Integration/manual checks

Automated `IGameModule` update tests should avoid requiring a live `GraphicsDevice`. Cover engine-independent gameplay with rules/game tests. Manually smoke test the `StartScreenGame` cartridge lifecycle and `LoadContent`/`Draw` path because the current test project has no graphics device fixture.

Run the relevant `Cheron.Tests` tests and then build the entire solution. New test files should use namespaces `Cheron.Tests` and `Xunit` attributes, matching existing files.

---

## 7. Ordered implementation roadmap

1. **Confirm rule decisions.** Resolve the six ambiguities in section 2.4, especially final-room resolution and deck/Joker construction. Record the decisions in user-facing Scoundrel help text and tests.
2. **Create domain types.** Add suit, card type, game status, card, and deck builder. Verify 44-card composition with unit tests.
3. **Add pure mechanics.** Implement and unit-test `ScoundrelRules` for values, damage, weapon degradation, and capped healing. No MonoGame dependencies.
4. **Add layout/constants/artwork.** Define every card/control rectangle and generated artwork ownership. Verify all interaction targets use layout factories rather than magic rectangles.
5. **Create the game state module.** Implement `ScoundrelGame : IGameModule`, `StartNewGame`, controlled room draw/fill, semantic action methods, terminal-state handling, and internal `Test*` seams. Verify deterministic action sequences with `ScoundrelGameTests`.
6. **Add rendering.** Create the renderer, draw full state and terminal overlays using viewport transform/scissor conventions. Verify visually at normal and high-DPI scaling.
7. **Add interaction.** Implement controller pointer mapping, keyboard focus/activation, pending combat choice, invalid feedback, and prevention of terminal actions. Verify interaction tests and manual input.
8. **Register the cartridge.** Modify only `src/Engine/StartScreenGame.cs` to list and instantiate Scoundrel. Verify full startup -> cartridge -> game -> application disposal lifecycle.
9. **Run validation.** Execute all relevant xUnit tests, build `src/Cheron.slnx`, then manually test new game, each card type, weapon replacement/degradation, potion limit, avoid rule, victory, defeat, and final-room behavior.

---

## Final implementation checklist

- [ ] No dependency on nonexistent `Cheron.Engine`/`Cheron.Games.Card` projects or invented cartridge API.
- [ ] Every new production type is in `Cheron.Games.Card.Scoundrel`.
- [ ] All actions are impossible after `Won` or `Lost`.
- [ ] Deck/filter and final-room policy are explicitly decided and covered by tests.
- [ ] Rule calculations have no graphics/input dependency.
- [ ] Graphics resources are created after device availability and disposed by the module only.
- [ ] Pointer coordinates are transformed through `GameViewport.ToLogical` before hit testing.
- [ ] Integration modifies only the start-screen registration mechanism unless a proven new requirement demands more.
- [ ] Tests use `Cheron.Tests`, xUnit, deterministic decks/seeds, and internal `Test*` seams rather than reflection.
