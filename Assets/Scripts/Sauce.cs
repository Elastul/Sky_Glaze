using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sauce : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] Animator animator;
    public AudioSource audioBlast;
    BoxCollider2D bc;
    public ParticleSystem sauceBlastPS;
    float triggerDistanceY;
    float triggerDistanceX;
    float shotTime;
    public float startShotTime;
    bool isActive = false;
    bool canBlast = false;
    Color rayColor = Color.red;

    // Start is called before the first frame update
    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        triggerDistanceX = bc.bounds.extents.x * 10f;
        triggerDistanceY = bc.bounds.extents.y * 10f;
    }

    // Update is called once per frame
    void Update()
    {
        TriggerStartEndZone();
        if (isActive && canBlast)
        {
            StartCoroutine(BlastSauce());
        }
    }

    IEnumerator BlastSauce()
    {
        
        animator.SetBool("IsBlasting",true);
        canBlast = false;
        yield return new WaitForSeconds(.5f); 
        Blast();
        yield return new WaitForSeconds(3f);
        animator.SetBool("IsBlasting", false);
        canBlast = true;
    }

    bool TriggerStartEndZone()
    {
        RaycastHit2D raycastHit1 = Physics2D.Raycast(bc.bounds.center + new Vector3(triggerDistanceX, 0), Vector2.up, triggerDistanceY, layerMask);
        RaycastHit2D raycastHit2 = Physics2D.Raycast(bc.bounds.center - new Vector3(triggerDistanceX, 0), Vector2.up, triggerDistanceY, layerMask); 
        RaycastHit2D raycastHitExit1 = Physics2D.Raycast(bc.bounds.center + new Vector3(triggerDistanceX + bc.bounds.extents.x * 2.3f, 0), Vector2.up, triggerDistanceY, layerMask);
        RaycastHit2D raycastHitExit2 = Physics2D.Raycast(bc.bounds.center - new Vector3(triggerDistanceX + bc.bounds.extents.x * 2.3f, 0), Vector2.up, triggerDistanceY, layerMask);
        
        if ((raycastHit1.collider != null || raycastHit2.collider != null) && !isActive)
        {
            canBlast = true;
            isActive = true;
            rayColor = Color.green;
        }
        else if (raycastHitExit1.collider != null || raycastHitExit2.collider != null)
        {
            isActive = false;
            rayColor = Color.red;
        }
        Debug.DrawRay(bc.bounds.center + new Vector3(triggerDistanceX, 0), Vector2.up * triggerDistanceY, rayColor);
        Debug.DrawRay(bc.bounds.center - new Vector3(triggerDistanceX, 0), Vector2.up * triggerDistanceY, rayColor); 
        Debug.DrawRay(bc.bounds.center + new Vector3(triggerDistanceX + bc.bounds.extents.x*2.3f, 0), Vector2.up * triggerDistanceY, rayColor);
        Debug.DrawRay(bc.bounds.center - new Vector3(triggerDistanceX + bc.bounds.extents.x*2.3f, 0), Vector2.up * triggerDistanceY, rayColor);
        return rayColor == Color.green;
    }

    void Blast()
    {
        sauceBlastPS.Play(); 
        audioBlast.Play();
    }
}
