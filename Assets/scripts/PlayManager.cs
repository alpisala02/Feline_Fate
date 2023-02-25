using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour
{
    public static PlayManager Instance;
    public GameState CurrentState;
    public Board board;
    public List<Card> Deck = new List<Card>();
    public HealthBarController P1Health;
    public HealthBarController P2Health;
    public List<CardLogic> playerCards= new List<CardLogic>();
    public float AIDifficulty=0.1f;
    [SerializeField] GameObject blackScreen;
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject loseScreen;
    [SerializeField] GameObject cardProjectilePrefab;
    [SerializeField]TMP_Text deckText;
    [SerializeField]TMP_Text turText;

    public AudioClip cardAudio;
    public AudioClip shotAudio;
    [SerializeField] AudioClip matchMusic;
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip loseMusic;
    void Awake()
    {
        Instance=this;
    }
    private void Start()
    {
        AudioManager.Instance.PlayMusic(matchMusic);
        turText.gameObject.SetActive(false);
        UpdateGameStates(GameState.PreGame);
    }
    void Update()
    {
        deckText.text=Deck.Count.ToString();
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    public void UpdateGameStates(GameState newState)
    {
        CurrentState = newState;
        Debug.Log(newState+"'a geçiliyor.");
        turText.gameObject.SetActive(false);
        if(!Deck.Any())
        {
            if(P1Health.health>P2Health.health)
            {
                blackScreen.SetActive(true);
                winScreen.SetActive(true);
                return;
            }else
            {
                blackScreen.SetActive(true);
                loseScreen.SetActive(true);
                return;
            }
        }
        board.DiscardBuffer=0;
        switch (CurrentState)
        {
            case GameState.PreGame:
                HandleCoinFlip();
                break;
            case GameState.PlayerTurn:
                turText.gameObject.SetActive(true);
                break;
            case GameState.EnemyTurn:
                AIMakeDecision();
                break;
            case GameState.Victory:
                AudioManager.Instance.PlaySound(winSound);
                blackScreen.SetActive(true);
                winScreen.SetActive(true);
                break;
            case GameState.Lose:
                AudioManager.Instance.PlayMusic(loseMusic);
                blackScreen.SetActive(true);
                loseScreen.SetActive(true);
                break;
        }
    }
    void HandleCoinFlip()
    {
        RandomSortDeck();
        int coin = Random.Range(1, 3);
        foreach(CardLogic a in playerCards)
        {
            PlayerDrawCard(a.card);
        }
        if (coin == 1)
            UpdateGameStates(GameState.PlayerTurn);
        else
            UpdateGameStates(GameState.EnemyTurn);
    }
    void AIMakeDecision()
    {
        float decision=(Random.Range(0f,1f))+AIDifficulty;
        Debug.Log("Decision: "+decision);
        if(board.CardsOnBoard.Count==0)
        {
            Debug.Log("No items in field");
            Card card = Deck.FirstOrDefault();
            if(card!=null)
            {
                StartCoroutine(AIPlayMove(card,playType.PlaceOnBoard));
                return;
            }
        }
        if(board.CardsOnBoard.Count==9)
        {
            Card card = Deck.FirstOrDefault(x=>board.CardsOnBoard.Any(y => y == x));
            if(card!=null)
            {
                StartCoroutine(AIPlayMove(card,playType.Discard));
                return;
            }
        }
        if(decision>1)
        {
            //playerin elindeki kartlardan bir tane at.
            Card card = Deck.FirstOrDefault(x=>board.CardsOnBoard.Contains(x)==false && 
            playerCards.Select(x=>x.card).Contains(x));
            if(card!=null)
            {
                StartCoroutine(AIPlayMove(card,playType.PlaceOnBoard));
            }
            else
                AIMakeDecision();

        }else if(decision<0.5f)
        {
            //hasar ye
            Card card = Deck.FirstOrDefault(x=>board.CardsOnBoard.Any(y => y == x));
            if(card!=null)
            {
                StartCoroutine(AIPlayMove(card,playType.Discard));
            }else
                AIMakeDecision();
        }else
        {
            //herhangi bi kart at 
            Card card = Deck.FirstOrDefault(x=>board.CardsOnBoard.Contains(x)==false);
            if(card!=null)
            {
                StartCoroutine(AIPlayMove(card,playType.PlaceOnBoard));
            }
            else
                AIMakeDecision();
        }
    }
    void CardPlacedOnBoard(Card card)
    {
        Deck.Remove(card);
        board.CardPlaced(card);
        Debug.Log(card.name+ " kartı oynadı.");
        UpdateGameStates(GameState.PlayerTurn);
    }
    void CardDiscarded(Card card)
    {
        Deck.Remove(card);
        board.CardDiscarded(card);
        P1Health.TakeDamage();
        Debug.Log(card.name+ " kartı discard etti."); 
        if(P1Health.health<=0)
        {
            UpdateGameStates(GameState.Lose);
            return;
        }
        if(board.DiscardBuffer<3)
            AIMakeDecision();
        else
        {
            P2Health.Heal();
            board.DiscardBuffer=0;
            UpdateGameStates(GameState.PlayerTurn);
        }
    }
    public void PlayerDrawCard(Card card)
    {
        CardLogic cardLogic= playerCards.FirstOrDefault(x=>x.card==card);
        if (cardLogic != null)
        {
            Card newCard = Deck.FirstOrDefault();
            Debug.Log("Yeni kart: " + newCard.CardType);
            Deck.Remove(newCard);
            cardLogic.CardChanged(newCard);
        }
    }
    public void RandomSortDeck()
    {
        System.Random rand = new System.Random();
        Deck = Deck.OrderBy(_ => rand.Next()).ToList();
    }
    public IEnumerator AIPlayMove(Card card,playType type)
    {
        GameObject cardPorjectile = Instantiate(cardProjectilePrefab,board.transform.position,Quaternion.identity);
        cardPorjectile.transform.SetParent(board.transform);
        cardPorjectile.GetComponentInChildren<Image>().sprite=card.CardArt;
        yield return new WaitForSeconds(1.3f);
        switch (type)
        {
            case playType.PlaceOnBoard:
                cardPorjectile.GetComponent<CardProjectile>().Attack(board.cardVisuals[card.CardType].GetComponent<RectTransform>().anchoredPosition);
                AudioManager.Instance.PlaySound(cardAudio);
                yield return new WaitForSeconds(.6f);
                CardPlacedOnBoard(card);  
                DOTween.Kill(cardPorjectile);
                break;
            case playType.Discard:
                cardPorjectile.GetComponent<CardProjectile>().Attack(new Vector2(-1200,-1000),0.3f); 
                AudioManager.Instance.PlaySound(shotAudio);
                yield return new WaitForSeconds(.6f);
                CardDiscarded(card);
                DOTween.Kill(cardPorjectile);
                break;
        }
    }
    public void EndGame()
    {   
        if(CurrentState==GameState.Victory)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        }else if (CurrentState==GameState.Lose)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
public enum playType{
    PlaceOnBoard,
    Discard
}
public enum GameState{
    PreGame,
    PlayerTurn,
    EnemyTurn,
    Victory,
    Lose
}
