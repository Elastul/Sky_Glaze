using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleEnemy : MonoBehaviour
{
    BoxCollider2D bc;
    public float speed;
    [SerializeField] LayerMask layerMask;
    public AudioSource audioSodaBlast;
    Transform playerTransform;
    SpriteRenderer renderer;
    float triggerDistanceX;
    float triggerDistanceY;
    bool hasTriggered = false;
    bool hasAppeared = false;
    public bool isRight;

    // Start is called before the first frame update
    void Start()
    {
        speed = -speed;
        bc = GetComponent<BoxCollider2D>();
        renderer = GetComponent<SpriteRenderer>();
        triggerDistanceX = bc.bounds.extents.x * 25;
        triggerDistanceY = bc.bounds.extents.y * 1.5f;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
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
        if(!hasTriggered && TriggerStartZone())
        {
            hasTriggered = true;
            audioSodaBlast.Play();
            Debug.Log(TriggerStartZone());
        } else if(hasTriggered){
            Blow(speed);
        }        
    }

    bool TriggerStartZone()
    {
        if (!isRight)
        {
            float distance = Vector2.Distance(playerTransform.position, transform.position);
            Vector2 vectorSize = new Vector2(10, bc.bounds.size.y);
            RaycastHit2D raycastHit1 = Physics2D.Raycast(bc.bounds.center + new Vector3(0, triggerDistanceY), Vector2.left, triggerDistanceX, layerMask);
            RaycastHit2D raycastHit2 = Physics2D.Raycast(bc.bounds.center - new Vector3(0, triggerDistanceY), Vector2.left, triggerDistanceX, layerMask);
            Color rayColor;
            if (raycastHit1.collider != null || raycastHit2.collider != null)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }
            Debug.DrawRay(bc.bounds.center + new Vector3(0, triggerDistanceY), Vector2.left * triggerDistanceX, rayColor);
            Debug.DrawRay(bc.bounds.center - new Vector3(0, triggerDistanceY), Vector2.left * triggerDistanceX, rayColor);
            return rayColor == Color.green;
        } else
        {
            float distance = Vector2.Distance(playerTransform.position, transform.position);
            Vector2 vectorSize = new Vector2(10, bc.bounds.size.y);
            RaycastHit2D raycastHit1 = Physics2D.Raycast(bc.bounds.center + new Vector3(0, triggerDistanceY), Vector2.right, triggerDistanceX, layerMask);
            RaycastHit2D raycastHit2 = Physics2D.Raycast(bc.bounds.center - new Vector3(0, triggerDistanceY), Vector2.right, triggerDistanceX, layerMask);
            Color rayColor;
            if (raycastHit1.collider != null || raycastHit2.collider != null)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }
            Debug.DrawRay(bc.bounds.center + new Vector3(0, triggerDistanceY), Vector2.right * triggerDistanceX, rayColor);
            Debug.DrawRay(bc.bounds.center - new Vector3(0, triggerDistanceY), Vector2.right * triggerDistanceX, rayColor);
            return rayColor == Color.green;
        }
    }

    void Blow(float speed)
    {
        if (!isRight)
        {
            transform.position = new Vector2(transform.position.x + speed * Time.deltaTime, transform.position.y);
        } else
        {
            transform.position = new Vector2(transform.position.x + -speed * Time.deltaTime, transform.position.y);
        }
        
    }
}
