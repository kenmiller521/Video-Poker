using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Deck deck;
    [SerializeField] private PayTableElement[] payTable;
    private UIManager uiManager;

    public delegate void OnGameManagerInitFinishedEvent();
    public static event OnGameManagerInitFinishedEvent OnGameManagerInitFinished;
    private void OnEnable()
    {
        if(Instance == null)
            Instance = this;
    }
    void Start()
    {
        uiManager = UIManager.Instance;
        deck = new Deck();
        OnGameManagerInitFinished?.Invoke();
    }
    public void Deal()
    {
        for(int i = 0; i < 5; i++)
        {
            if(uiManager.CheckCardHeldStatus(i) == false)
            {
                Card card = deck.DealCard();
                uiManager.updateCardSlot(card, i);
            }            
        }
    }
}
