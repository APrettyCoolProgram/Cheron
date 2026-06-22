<div align="center">

  <h1>Cheron: Development - Roadmap</h1>

</div>

## Tekst Engine

- [ ] If the banner == null or empty, the game title from <c>GameDetailData</c> will display the GameDetailData.GameTitle instead.
- [ ] If Story is null or empty, GameDetailData.GameDescription is used.
- [ ] Different fonts?
- [ ] pre-built themes?
- [ ] CLS between sessions
- [ ] CLS between rooms
- [X] --help command for listing available commands and options
- [ ] Add support for rich text formatting
- [ ] Optimize performance for large text blocks
- [ ] List carts
- [ ] Compress carts for distribution
- [ ] Unformatted JSON-carts for size (Studio shows formatted, or can export as formatted)
- [ ] End game message in cart
- [ ] Tekst GameType Variations
    * TekstGameTypeVariationA
      * 80-column width
      * No header
      * No color support
    * TekstGameTypeVariationB
      * 80-column width
      * Header
      * Single color scheme support
    * TekstGameTypeVariationC
      * No column width limit
      * Header
      * Color scheme support for:
          * Header
          * Text

- TekstGameTypeVariationC: Advanced text-based game with full features
- TekstGameTypeVariationD: Experimental text-based game with cutting-edge features
- TekstGameTypeVariationE: Future text-based game with experimental features
- 

## Engines

* Tekst Engine: Interactive text-based game engine for creating and managing text-based games within the Cheron ecosystem.
* Dungine: Interactive dungeon-crawling game engine for creating and managing dungeon-based games within the Cheron ecosystem. Roguelikes.
* Kort Engine: Interactive card game engine for creating and managing card-based games within the Cheron ecosystem.