using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class MatchStartScreen : MonoBehaviour
{
    [SerializeField]TMP_Text text;   

    void Start()
    {
        StartCoroutine(StartGame());
    }
    IEnumerator StartGame()
    {
        text.transform.DOScale(new Vector3(1,1,1),1f);
        yield return new WaitForSeconds(1f);
        DOTween.Kill(text.gameObject);
        Destroy(this.gameObject);
    }
}
