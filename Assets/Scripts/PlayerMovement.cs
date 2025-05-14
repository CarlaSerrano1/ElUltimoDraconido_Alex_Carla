using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float wallSlideSpeed = 2f; // Velocidad al deslizarse por la pared
    [SerializeField] private float wallJumpForceX = 6f; // Fuerza horizontal al saltar desde la pared
    [SerializeField] private float wallJumpForceY = 8f; // Fuerza vertical al saltar desde la pared
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;
    public HealthBarHUDTester healthBar;
    public PlayerStats playerStats;
    private float dañoObstaculo = 0.5f;
    private bool invulnerable = false;
    private bool isWallSliding = false; // Indica si el jugador está deslizándose en una pared
    private bool isDead = false;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerStats = GetComponent<PlayerStats>();
        healthBar = FindObjectOfType<HealthBarHUDTester>();

        if (healthBar == null)
        {
            Debug.LogError("No se encontró el script healthBar en la escena.");
        }
    }

    private void Update()
{
    if (isDead) return; // Evita cualquier acción si el jugador está muerto

    horizontalInput = Input.GetAxis("Horizontal");

    if (horizontalInput > 0.01f)
        transform.localScale = Vector3.one;
    else if (horizontalInput < -0.01f)
        transform.localScale = new Vector3(-1, 1, 1);

    anim.SetBool("run", horizontalInput != 0);
    anim.SetBool("grounded", isGrounded());

    if (onWall() && !isGrounded() && body.velocity.y < 0)
    {
        isWallSliding = true;
        body.velocity = new Vector2(body.velocity.x, -wallSlideSpeed);
    }
    else
    {
        isWallSliding = false;
    }

    if (wallJumpCooldown > 0.2f)
    {
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
    }
    else
        wallJumpCooldown += Time.deltaTime;
}



    private void Jump()
    {
        if (isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            anim.SetTrigger("jump");
        }
        else if (isWallSliding) // Si está en la pared, hacer un wall jump
        {
            float jumpDirection = -Mathf.Sign(transform.localScale.x); // Dirección opuesta a la pared
            body.velocity = new Vector2(jumpDirection * wallJumpForceX, wallJumpForceY);
            transform.localScale = new Vector3(jumpDirection, 1, 1); // Voltear personaje
            wallJumpCooldown = 0;
        }
    }

    private bool isGrounded()
{
    // Si el jugador está en una pared, no lo consideramos en el suelo
    if (onWall()) 
        return false;

    // Detección normal de suelo
    RaycastHit2D raycastHit = Physics2D.BoxCast(
        boxCollider.bounds.center, 
        boxCollider.bounds.size, 
        0, 
        Vector2.down, 
        0.1f, 
        groundLayer
    );

    return raycastHit.collider != null;
}

    
    private bool onWall()
{
    // Definir la dirección basada en la orientación del personaje
    Vector2 direction = new Vector2(Mathf.Sign(transform.localScale.x), 0);

    // Disparar el BoxCast en la dirección de la pared
    RaycastHit2D raycastHit = Physics2D.BoxCast(
        boxCollider.bounds.center, 
        boxCollider.bounds.size * 0.9f, // Reducimos un poco el tamaño para evitar colisiones erróneas
        0, 
        direction, 
        0.1f, 
        groundLayer // Usamos el layer porque las paredes están en Ground
    );

    // Depuración visual (muestra la línea en la escena para verificar si el rayo está bien dirigido)
    Debug.DrawRay(boxCollider.bounds.center, direction * 0.1f, Color.red);

    // Verificar si el objeto impactado tiene el tag "Wall"
    return raycastHit.collider != null && raycastHit.collider.CompareTag("Wall");
}
 
    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Health"))
        {
            healthBar.Heal(0.5f);
        }

    if (invulnerable || isDead) return;

    if (collision.gameObject.CompareTag("Obstaculo") || collision.gameObject.CompareTag("enemigo"))
    {
        healthBar.Hurt(dañoObstaculo);

        if (playerStats.health > 0)
        {
            anim.SetTrigger("hurt");
            StartCoroutine(InvulnerabilityTimer());
        }
        else
        {
            isDead = true; // Marca al personaje como muerto
            body.velocity = Vector2.zero; // Detiene el movimiento inmediatamente
            body.constraints = RigidbodyConstraints2D.FreezeAll; // Congela posición y rotación completamente
            anim.SetTrigger("die");
            StartCoroutine(RestartLevelAfterAnimation());
        }

    }
    }


    IEnumerator InvulnerabilityTimer()
    {
        invulnerable = true;
        yield return new WaitForSeconds(1.5f);
        invulnerable = false;
    }

    IEnumerator RestartLevelAfterAnimation()
    {
        yield return new WaitForSeconds(0.2f);
        float animDuration = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animDuration);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
