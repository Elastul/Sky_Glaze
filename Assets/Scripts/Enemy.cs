using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineInternal;

public class Enemy : MonoBehaviour
{
    CapsuleCollider2D playerCc;
    BoxCollider2D bc;
    public ParticleSystem particle;
    float shotTime;
    public float startShotTime;
    public GameObject spear;
    Transform playerTransform;
    [SerializeField] LayerMask platformLayerMask;
    [SerializeField] Animator animator;
    public AudioSource audioThrowSpear;
    SpriteRenderer sr;
    public bool canShoot = true;
    public float speed;
    public float spearSpeed;
    float distance;
    public float nearDistance;
    bool isAlive = true;
    float plrX;
    float plrY;
    float lastDirMove;


    // Start is called before the first frame update
    void Start()
    {
        shotTime = startShotTime;
        bc = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        playerCc = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (bc.IsTouching(playerCc) && PlayerController.plrInstance.isDashing == true && isAlive)
        {
            animator.SetBool("IsWalking", false);
            isAlive = false;
            StartCoroutine(Die());
        }

        
    }
    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            distance = Vector2.Distance(playerTransform.position, transform.position);
            
            if (distance <= nearDistance)
            {

                if (canShoot)
                {
                    if (shotTime <= 0)
                    {
                        plrY = playerTransform.position.y;
                        plrX = playerTransform.position.x;
                        animator.SetBool("IsAttacking", true);
                        Vector2 aimDir = new Vector2(plrX - transform.position.x, plrY - transform.position.y).normalized;
                        float _angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
                        Vector3 rot = new Vector3(0, 0, _angle - 90);
                        GameObject spearObject = Instantiate(spear, transform.position, Quaternion.Euler(rot));
                        spearObject.GetComponent<Rigidbody2D>().velocity = aimDir * spearSpeed; 
                        audioThrowSpear.Play();
                        shotTime = startShotTime;

                    }
                    else
                    {
                        animator.SetBool("IsAttacking", false);
                        shotTime -= Time.deltaTime;
                    }
                }
                else if (IsGrounded())
                {
                    if (playerTransform.position.x - transform.position.x > 0)
                    {
                        lastDirMove = -1;
                        animator.SetBool("IsRight", true);
                        animator.SetBool("IsLeft", false);
                        animator.SetBool("IsWalking", true);
                        transform.position = new Vector2(transform.position.x + speed * Time.deltaTime, transform.position.y);
                    }
                    else if (playerTransform.position.x - transform.position.x < 0)
                    {
                        lastDirMove = 1;
                        animator.SetBool("IsRight", false);
                        animator.SetBool("IsLeft", true);
                        animator.SetBool("IsWalking", true);
                        transform.position = new Vector2(transform.position.x + -speed * Time.deltaTime, transform.position.y);
                    }
                }
                else if (!IsGrounded())
                {
                    transform.position = new Vector2(transform.position.x + speed * lastDirMove * Time.deltaTime, transform.position.y);
                }
            }
            else if (!canShoot && !(distance <= nearDistance))
            {
                animator.SetBool("IsWalking", false);
            }
        }        
    }

    IEnumerator Die()
    {
        particle.Play();
        sr.enabled = false;
        bc.enabled = false;
        yield return new WaitForSeconds(particle.main.startLifetime.constantMax);

        Destroy(gameObject);
    }

    bool IsGrounded()
    {
        float extraHeight = .1f;
        RaycastHit2D raycastHit = Physics2D.Raycast(bc.bounds.center, Vector2.down, bc.bounds.extents.y + extraHeight, platformLayerMask);
        
        return raycastHit.collider != null;
    }

}
