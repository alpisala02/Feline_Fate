using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Board : MonoBehaviour,IDropHandler
{
    [SerializeField] GameObject cardProjectilePrefab;
    public List<Card> CardsOnBoard=new List<Card>();
    public List<TMP_Text> cardVisuals= new List<TMP_Text>();
    public List<Card> CardsOnDiscardPile=new List<Card>();
    public int DiscardBuffer=0;
    public CardLogic lastCard;
    public void OnDrop(PointerEventData eventData)
    {  
        if(eventData.pointerDrag!=null && PlayManager.Instance.CurrentState == GameState.PlayerTurn)
        {
            lastCard=eventData.pointerDrag.GetComponent<CardLogic>();
            var card=lastCard.card;
            lastCard.transform.localScale=new Vector3(0,0,0);
            if(!CardsOnBoard.Contains(card))
            {
                Debug.Log(card.name + ", board'a atıldı.");
                StartCoroutine(PlayerPlacedCard(card));
            }else
            {
                Debug.Log(card.name + ", discard Pile'a atıldı.");
                StartCoroutine(PlayerDiscardCard(card));
            }
        }
    }
    IEnumerator PlayerPlacedCard(Card card)
    {
        GameObject cardPorjectile = Instantiate(cardProjectilePrefab,transform.position,Quaternion.identity);
        cardPorjectile.transform.SetParent(transform);
        cardPorjectile.GetComponentInChildren<Image>().sprite=card.CardArt;
        cardPorjectile.GetComponent<CardProjectile>().Attack(cardVisuals[card.CardType].GetComponent<RectTransform>().anchoredPosition);
        AudioManager.Instance.PlaySound(PlayManager.Instance.cardAudio);
        yield return new WaitForSeconds(.6f);
        CardPlaced(card);
        PlayManager.Instance.PlayerDrawCard(card);
        PlayManager.Instance.UpdateGameStates(GameState.EnemyTurn);
        lastCard.transform.localScale=new Vector3(1,1,1);
    }
    IEnumerator PlayerDiscardCard(Card card)
    {
        GameObject cardPorjectile = Instantiate(cardProjectilePrefab,transform.position,Quaternion.identity);
        cardPorjectile.transform.SetParent(transform);
        cardPorjectile.GetComponentInChildren<Image>().sprite=card.CardArt;
        cardPorjectile.GetComponent<CardProjectile>().Attack(new Vector2(-1200,1000),0.3f);
        AudioManager.Instance.PlaySound(PlayManager.Instance.shotAudio);
        yield return new WaitForSeconds(.6f);
        PlayManager.Instance.P2Health.TakeDamage();
                if(PlayManager.Instance.P2Health.health<=0)
                {
                    PlayManager.Instance.UpdateGameStates(GameState.Victory);
                }
                PlayManager.Instance.PlayerDrawCard(card);
                CardDiscarded(card);
                if(DiscardBuffer==3)
                {
                    DiscardBuffer=0;
                    PlayManager.Instance.P1Health.Heal();
                    PlayManager.Instance.UpdateGameStates(GameState.EnemyTurn);
                }
        lastCard.transform.localScale=new Vector3(1,1,1);
    }
    public void CardPlaced(Card card)
    { 
        CardsOnBoard.Add(card);
        cardVisuals[card.CardType].color = new Color32(255, 37, 37, 255);
    }

    public void CardDisplaced(Card card)
    {
        CardsOnBoard.Remove(card);
        cardVisuals[card.CardType].color = new Color32(106, 37, 37, 255);
    }
    public void CardDiscarded(Card card)
    {
        CardsOnDiscardPile.Add(card);
        CardDisplaced(card);
        DiscardBuffer++;
    }
}
