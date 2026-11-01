using System;

namespace Cheron.Games.Card.Scoundrel;

/// <summary>Contains Scoundrel gameplay rule helpers.</summary>
public static class ScoundrelRules
{
    /// <summary>Gets the Scoundrel card type from suit.</summary>
    public static ScoundrelCardType GetCardType(ScoundrelCard card)
    {
        return card.Suit switch
        {
            ScoundrelSuit.Clubs or ScoundrelSuit.Spades => ScoundrelCardType.Monster,
            ScoundrelSuit.Diamonds => ScoundrelCardType.Weapon,
            _ => ScoundrelCardType.Potion
        };
    }

    /// <summary>Gets the gameplay value of a card.</summary>
    public static int GetValue(ScoundrelCard card)
    {
        if (GetCardType(card) == ScoundrelCardType.Monster && card.Rank == 1)
        {
            return 14;
        }

        return card.Rank;
    }

    /// <summary>Returns whether current weapon history allows fighting this monster.</summary>
    public static bool CanWeaponFight(int monsterValue, int? lastWeaponKillValue)
    {
        return !lastWeaponKillValue.HasValue || monsterValue < lastWeaponKillValue.Value;
    }

    /// <summary>Calculates damage taken from a monster.</summary>
    public static int GetMonsterDamage(int monsterValue, ScoundrelCard? weapon)
    {
        if (weapon is null)
        {
            return monsterValue;
        }

        var weaponValue = GetValue(weapon);
        return Math.Max(0, monsterValue - weaponValue);
    }

    /// <summary>Applies healing with a hard max-health cap.</summary>
    public static int Heal(int currentHealth, int potionValue, int maxHealth)
    {
        return Math.Min(maxHealth, currentHealth + potionValue);
    }
}
