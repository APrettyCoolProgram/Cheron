Now that we have a clear understanding of the existing structure and files in the `Cheron` game engine, here's a **detailed implementation plan** to create the Yukon variant of Solitaire. This plan is written in a way that other models (like Copilot, Claude, etc.) can follow and implement the game.

---

## 🎯 **Objective**
Implement the Yukon variant of Solitaire using the Cheron game engine, with procedural rendering and logic, without the need for custom assets.

---

## 📁 **Folder Structure**
We will create a new folder for Yukon Solitaire inside the `Games/Solitaire` directory, structured similarly to the `Klondike` and `FreeCell` folders.

### **Directory Structure Plan**
```
Games/
└── Solitaire/
    └── Yukon/
        ├── YukonBoardRenderer.cs
        ├── YukonDealBuilder.cs
        ├── YukonGameplay.cs
        ├── YukonLayout.cs
        ├── YukonMoveRules.cs
        └── YukonVariantBehavior.cs
```

---

## 🧩 **File Implementation Plan**

### **1. YukonVariantBehavior.cs**
- **Purpose**: Define Yukon as a variant of Solitaire.
- **Implementation Steps**:
  - Inherit from `ISolitaireVariantBehavior`.
  - Implement methods like `InitializeGame()`, `GetGameType()`, and `GetGameRules()`.
  - Set the game type to "Yukon".
  - Reference the `YukonGameplay` class for game-specific logic.

### **2. YukonGameplay.cs**
- **Purpose**: Handle the main gameplay logic.
- **Implementation Steps**:
  - Inherit from `SolitaireGame`.
  - Implement the Yukon-specific game rules:
    - 7 tableau stacks with the specified layout (1, 6, 7, 8, 9, 10, 11 cards).
    - Foundations build up by suit.
    - Groups of cards can be moved with alternate color, one rank higher.
    - No stock pile.
    - Any card can be placed in empty tableau spaces.
  - Implement methods for:
    - Card movement.
    - Game state checks (win/loss conditions).
    - Undo functionality (if required).

### **3. YukonLayout.cs**
- **Purpose**: Define the layout of the game interface.
- **Implementation Steps**:
  - Inherit from `SolitaireLayout`.
  - Define the positions and sizes of:
    - Tableau stacks.
    - Foundations.
    - Any UI elements (e.g., menus, buttons).
  - Use procedural rendering for the layout.

### **4. YukonMoveRules.cs**
- **Purpose**: Implement the rules for moving cards.
- **Implementation Steps**:
  - Inherit from `SolitaireMoveRules`.
  - Define logic for:
    - Moving cards from tableau to foundations (based on suit and rank).
    - Moving cards between tableau stacks (based on color and rank).
    - Handling group moves.
    - Empty tableau spaces.

### **5. YukonDealBuilder.cs**
- **Purpose**: Initialize and deal the cards for the game.
- **Implementation Steps**:
  - Inherit from `SolitaireDealBuilder`.
  - Deal the cards in the specified Yukon layout:
    - 7 tableau stacks with the top 5 cards face-up, and the rest face-down.
    - No stock pile.

### **6. YukonBoardRenderer.cs**
- **Purpose**: Render the game board.
- **Implementation Steps**:
  - Inherit from `SolitaireRenderer` or create a custom renderer if needed.
  - Implement logic to draw:
    - Tableau stacks with face-up/face-down cards.
    - Foundations.
    - Card positions and states.
  - Use procedural rendering for cards and UI.

---

## 🧪 **Testing and Debugging**
- Create a test method in `YukonGameplay.cs` to verify:
  - Initial layout correctness.
  - Movement rules.
  - Game win/loss logic.
- Use the `SolitaireInteractionController` to test user input handling.
- Run the game in the engine and verify that all rules are implemented correctly.

---

## 🧱 **Integrate with the Game Engine**
- Update `SolitaireVariantExtensions.cs` to include `YukonVariantBehavior` in the list of supported variants.
- Update the main game launcher (`StartScreenGame.cs`) to allow launching Yukon Solitaire.
- Ensure that the game is accessible through the main menu or game selection screen.

---

## 📝 **Documentation**
- Create a README file in the `Yukon` folder to explain:
  - The purpose of the game.
  - How it differs from other variants.
  - How to build and run the game.
- Add comments to the source files to explain the logic and implementation.

---

## ✅ **Final Steps**
1. Create the `Yukon` folder inside `Games/Solitaire`.
2. Create the files listed in the plan and implement the logic.
3. Test the game to ensure all rules are working as intended.
4. Add the game to the main menu and ensure it is fully functional.

---

Would you like help creating the first file (`YukonVariantBehavior.cs`) or any other file? 🃏