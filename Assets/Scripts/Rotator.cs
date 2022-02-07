using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    CapsuleCollider2D playerCc;
    CircleCollider2D playerCc2;
    BoxCollider2D bc;
    SpriteRenderer renderer;
    bool hasAppeared = false;

    private void Start()
    {
        playerCc = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>();
        playerCc2 = GameObject.FindGameObjectWithTag("Player").GetComponent<CircleCollider2D>();
        bc = GetComponent<BoxCollider2D>();
        renderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    private void Update()
    {
        if (renderer.isVisible)
        {
            hasAppeared = true;
        }
        if (hasAppeared)
        {
            if (!renderer.isVisible)
            {
                Destroy(gameObject, .1f);
            }
        }
        if (bc.IsTouching(playerCc) || bc.IsTouching(playerCc2))
        {
            Destroy(gameObject);
        }
    }

}
