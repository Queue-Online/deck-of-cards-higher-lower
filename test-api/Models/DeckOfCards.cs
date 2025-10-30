namespace test_api.Models;

public record Card(
    string Code,
    string Image,
    CardImages Images,
    string Value,
    string Suit
);

public record CardImages(
    string Svg,
    string Png
);

public record Deck(
    bool Success,
    string DeckId,
    bool Shuffled,
    int Remaining
);

public record DrawCardsResponse(
    bool Success,
    string DeckId,
    Card[] Cards,
    int Remaining
);

public record CreateDeckRequest(
    int DeckCount = 1
);

public record DrawCardsRequest(
    int Count = 2
);

// API response models for mapping
public record DeckApiResponse(
    bool Success,
    string Deck_Id,
    bool Shuffled,
    int Remaining
);

public record DrawCardsApiResponse(
    bool Success,
    string Deck_Id,
    CardApiResponse[] Cards,
    int Remaining
);

public record CardApiResponse(
    string Code,
    string Image,
    CardImagesApiResponse Images,
    string Value,
    string Suit
);

public record CardImagesApiResponse(
    string Svg,
    string Png
);