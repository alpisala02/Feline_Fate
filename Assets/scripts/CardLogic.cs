using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardLogic : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler,IPointerEnterHandler,IPointerExitHandler
{
    RectTransform rectTransfrom;
    CanvasGroup canvasGroup;
    [SerializeField]Canvas canvas;
    Vector2 startingPosition;
    Quaternion startingRotation;
    public Card card;
    Image inspectionImage;
    [SerializeField]Image cardImage;
    [SerializeField]GameObject cardBackImage;

    void Awake()
    {
        rectTransfrom =GetComponent<RectTransform>();
        canvasGroup=GetComponent<CanvasGroup>();
        startingPosition=rectTransfrom.anchoredPosition;
        startingRotation=rectTransfrom.rotation;
        cardImage.sprite=card.CardArt;
        inspectionImage=GameObject.FindGameObjectWithTag("Inspection").GetComponent<Image>();
        Flip();
    }
    public void CardChanged(Card _card)
    {
        card=_card;
        cardImage.sprite=card.CardArt;
    }
    void Start()
    {
        inspectionImage.gameObject.SetActive(false);
    }
    public void Flip()
    {
        cardBackImage.SetActive(!cardBackImage.activeInHierarchy);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts=false;
        inspectionImage.gameObject.SetActive(false);
        cardImage.color=new Color32(255,255,255,170);
        rectTransfrom.rotation=Quaternion.identity;
    }
    public void OnDrag(PointerEventData eventData)
    {
        rectTransfrom.anchoredPosition += eventData.delta / canvas.scaleFactor;
        //transform.position=Input.mousePosition;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts=true;
        cardImage.color=new Color32(255,255,255,255);
        rectTransfrom.anchoredPosition= startingPosition;
        rectTransfrom.rotation=startingRotation;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inspectionImage.gameObject.SetActive(true);
        inspectionImage.sprite=card.CardArt;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inspectionImage.gameObject.SetActive(false);
    }
}
