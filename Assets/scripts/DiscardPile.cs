using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiscardPile : MonoBehaviour,IDropHandler
{
    public List<Card> CardsOnDiscardPile=new List<Card>();
    public int DiscardBuffer=0;
    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag!=null && PlayManager.Instance.CurrentState == GameState.PlayerTurn)
        {
            var card=eventData.pointerDrag.GetComponent<CardLogic>().card;
            if(PlayManager.Instance.board.CardsOnBoard.Contains(card))
            {
                Debug.Log(card.name + ", discard Pile'a atıldı.");
                PlayManager.Instance.P2Health.TakeDamage();
                if(PlayManager.Instance.P2Health.health<=0)
                {
                    PlayManager.Instance.UpdateGameStates(GameState.Victory);
                    return;
                }
                PlayManager.Instance.PlayerDrawCard(card);
                CardDiscarded(card);
                if(DiscardBuffer==3)
                {
                    DiscardBuffer=0;
                    PlayManager.Instance.P1Health.Heal();
                    PlayManager.Instance.UpdateGameStates(GameState.EnemyTurn);
                }
            }
        }
    }
    public void CardDiscarded(Card card)
    {
        CardsOnDiscardPile.Add(card);
        PlayManager.Instance.board.CardDisplaced(card);
        DiscardBuffer++;
    }
}
