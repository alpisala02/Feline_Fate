using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CardProjectile : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Vector3 targetScale;
    public void Attack(Vector2 pos,float speed=0.5f)
    {
        if(this!=null)
        {
            transform.DOLookAt(pos,speed/4);
            transform.DOScale(targetScale,speed/2);
            rectTransform.DOAnchorPos(pos,speed);
        }
    }
}
