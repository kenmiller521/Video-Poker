using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour
{
    [SerializeField] private Text heldText;
    [SerializeField] private Image cardImage;
    private bool isHeld;
    private Card card;

    public bool IsCardHeld
    {
        get { return isHeld; }
        set { isHeld = value; }
    }
    private void Start()
    {
        isHeld = false;
        heldText.gameObject.SetActive(false);
    }
    public void CardSlotCick()
    {
        isHeld = !isHeld;
        heldText.gameObject.SetActive(!heldText.gameObject.activeSelf);
    }
    public void SetCardUI(Sprite cardSprite)
    {
        cardImage.sprite = cardSprite;
    }
}
