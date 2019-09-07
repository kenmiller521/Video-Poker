using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    public const int NumberOfSuits = 4;
    public const int NumberOfRanks = 13;

    private List<Card> cards = new List<Card>();
    private int deckCounter;

    public Deck()
    {
        deckCounter = 0;
        CreateDeck();
        ShuffleDeck();
    }
    private void CreateDeck()
    {
        for (int i = 0; i < NumberOfSuits; i++)
            for (int j = 0; j < NumberOfRanks; j++)
                cards.Add(new Card((RANK)j + 1, (SUIT)i));
    }
    private void ShuffleDeck()
    {
        for(int i = 0; i < cards.Count; i++)
        {
            int randIndex = Random.Range(0, cards.Count);
            Card tempCard = cards[randIndex];
            cards[randIndex] = cards[i];
            cards[i] = tempCard;
        }
    }
    private void PrintDeck()
    {
        foreach(Card card in cards)
            Debug.Log(card.ToString());
    }

    public Card DealCard()
    {
        if(deckCounter == cards.Count)
        {
            deckCounter = 0;
            ShuffleDeck();
        }

        return cards[deckCounter++];
    }
}
