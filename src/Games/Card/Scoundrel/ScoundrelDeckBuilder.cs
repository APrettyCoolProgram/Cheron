using System;
using System.Collections.Generic;

namespace Cheron.Games.Card.Scoundrel;

/// <summary>Builds shuffled Scoundrel dungeon decks.</summary>
public static class ScoundrelDeckBuilder
{
    /// <summary>Creates a shuffled 44-card Scoundrel deck.</summary>
    public static List<ScoundrelCard> CreateShuffledDeck(Random random)
    {
        var deck = new List<ScoundrelCard>(44);

        foreach (var suit in Enum.GetValues<ScoundrelSuit>())
        {
            for (var rank = 1; rank <= 13; rank++)
            {
                if (ShouldRemoveCard(suit, rank))
                {
                    continue;
                }

                deck.Add(new ScoundrelCard(rank, suit));
            }
        }

        Shuffle(deck, random);
        return deck;
    }

    private static bool ShouldRemoveCard(ScoundrelSuit suit, int rank)
    {
        if (suit is not (ScoundrelSuit.Hearts or ScoundrelSuit.Diamonds))
        {
            return false;
        }

        return rank == 1 || rank >= 11;
    }

    private static void Shuffle(List<ScoundrelCard> deck, Random random)
    {
        for (var index = deck.Count - 1; index > 0; index--)
        {
            var swapIndex = random.Next(index + 1);
            (deck[index], deck[swapIndex]) = (deck[swapIndex], deck[index]);
        }
    }
}
