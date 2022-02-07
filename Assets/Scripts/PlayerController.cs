using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public ParticleSystem dust;
    Rigidbody2D rb;
    public AudioSource audioJump;
    public AudioSource audioDash;
    public AudioSource audioGotHit;

    CircleCollider2D cc;
    CapsuleCollider2D capsc;
    [SerializeField] Animator animator;
    public ParticleSystem particle;
    public Image heart1;
    public Image heart2;
    public Image heart3;
    bool isAlive = true;
    SpriteRenderer sr;
    Color c;
    Color g;
    SpriteRenderer srGlaze;
    BoxCollider2D chipsCc;
    BoxCollider2D spear;
    CapsuleCollider2D spike;
    PolygonCollider2D sodaPolCol;
    BoxCollider2D sodaBoxCol;
    [SerializeField] Animator animatorGlaze;

    public float playerSpeed;
    public float jumpHeight;
    public float fallMultiplier;
    public float lowerJumpHeight;
    public float invulTime;
    float jumpPressedRememberTime = 0.2f;
    float jumpPressedRemember;
    public LayerMask platformLayer;
    public int hp;
    bool canTakeDamage = true;
    public static PlayerController plrInstance;

    public bool isDashing = false;
    public float dashDistance;
    float doubleTapTime;
    KeyCode lastPressedKeyCode;

    // Start is called before the first frame update
    void Start()
    {
        plrInstance = this;
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CircleCollider2D>();
        capsc = GetComponent<CapsuleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        srGlaze = GameObject.FindGameObjectWithTag("Glaze").GetComponent<SpriteRenderer>();
        c = sr.material.color;
        g = srGlaze.material.color;

        //chipsCc = GameObject.FindGameObjectWithTag("ChipsSpearEnemy").GetComponent<BoxCollider2D>();

        sodaPolCol = GameObject.FindGameObjectWithTag("Soda").GetComponent<PolygonCollider2D>();
        sodaBoxCol = GameObject.FindGameObjectWithTag("Soda").GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(cc, sodaBoxCol, true);

        spear = GameObject.FindGameObjectWithTag("Spear").GetComponent<BoxCollider2D>();

        spike = GameObject.FindGameObjectWithTag("PotatoSpikes").GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        chipsCc = FindObjectOfType<Enemy>().GetComponent<BoxCollider2D>();
        sodaPolCol = GameObject.FindGameObjectWithTag("Soda").GetComponent<PolygonCollider2D>();
        sodaBoxCol = GameObject.FindGameObjectWithTag("Soda").GetComponent<BoxCollider2D>();
        spear = GameObject.FindGameObjectWithTag("Spear").GetComponent<BoxCollider2D>();
        spike = GameObject.FindGameObjectWithTag("PotatoSpikes").GetComponent<CapsuleCollider2D>();
        if (isAlive)
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            Vector2 direction = new Vector2(x, y);
            switch (hp)
            {
                case 2:
                    heart3.enabled = false;
                    break;
                case 1:
                    heart2.enabled = false;
                    break;
                case 0:
                    heart1.enabled = false;
                    break;
            }
            if (hp <= 0 || rb.position.y < -6f)
            {
                StartCoroutine(Die());
            }
            if (!isDashing)
            {
                Walk(direction);
                if (Input.GetKeyDown(KeyCode.A))
                {
                    if (doubleTapTime > Time.time && lastPressedKeyCode == KeyCode.A)
                    {
                        StartCoroutine(Dash(-1f));
                    }
                    else
                    {
                        doubleTapTime = Time.time + 0.5f;
                    }
                    lastPressedKeyCode = KeyCode.A;
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    if (doubleTapTime > Time.time && lastPressedKeyCode == KeyCode.D)
                    {
                        StartCoroutine(Dash(1f));
                    }
                    else
                    {
                        doubleTapTime = Time.time + 0.5f;
                    }
                    lastPressedKeyCode = KeyCode.D;
                }
            }

            jumpPressedRemember -= Time.deltaTime;
            if (Input.GetButtonDown("Jump"))
            {
                jumpPressedRemember = jumpPressedRememberTime;
            }

            if ((jumpPressedRemember > 0) && cc.IsTouchingLayers(platformLayer))
            {
                jumpPressedRemember = 0;
                Jump();
                StartCoroutine(JumpAnimDisabler());
            }

            if (rb.velocity.x > 0)
            {
                animatorGlaze.SetBool("IsWalkingRight", true);
            }
            else if (rb.velocity.x < 0)
            {
                animatorGlaze.SetBool("IsWalkingLeft", true);
            }
            else
            {
                animatorGlaze.SetBool("IsWalkingRight", false);
                animatorGlaze.SetBool("IsWalkingLeft", false);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isAlive)
        {
            if (!isDashing)
            {
                if (rb.velocity.y < 0)
                {
                    rb.velocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
                }
                else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
                {
                    rb.velocity += Vector2.up * Physics2D.gravity.y * lowerJumpHeight * Time.deltaTime;
                }
            }
        }
    }

    void Walk(Vector2 direction)
    {
        rb.velocity = new Vector2(direction.x * playerSpeed, rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canTakeDamage)
        {
            if (!isDashing)
            {
                if (collision.gameObject.CompareTag("ChipsSpearEnemy") && hp > 0)
                {
                    Debug.Log("Collided!!!");
                    StartCoroutine(TakeDamageAndInvulnerable(1));
                }
                if (collision.gameObject.CompareTag("Spear") && hp > 0)
                {
                    StartCoroutine(TakeDamageAndInvulnerable(1));
                }
            }
            if (collision.gameObject.CompareTag("PotatoSpikes") && hp > 0)
            {
                StartCoroutine(TakeDamageAndInvulnerable(1));
            }
            if (collision.gameObject.CompareTag("Soda") && hp > 0)
            {
                StartCoroutine(TakeDamageAndInvulnerable(1));
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (canTakeDamage)
        {
            StartCoroutine(TakeDamageAndInvulnerable(1));
        }
    }

    void Jump()
    {
        CreateDust();
        audioJump.Play();
        animator.SetBool("IsJumping", true);
        animatorGlaze.SetBool("IsJumping", true);
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += Vector2.up * jumpHeight;
    }

    IEnumerator Dash(float direction)
    {
        isDashing = true;
        CreateDust();
        audioDash.Play();
        rb.AddForce(new Vector2(dashDistance * direction, rb.velocity.y), ForceMode2D.Impulse);
        float gravity = rb.gravityScale;
        rb.gravityScale = 0;
        yield return new WaitForSeconds(0.4f);
        isDashing = false;
        rb.gravityScale = gravity;
    }

    IEnumerator TakeDamageAndInvulnerable(int damage)
    {
        canTakeDamage = false;
        audioGotHit.Play();
        c.a = 0.5f;
        sr.material.color = c;
        g.a = 0.5f;
        srGlaze.material.color = g;
        Physics2D.IgnoreCollision(cc, chipsCc, true);
        Physics2D.IgnoreCollision(cc, spear, true);
        Physics2D.IgnoreCollision(cc, spike, true);
        Physics2D.IgnoreCollision(cc, sodaPolCol, true);
        hp -= damage;
        Debug.Log("Damage, hp: " + hp);        
        yield return new WaitForSeconds(invulTime);
        canTakeDamage = true;
        c.a = 1f;
        sr.material.color = c;
        g.a = 1f;
        srGlaze.material.color = g;
        Physics2D.IgnoreCollision(cc, chipsCc, false);
        Physics2D.IgnoreCollision(cc, spear, false);
        Physics2D.IgnoreCollision(cc, spike, false);
        Physics2D.IgnoreCollision(cc, sodaPolCol, false);

    }

    IEnumerator JumpAnimDisabler()
    {
        yield return new WaitForSeconds(0.1f); 
        animator.SetBool("IsJumping", false);
        animatorGlaze.SetBool("IsJumping", false);

    }
    IEnumerator Die()
    {
        particle.Play();
        rb.isKinematic = true;
        isAlive = false;
        sr.enabled = false;
        cc.enabled = false;
        capsc.enabled = false;
        srGlaze.enabled = false;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        playerSpeed = 0;

        yield return new WaitForSeconds(particle.main.startLifetime.constantMax + .5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void CreateDust()
    {
        dust.Play();
    }
}
