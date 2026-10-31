// Card object [151123]

namespace TheCards
{
    class Card
    {
        // These are the properties that are pulled from set main csv
        public string Name { get; set; }
        public string Set { get; set; }
        public string SetCode { get; set; }
        public string ID { get; set; }
        public string Type { get; set; }
        public string Attack { get; set; }
        public string Defense { get; set; }
        public string Allegiance { get; set; }
        public string SpecificCost { get; set; }
        public string GenericCost { get; set; }
        public string Artist { get; set; }
        public string Flavor { get; set; }
        public string Color { get; set; }
        public string GeneratedResource { get; set; }
        public string Number { get; set; }
        public string Rarity { get; set; }
        public string Back { get; set; }
        public string Ability { get; set; }

        // These are the properties that pulled from the set properties csv
        public string Border { get; set; }

        // These are the properties that pulled from the card properties csv
        public string Favorite { get; set; }

        // These are the properties that set dynamically created
        public string FrontA { get; set; }
        public string FrontB { get; set; }
        public string FrontC { get; set; }
    }
}