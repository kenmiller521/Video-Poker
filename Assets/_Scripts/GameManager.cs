using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;
    public bool isGameHigh = false;
    public Deck deck;
    public float winGameResetDelayAmount = 3;
    public float loseGameResetDelayAmount = 1;
    [SerializeField] private int startingCredits = 500;
    [SerializeField] private PayTableElement[] payTable;
    private List<Card> hand = new List<Card>();
    private int credits;
    private GAMESTATE currentGameState;
    private WINTYPE currentWinType;
    private int currentBet = 1;
    private Card[] orderedHand;

    public delegate void OnGameManagerInitFinishedEvent();
    public static event OnGameManagerInitFinishedEvent OnGameManagerInitFinished;
    public delegate void OnGameStateChangedEvent(GAMESTATE gameState);
    public static event OnGameStateChangedEvent OnGameStateChanged;
    public delegate void OnCreditsChangedEvent();
    public static event OnCreditsChangedEvent OnCreditsChanged;
    public delegate void OnBetChangedEvent();
    public static event OnBetChangedEvent OnBetChanged;

    public int Credits {
        get { return credits; }
        private set {
            credits = value;
            OnCreditsChanged?.Invoke();
        }
    }
    public int CurrentBet
    {
        get { return currentBet; }
        private set {
            currentBet = value;
            OnBetChanged?.Invoke();
        }
    }
    public GAMESTATE CurrentGameState
    {
        get { return currentGameState; }
        private set
        {
            currentGameState = value;
            OnGameStateChanged?.Invoke(currentGameState);
        }
    }
    public WINTYPE CurrentWinType
    {
        get { return currentWinType; }
        private set { currentWinType = value; }
    }
    public PayTableElement[] PayTable
    {
        get { return payTable; }
        private set { payTable = value; }
    }

    private void OnEnable()
    {
        OnGameStateChanged += OnGameStateChangedEventHandler;
    }

    private void OnDisable()
    {
        OnGameStateChanged -= OnGameStateChangedEventHandler;
    }

    void Start()
    {
        Credits = startingCredits;
        CurrentBet = 1;
        CurrentGameState = GAMESTATE.BETTING;
        deck = new Deck();
        OnGameManagerInitFinished?.Invoke();
    }

    private void OnGameStateChangedEventHandler(GAMESTATE gameState)
    {
        switch (gameState)
        {
            case GAMESTATE.BETTING:
                CurrentWinType = WINTYPE.NONE;
                break;
            case GAMESTATE.HAND_DEALT:
                Credits -= CurrentBet;
                CheckHand();
                break;
            case GAMESTATE.NEW_CARDS_DRAWN:
                CheckHand();
                break;
            case GAMESTATE.GAME_WON:
                Credits += GetWinningPayout();
                StartCoroutine(DelayedGameReset(winGameResetDelayAmount));
                break;
            case GAMESTATE.GAME_LOST:
                StartCoroutine(DelayedGameReset(loseGameResetDelayAmount));
                break;
        }
    }
    
    public void Deal()
    {
        if (Credits == 0) return;

        if (currentGameState == GAMESTATE.BETTING)
        {
            hand.Clear();
            for (int i = 0; i < 5; i++)
            {
                Card card = deck.DealCard();
                hand.Add(card);
                uiManager.updateCardSlot(card, i);
            }
            CurrentGameState = GAMESTATE.HAND_DEALT;
        }
        else if(currentGameState == GAMESTATE.HAND_DEALT)
        {
            for (int i = 0; i < 5; i++)
            {
                if (uiManager.CheckCardHeldStatus(i) == false)
                {
                    Card card = deck.DealCard();
                    hand[i] = card;
                    uiManager.updateCardSlot(card, i);
                }
            }
            CurrentGameState = GAMESTATE.NEW_CARDS_DRAWN;
        }
    }

    public void CheckHand()
    {
        if(isGameHigh)
            for(int i = 0; i < hand.Count; i++)
                if (hand[i].Rank == RANK.ACELOW)
                    hand[i].Rank = RANK.ACEHIGH;

        var handOrdered = from card in hand
                          orderby card.Rank descending
                          select card;

        orderedHand = handOrdered.ToArray();

        if(currentGameState == GAMESTATE.HAND_DEALT) //Cannot lose in this state, you either win or replace non-held cards
        {
            if (CheckWin())
                CurrentGameState = GAMESTATE.GAME_WON;
        }
        else if(currentGameState == GAMESTATE.NEW_CARDS_DRAWN) //You can only either win or lose
        {
            CurrentGameState = CheckWin() ? GAMESTATE.GAME_WON : GAMESTATE.GAME_LOST;
        }
    }

    private bool CheckWin()
    {
        return CheckRoyalFlush() || 
            CheckStraightFlush() || 
            CheckFourOfAKind() ||
            CheckFullHouse() || 
            CheckFlush() || 
            CheckStraight() || 
            CheckThreeOfAKind() || 
            CheckTwoPair() || 
            CheckJacksOrBetter();
    }

    public int GetWinningPayout()
    {
        return payTable[(int)CurrentWinType].basePayout * CurrentBet;
    }

    public void IncrementBet()
    {
        if(CurrentBet != 5)
            CurrentBet += 1;
    }

    public void DecrementBet()
    {
        if(CurrentBet != 1)
        CurrentBet -= 1;
    }

    public void MinimumBet()
    {
        CurrentBet = 1;
    }

    public void MaximumBet()
    {
        CurrentBet = 5;
    }

    IEnumerator DelayedGameReset(float delayAmount)
    {
        yield return new WaitForSeconds(delayAmount);
        CurrentGameState = GAMESTATE.BETTING;
    }

    #region Winning Hand Checks
    private bool CheckRoyalFlush()
    {
        Card startingCard = orderedHand[0];

        if(startingCard.Rank != RANK.ACEHIGH)
            return false;

        if (CheckStraightFlush())
        {
            CurrentWinType = WINTYPE.ROYAL_FLUSH;
            return true;
        }
        else return false;
    }
    private bool CheckStraightFlush()
    {
        if (CheckStraight() && CheckFlush())
        {
            CurrentWinType = WINTYPE.STRAIGHT_FLUSH;
            return true;
        }
        else return false;
    }

    private bool CheckFourOfAKind()
    {
        if ((orderedHand[0].Rank == orderedHand[3].Rank) || (orderedHand[1].Rank == orderedHand[4].Rank))
        {
            CurrentWinType = WINTYPE.FOUR_OF_A_KIND;
            return true;
        }
        else return false;
    }

    private bool CheckFullHouse()
    {
        Card startingCard = orderedHand[0];

        //If the first and third card the same, that means the second card is the same
        bool isMatch3First = startingCard.Rank == orderedHand[2].Rank;
        bool isMatch2First = startingCard.Rank == orderedHand[1].Rank;

        if (isMatch3First) //If first 3 cards are matched
        {
            if (orderedHand[3].Rank == orderedHand[4].Rank)//Only need to check last 2 cards
            {
                CurrentWinType = WINTYPE.FULL_HOUSE;
                return true;
            }
            else return false;
        }
        else if (isMatch2First) //Else if first 2 cards match, then only need to check 3rd and 5th card
        {
            if (orderedHand[2].Rank == orderedHand[4].Rank)
            {
                CurrentWinType = WINTYPE.FULL_HOUSE;
                return true;
            }
            else return false;
        }
        else //else return false
            return false;
    }
    private bool CheckFlush()
    {
        Card startingCard = orderedHand[0];
        int matchingCount = 0;

        for (int i = 1; i < orderedHand.Length; i++)
        {
            if (startingCard.Suit == orderedHand[i].Suit)
                matchingCount++;
            else
                matchingCount--;

            startingCard = orderedHand[i];
        }

        if (matchingCount == 4)
        {
            CurrentWinType = WINTYPE.FLUSH;
            return true;
        }
        else return false;

    }
    private bool CheckStraight()
    {
        Card startingCard = orderedHand[0];
        int matchingCount = 0;
        for (int i = 1; i < orderedHand.Length; i++)
        {
            if (startingCard.Rank == orderedHand[i].Rank + 1)
            {
                startingCard = orderedHand[i];
                matchingCount++;
            }
        }
        if (matchingCount == 4)
        {
            CurrentWinType = WINTYPE.STRAIGHT;
            return true;
        }
        else return false;
    }

    private bool CheckThreeOfAKind()
    {
        Card startingCard = orderedHand[0];
        bool isLeftType = orderedHand[2].Rank == orderedHand[0].Rank; //If first 3 cards are same
        bool isMiddleType = orderedHand[2].Rank == orderedHand[1].Rank && orderedHand[2].Rank == orderedHand[3].Rank; //If middle 3 cards are same
        bool isRightType = orderedHand[2].Rank == orderedHand[4].Rank;  //If last 3 cards are same

        if (isLeftType)
        {
            //Only need to check last 2 are not equal
            if (orderedHand[3].Rank != orderedHand[4].Rank)
            {
                CurrentWinType = WINTYPE.THREE_OF_A_KIND;
                return true;
            }
            else return false;
        }
        else if (isMiddleType)
        {
            //Only need to check first and last are not equal
            if (orderedHand[1].Rank != orderedHand[4].Rank)
            {
                CurrentWinType = WINTYPE.THREE_OF_A_KIND;
                return true;
            }
            else return false;
        }
        else if (isRightType)
        {   //Only need to check first and second are not equal
            if (orderedHand[0].Rank != orderedHand[1].Rank)
            {
                CurrentWinType = WINTYPE.THREE_OF_A_KIND;
                return true;
            }
            else return false;
        }
        else return false;
    }

    private bool CheckTwoPair()
    {
        Card startingCard = orderedHand[0];
        int matchingCount = 0;
        for (int i = 1; i < orderedHand.Length; i++)
        {
            if (startingCard.Rank == orderedHand[i].Rank)
                matchingCount++;

            startingCard = orderedHand[i];
        }

        if (matchingCount == 2)
        {
            CurrentWinType = WINTYPE.TWO_PAIR;
            return true;
        }
        else return false;
    }

    private bool CheckJacksOrBetter()
    {
        Card startingCard = orderedHand[0];
        int matchingCount = 0;
        for (int i = 1; i < orderedHand.Length; i++)
        {
            if (startingCard.Rank >= RANK.JACK && startingCard.Rank == orderedHand[i].Rank)
                matchingCount++;

            startingCard = orderedHand[i];
        }
        if (matchingCount == 1)
        {
            CurrentWinType = WINTYPE.JACKS_OR_BETTER;
            return true;
        }
        else return false;
    }
    #endregion
}

public enum GAMESTATE
{
    BETTING,    
    HAND_DEALT,
    NEW_CARDS_DRAWN,
    GAME_WON,
    GAME_LOST
}

public enum WINTYPE
{
    JACKS_OR_BETTER,
    TWO_PAIR,
    THREE_OF_A_KIND,
    STRAIGHT,
    FLUSH,
    FULL_HOUSE,
    FOUR_OF_A_KIND,
    STRAIGHT_FLUSH,
    ROYAL_FLUSH,
    NONE
}
