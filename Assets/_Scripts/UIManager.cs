using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    public Text creditsText;
    public Text betText;
    public Sprite[] cardSprites;

    [SerializeField] private CardSlot[] cardSlots;
    [SerializeField] private Text gameResultText;

    [Header("Pay Tier Panels")]
    [SerializeField] private Image panelPayPanel1;
    [SerializeField] private Image panelPayPanel2;
    [SerializeField] private Image panelPayPanel3;
    [SerializeField] private Image panelPayPanel4;
    [SerializeField] private Image panelPayPanel5;

    [Header("Pay Tier Text Elements")]
    [SerializeField] private Text winningHandsText;
    [SerializeField] private Text panelPayPanel1Text;
    [SerializeField] private Text panelPayPanel2Text;
    [SerializeField] private Text panelPayPanel3Text;
    [SerializeField] private Text panelPayPanel4Text;
    [SerializeField] private Text panelPayPanel5Text;

    [Header("Deal/Draw Button")]
    [SerializeField] private Button dealDrawButton;
    [SerializeField] private Text dealDrawButtonText;

    [Header("Bet Buttons")]
    [SerializeField] private Button minimumBetButton;
    [SerializeField] private Button decrementBetButton;
    [SerializeField] private Button incrementBetButton;
    [SerializeField] private Button maximumBetButton;

    private void OnEnable()
    {
        GameManager.OnGameManagerInitFinished += OnGameManagerInitFinishedEventHandler;
        GameManager.OnGameStateChanged += OnGameStateChangedEventHandler;
        GameManager.OnCreditsChanged += OnCreditsChangedEventHandler;
        GameManager.OnBetChanged += OnBetChangedEventHandler;
    }

    private void OnDisable()
    {
        GameManager.OnGameManagerInitFinished -= OnGameManagerInitFinishedEventHandler;
        GameManager.OnGameStateChanged -= OnGameStateChangedEventHandler;
        GameManager.OnCreditsChanged -= OnCreditsChangedEventHandler;
        GameManager.OnBetChanged -= OnBetChangedEventHandler;

    }

    private void OnGameManagerInitFinishedEventHandler()
    {
        BuildPayTableUI();
        UpdateCreditsText();
    }

    private void OnGameStateChangedEventHandler(GAMESTATE gameState)
    {
        switch (gameState)
        {
            case GAMESTATE.BETTING:
                gameResultText.text = "";
                ToggleBetButtons(true);
                dealDrawButton.interactable = true;
                dealDrawButtonText.text = "Deal";
                break;
            case GAMESTATE.HAND_DEALT:
                ToggleBetButtons(false);
                dealDrawButtonText.text = "Draw";
                break;
            case GAMESTATE.NEW_CARDS_DRAWN:
                dealDrawButton.interactable = false;
                break;
            case GAMESTATE.GAME_WON:
                dealDrawButton.interactable = false;
                gameResultText.text = "Winner!\n" + FormatWinTypeText(gameManager.CurrentWinType) + "\n+" + gameManager.GetWinningPayout().ToString();
                break;
            case GAMESTATE.GAME_LOST:
                dealDrawButton.interactable = false;
                gameResultText.text = "No Win!";
                break;
        }
    }

    private void OnCreditsChangedEventHandler()
    {
        UpdateCreditsText();
    }

    private void OnBetChangedEventHandler()
    {
        UpdateBetText();
        UpdateBetTier();
    }

    private string FormatWinTypeText(WINTYPE winType)
    {
        switch (winType)
        {
            case WINTYPE.ROYAL_FLUSH:
                return "Royal Flush";
            case WINTYPE.STRAIGHT_FLUSH:
                return "Straight Flush";
            case WINTYPE.FOUR_OF_A_KIND:
                return "Four Of A Kind";
            case WINTYPE.FULL_HOUSE:
                return "Full House";
            case WINTYPE.FLUSH:
                return "Flush";
            case WINTYPE.STRAIGHT:
                return "Straight";
            case WINTYPE.THREE_OF_A_KIND:
                return "Three Of A Kind";
            case WINTYPE.TWO_PAIR:
                return "Two Pair";
            case WINTYPE.JACKS_OR_BETTER:
                return "Jacks Or Better";
            default:
                return string.Empty;
        }
    }
    
    private void ToggleBetButtons(bool toggleValue)
    {
        minimumBetButton.interactable = toggleValue;
        decrementBetButton.interactable = toggleValue;
        incrementBetButton.interactable = toggleValue;
        maximumBetButton.interactable = toggleValue;
    }

    public void updateCardSlot(Card card, int index)
    {
        if (cardSlots[index].IsCardHeld) return;

        cardSlots[index].SetCardUI(cardSprites[card.getSpriteIndex()]);
    }

    public bool CheckCardHeldStatus(int index)
    {
        return cardSlots[index].IsCardHeld;
    }

    private void UpdateCreditsText()
    {
        creditsText.text = "CREDITS: " + gameManager.Credits;
    }

    private void UpdateBetText()
    {
        betText.text = "BET: " + gameManager.CurrentBet;
    }

    private void UpdateBetTier()
    {
        panelPayPanel1.color = (gameManager.CurrentBet == 1) ? Color.red : Color.blue;
        panelPayPanel2.color = (gameManager.CurrentBet == 2) ? Color.red : Color.blue;
        panelPayPanel3.color = (gameManager.CurrentBet == 3) ? Color.red : Color.blue;
        panelPayPanel4.color = (gameManager.CurrentBet == 4) ? Color.red : Color.blue;
        panelPayPanel5.color = (gameManager.CurrentBet == 5) ? Color.red : Color.blue;
    }

    private void BuildPayTableUI()
    {
        List<PayTableElement> payTable = new List<PayTableElement>(gameManager.PayTable);
        payTable.Reverse();
        winningHandsText.text = payTable[0].winningHandName;
        panelPayPanel1Text.text = payTable[0].basePayout.ToString();
        panelPayPanel2Text.text = (payTable[0].basePayout * 2).ToString();
        panelPayPanel3Text.text = (payTable[0].basePayout * 3).ToString();
        panelPayPanel4Text.text = (payTable[0].basePayout * 4).ToString();
        panelPayPanel5Text.text = (payTable[0].basePayout * 5).ToString();
        for (int i = 1; i < payTable.Count; i++)
        {
            winningHandsText.text += "\n" + payTable[i].winningHandName;
            panelPayPanel1Text.text += "\n" + payTable[i].basePayout.ToString();
            panelPayPanel2Text.text += "\n" + (payTable[i].basePayout * 2).ToString();
            panelPayPanel3Text.text += "\n" + (payTable[i].basePayout * 3).ToString();
            panelPayPanel4Text.text += "\n" + (payTable[i].basePayout * 4).ToString();
            panelPayPanel5Text.text += "\n" + (payTable[i].basePayout * 5).ToString();
        }
    }
}
