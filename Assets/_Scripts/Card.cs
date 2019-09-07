public class Card
{
    private RANK rank;
    private SUIT suit;

    public RANK Rank {
        get { return rank; }
        set { rank = value; }
    }

    public SUIT Suit
    {
        get { return suit; }
        private set { suit = value; }
    }
    public Card(RANK r, SUIT s)
    {
        rank = r;
        suit = s;
    }
    public override string ToString()
    {
        return string.Format("{0} of {1}", rank, suit);
    }
    
    public int getSpriteIndex()
    {
        return(13 * (int)Suit + (int)((rank == RANK.ACEHIGH) ? RANK.ACELOW : rank)) - 1;
    }
}

public enum SUIT
{
    CLUBS,
    DIAMONDS,
    HEARTS,
    SPADES
}

public enum RANK
{
    ACELOW = 1,
    TWO = 2,
    THREE = 3,
    FOUR = 4,
    FIVE = 5,
    SIX = 6,
    SEVEN = 7,
    EIGHT = 8,
    NINE = 9,
    TEN = 10,
    JACK = 11,
    QUEEN = 12,
    KING = 13,
    ACEHIGH = 14
}
