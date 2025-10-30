namespace DeckOfCardsWeb.Models;

public class DeckResponse
{
    public bool Success { get; set; }
    public string DeckId { get; set; } = string.Empty;
    public bool Shuffled { get; set; }
    public int Remaining { get; set; }
}

public class DrawCardsResponse
{
    public bool Success { get; set; }
    public string DeckId { get; set; } = string.Empty;
    public Card[] Cards { get; set; } = Array.Empty<Card>();
    public int Remaining { get; set; }
}

public class Card
{
    public string Code { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public CardImages Images { get; set; } = new();
    public string Value { get; set; } = string.Empty;
    public string Suit { get; set; } = string.Empty;
}

public class CardImages
{
    public string Svg { get; set; } = string.Empty;
    public string Png { get; set; } = string.Empty;
}