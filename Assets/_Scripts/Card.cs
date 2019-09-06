public class Card
{
    private int number;
    private SUIT suit;

    public int Number {
        get { return number; }
        set { number = value; }
    }
    public SUIT Suit
    {
        get { return suit; }
        set { suit = value; }
    }
    public Card(int n, SUIT s)
    {
        number = n;
        suit = s;
    }
    public override string ToString()
    {
        return string.Format("{0} of {1}", suit, number);
    }
    
    public int getSpriteIndex()
    {
        return (13*(int)Suit + number)-1;
    }
}

public enum SUIT
{
    CLUBS,
    DIAMONDS,
    HEARTS,
    SPADES
}
