using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private string winningHandsText;
    [SerializeField] private string creditPayText1;
    [SerializeField] private CardSlot[] cardSlots;
    public Sprite[] cardSprites;
    private GameManager gameManager;
    private void OnEnable()
    {
        if (Instance == null)
            Instance = this;

        GameManager.OnGameManagerInitFinished += OnGameManagerInitFinishedEventHandler;
    }

    private void OnDisable()
    {
        GameManager.OnGameManagerInitFinished -= OnGameManagerInitFinishedEventHandler;

    }
    private void OnGameManagerInitFinishedEventHandler()
    {
        //TODO: Initialize UI Manager
        Debug.Log("UI Manager Event Handlker");
    }
    void Start()
    {
        gameManager = GameManager.Instance;
        cardSprites = Resources.LoadAll<Sprite>("Art/Cards");
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
}
