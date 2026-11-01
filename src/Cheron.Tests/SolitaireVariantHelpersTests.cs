// 261101_code
// 261101_documentation

using Cheron.Games.Solitaire;
using Cheron.Games.Solitaire.FreeCell;
using Cheron.Games.Solitaire.Klondike;
using Cheron.Games.Solitaire.Spider;
using Cheron.Games.Solitaire.Yukon;
using Xunit;

namespace Cheron.Tests;

public sealed class SolitaireVariantHelpersTests
{
    [Fact]
    public void FreeCellInitializeDeal_PopulatesCascadesCellsAndFoundations()
    {
        var deck = CreateOrderedDeck();
        var tableau = new List<List<SolitaireCard>>();
        var foundations = new List<List<SolitaireCard>>();
        var freeCells = new List<List<SolitaireCard>>();

        FreeCellGameplay.InitializeDeal(deck, tableau, foundations, freeCells);

        Assert.Equal(8, tableau.Count);
        Assert.Equal(new[] { 7, 7, 7, 7, 6, 6, 6, 6 }, GetPileCounts(tableau));
        Assert.Equal(4, freeCells.Count);
        Assert.Equal(new[] { 0, 0, 0, 0 }, GetPileCounts(freeCells));
        Assert.Equal(4, foundations.Count);
        Assert.Equal(new[] { 0, 0, 0, 0 }, GetPileCounts(foundations));
        Assert.Empty(deck);
    }

    [Fact]
    public void KlondikeInitializeDeal_PopulatesTableauStockAndFoundations()
    {
        var deck = CreateOrderedDeck();
        var tableau = new List<List<SolitaireCard>>();
        var stock = new List<SolitaireCard>();
        var foundations = new List<List<SolitaireCard>>();

        KlondikeGameplay.InitializeDeal(deck, tableau, stock, foundations);

        Assert.Equal(7, tableau.Count);
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7 }, GetPileCounts(tableau));
        Assert.Equal(24, stock.Count);
        Assert.Equal(4, foundations.Count);
        Assert.Equal(new[] { 0, 0, 0, 0 }, GetPileCounts(foundations));
        Assert.Equal(24, deck.Count);
        Assert.Equal(stock.Count, deck.Count);
    }

    [Fact]
    public void SpiderInitializeDeal_PopulatesTableauStockAndFoundations()
    {
        var deck = CreateOrderedDeck();
        var tableau = new List<List<SolitaireCard>>();
        var stock = new List<SolitaireCard>();
        var foundations = new List<List<SolitaireCard>>();

        SpiderGameplay.InitializeDeal(deck, tableau, stock, foundations);

        Assert.Equal(10, tableau.Count);
        Assert.Equal(new[] { 6, 6, 6, 6, 5, 5, 5, 5, 5, 5 }, GetPileCounts(tableau));
        Assert.Equal(50, stock.Count);
        Assert.Equal(4, foundations.Count);
        Assert.Equal(new[] { 0, 0, 0, 0 }, GetPileCounts(foundations));
        Assert.Equal(50, deck.Count);
    }

    [Fact]
    public void SpiderMoveRules_RequiresSameSuitDescendingRun()
    {
        var destination = new List<SolitaireCard>
        {
            new(10, SolitaireSuit.Spades) { FaceUp = true }
        };

        var validRun = new List<SolitaireCard>
        {
            new(9, SolitaireSuit.Spades) { FaceUp = true },
            new(8, SolitaireSuit.Spades) { FaceUp = true }
        };

        var invalidRun = new List<SolitaireCard>
        {
            new(9, SolitaireSuit.Spades) { FaceUp = true },
            new(8, SolitaireSuit.Hearts) { FaceUp = true }
        };

        Assert.True(SpiderMoveRules.CanMoveToTableau(validRun, 0, destination));
        Assert.False(SpiderMoveRules.CanMoveToTableau(invalidRun, 0, destination));
    }

    [Fact]
    public void YukonInitializeDeal_PopulatesTableauAndFoundations()
    {
        var deck = CreateOrderedDeck();
        var tableau = new List<List<SolitaireCard>>();
        var foundations = new List<List<SolitaireCard>>();

        YukonGameplay.InitializeDeal(deck, tableau, foundations);

        Assert.Equal(7, tableau.Count);
        Assert.Equal(new[] { 1, 6, 7, 8, 9, 10, 11 }, GetPileCounts(tableau));
        Assert.Equal(4, foundations.Count);
        Assert.Equal(new[] { 0, 0, 0, 0 }, GetPileCounts(foundations));
        Assert.Empty(deck);
    }

    [Fact]
    public void YukonMoveRules_AllowsFaceUpSelectionOnDescendingAlternatingTarget()
    {
        var source = new List<SolitaireCard>
        {
            new(9, SolitaireSuit.Clubs) { FaceUp = true },
            new(8, SolitaireSuit.Hearts) { FaceUp = true }
        };
        var destination = new List<SolitaireCard>
        {
            new(10, SolitaireSuit.Diamonds) { FaceUp = true }
        };

        var canMove = YukonMoveRules.CanMoveToTableau(source, 0, destination);

        Assert.True(canMove);
    }

    [Fact]
    public void FreeCellTopRowClick_OnCell_SelectsThatCell()
    {
        var freeCells = new List<List<SolitaireCard>>
        {
            new(), new(), new(), new()
        };
        var selectedPile = (List<SolitaireCard>?)null;
        var selectedIndex = -1;

        var clickPoint = SolitaireLayout.GetFreeCellBounds(2).Center;
        var handled = FreeCellGameplay.TryHandleTopRowClick(
            clickPoint,
            freeCells,
            SolitaireLayout.GetFreeCellBounds,
            selectedPile,
            _ => false,
            _ => throw new InvalidOperationException("Should not show invalid move for empty selection."),
            pile => selectedIndex = freeCells.IndexOf(pile));

        Assert.True(handled);
        Assert.Equal(2, selectedIndex);
    }

    [Fact]
    public void KlondikeTopRowClick_OnStock_InvokesDraw()
    {
        var waste = new List<SolitaireCard>();
        var drawCalled = false;
        var selectedWaste = false;

        var handled = KlondikeGameplay.TryHandleTopRowClick(
            SolitaireLayout.GetStockBounds().Center,
            SolitaireLayout.GetStockBounds(),
            SolitaireLayout.GetWasteBounds(),
            waste,
            () => drawCalled = true,
            _ => selectedWaste = true);

        Assert.True(handled);
        Assert.True(drawCalled);
        Assert.False(selectedWaste);
    }

    private static List<SolitaireCard> CreateOrderedDeck()
    {
        var deck = new List<SolitaireCard>(52);
        foreach (var suit in Enum.GetValues<SolitaireSuit>())
        {
            for (var rank = 1; rank <= 13; rank++)
            {
                deck.Add(new SolitaireCard(rank, suit));
            }
        }

        return deck;
    }

    private static int[] GetPileCounts(List<List<SolitaireCard>> piles)
    {
        var counts = new int[piles.Count];
        for (var index = 0; index < piles.Count; index++)
        {
            counts[index] = piles[index].Count;
        }

        return counts;
    }
}
