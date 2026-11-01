using System;
using System.Collections.Generic;

namespace Cheron.Games.Solitaire
{
    public class SolitaireVariant
    {
        public static void RegisterSpiderVariant()
        {
            var spiderBehavior = new SpiderVariantBehavior();
            SolitaireVariantRegistry.RegisterVariant(spiderBehavior);
        }
    }
}