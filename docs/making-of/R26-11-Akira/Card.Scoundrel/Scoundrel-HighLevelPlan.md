Creating a new card game called **"Scoundrel"** for the **Cheron** game engine is a multi-step process that involves understanding the existing architecture, designing the new game logic, and integrating it into the engine. Below is a **high-level step-by-step plan** to implement the **Scoundrel** game within the **Cheron** codebase.

---

## 🧭 High-Level Step-by-Step Plan to Implement "Scoundrel" in Cheron

### 1. **Understand the Existing Cheron Game Engine Structure**
   - Review the existing code in:
     ```
     D:/Repositories/GitHub/APCP/Public/Cheron/src/
     ```
   - Identify key components such as:
     - Game engine core
     - Card game base classes (e.g., `Cheron.Games.Card`)
     - Game state management
     - Player and card logic
     - Event systems and rules enforcement

---

### 2. **Review the Scoundrel Game Rules**
   - Study the rules document:
     ```
     D:/Repositories/GitHub/APCP/Public/Cheron/docs/making-of/R26-11-Akira/Card.Scoundrel/Scoundrel-Instructions.md
     ```
   - Identify:
     - Game objective
     - Card types and their effects
     - Game phases (setup, turn, endgame)
     - Win conditions
     - Special rules or mechanics

---

### 3. **Design the Scoundrel Game Architecture**
   - Create a new namespace:
     ```
     Cheron.Games.Card.Scoundrel
     ```
   - Plan the following components:
     - **Game class** (`ScoundrelGame`) – main game logic
     - **Card classes** – specific to Scoundrel (e.g., `ScoundrelCard`, `ScoundrelActionCard`, etc.)
     - **Player class** – if needed, or extend existing player logic
     - **Rules engine** – to enforce game rules
     - **Game state** – to track ongoing game state (e.g., current turn, players, cards in play)
     - **Event system** – to handle game events (e.g., onTurnStart, onCardPlayed)

---

### 4. **Set Up the Project Structure**
   - Create the necessary folders and files in:
     ```
     src/Games/Card/Scoundrel/
     ```
     Example:
     ```
     src/Games/Card/Scoundrel/
       ├── ScoundrelGame.cs
       ├── ScoundrelCard.cs
       ├── ScoundrelActionCard.cs
       ├── ScoundrelPlayer.cs
       ├── ScoundrelRules.cs
       ├── ScoundrelGameState.cs
       └── ScoundrelEvent.cs
     ```

---

### 5. **Implement Core Game Components**
   - **Game Initialization**:
     - Implement `ScoundrelGame` with a constructor that initializes the game state, players, and deck.
   - **Card System**:
     - Define `ScoundrelCard` as a base class or extend an existing `Card` class.
     - Implement specific card types (e.g., `ScoundrelActionCard`, `ScoundrelTreasureCard`, etc.).
   - **Game State**:
     - Implement `ScoundrelGameState` to track:
       - Players' hands
       - Cards in play
       - Turn order
       - Game progress
   - **Rules System**:
     - Implement `ScoundrelRules` to enforce:
       - Valid moves
       - Turn order
       - Card effects
       - Win conditions
   - **Event Handling**:
     - Implement `ScoundrelEvent` or extend existing event classes to handle:
       - Card played
       - Turn start/end
       - Game over

---

### 6. **Integrate with the Cheron Engine**
   - Modify the game engine to support the new game:
     - Register `ScoundrelGame` as a supported game type
     - Update the UI or menu system to allow selecting "Scoundrel"
     - Ensure compatibility with existing systems (e.g., saving/loading, multiplayer, AI)

---

### 7. **Implement Basic Game Flow**
   - Implement the game loop:
     - Setup phase
     - Player turn phase
     - End of turn
     - Endgame check
   - Implement basic card interactions:
     - Playing a card
     - Drawing cards
     - Discarding cards

---

### 8. **Test the Game**
   - Create unit tests for:
     - Card effects
     - Game rules
     - State transitions
   - Playtest the game manually or with automated scripts
   - Fix any bugs or inconsistencies

---

### 9. **Add Game UI (Optional)**
   - If the engine supports UI customization:
     - Create a UI for Scoundrel (e.g., hand display, cards in play, status messages)
     - Implement visual feedback for actions and events

---

### 10. **Document and Refactor**
   - Write documentation for:
     - Game rules
     - Code structure
     - API usage
   - Refactor code as needed for clarity and maintainability
   - Ensure code follows the same style and conventions as the rest of the engine

---

## ✅ Summary

| Step | Description |
|------|-------------|
| 1 | Understand Cheron codebase |
| 2 | Review Scoundrel rules |
| 3 | Design game architecture |
| 4 | Set up project structure |
| 5 | Implement core components |
| 6 | Integrate with engine |
| 7 | Implement game flow |
| 8 | Test game |
| 9 | Add UI (optional) |
| 10 | Document and refactor |

---

Let me know if you'd like a more detailed breakdown of any of these steps, or if you want help with specific files or components.