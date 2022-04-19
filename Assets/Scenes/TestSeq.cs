using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class TestSeq : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MySequence();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void MySequence()
    {
        Sequence s = DOTween.Sequence();
        for(var i = 0; i < 2; i++)
        {
            s.AppendCallback(() => Debug.Log("HI"));
            s.AppendInterval(0.2f);
            s.Append(transform.DOLocalMoveX(100, 10));

        }

    }
}
