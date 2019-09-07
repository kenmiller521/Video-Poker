using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour
{
    [SerializeField] private Text heldText;
    [SerializeField] private Image cardImage;
    [SerializeField] private Button button;

    private bool isCardHeldable;    //Designates if the card can be interacted with during different game states
    private bool isHeld;

    public bool IsCardHeld
    {
        get { return isHeld; }
        private set { isHeld = value; }
    }
    private void OnEnable()
    {
        GameManager.OnGameStateChanged += OnGameStateChangedEventHandler;
    }
    private void OnDisable()
    {
        GameManager.OnGameStateChanged += OnGameStateChangedEventHandler;
    }

    private void Start()
    {
        isHeld = false;
        heldText.gameObject.SetActive(false);
    }

    private void OnGameStateChangedEventHandler(GAMESTATE gameState)
    {
        switch (gameState)
        {
            case GAMESTATE.BETTING:
                isCardHeldable = false;
                IsCardHeld = false;
                button.interactable = isCardHeldable;
                heldText.gameObject.SetActive(false);
                break;
            case GAMESTATE.HAND_DEALT:
                isCardHeldable = true;
                button.interactable = isCardHeldable;
                break;
            case GAMESTATE.NEW_CARDS_DRAWN:
                isCardHeldable = false;
                button.interactable = isCardHeldable;
                break;
            case GAMESTATE.GAME_WON:
                break;
            case GAMESTATE.GAME_LOST:
                break;
        }
    }

    

    public void CardSlotClick()
    {
        if (!isCardHeldable) return;

        isHeld = !isHeld;
        heldText.gameObject.SetActive(!heldText.gameObject.activeSelf);
    }
    public void SetCardUI(Sprite cardSprite)
    {
        cardImage.sprite = cardSprite;
    }
}
