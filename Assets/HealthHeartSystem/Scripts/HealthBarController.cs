using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public int maxHealth;
    public int health;
    private GameObject[] heartContainers;
    private Image[] heartFills;
    public Transform heartsParent;
    public GameObject heartContainerPrefab;
    private void Start()
    {
        heartContainers = new GameObject[9];
        heartFills = new Image[9];
        health=maxHealth;
        InstantiateHeartContainers();
        UpdateHeartsHUD();
    }
    public void TakeDamage()
    {
        health--;
        Mathf.Clamp(health,0,maxHealth);
        UpdateHeartsHUD();
    }
    public void Heal()
    {
        health++;
        UpdateHeartsHUD();
    }
    public void UpdateHeartsHUD()
    {
        SetHeartContainers();
        SetFilledHearts();
    }

    void SetHeartContainers()
    {
        for (int i = 0; i < heartContainers.Length; i++)
        {
            if (i < maxHealth)
            {
                heartContainers[i].SetActive(true);
            }
            else
            {
                heartContainers[i].SetActive(false);
            }
        }
    }

    void SetFilledHearts()
    {
        for (int i = 0; i < heartFills.Length; i++)
        {
            if (i < health)
            {
                heartFills[i].fillAmount = 1;
            }
            else
            {
                heartFills[i].fillAmount = 0;
            }
        }

        if (health % 1 != 0)
        {
            int lastPos = Mathf.FloorToInt(health);
            heartFills[lastPos].fillAmount = health % 1;
        }
    }

    void InstantiateHeartContainers()
    {
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab);
            temp.transform.SetParent(heartsParent, false);
            heartContainers[i] = temp;
            heartFills[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
        }
    }
}
