using System.Collections;
using UnityEngine;

public class EnemigoSlime : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private Transform player; // Asignar manualmente en el Inspector
    [SerializeField] private float jumpHeight = 4f;   // Altura del salto
    [SerializeField] private float jumpSpeed = 3f;   // Velocidad horizontal del salto
    [SerializeField] private float jumpInterval = 2f; // Tiempo entre saltos

    [Header("Vida")]
    [SerializeField] private int health = 3; // Vida del slime

    [Header("Colisión con el suelo")]
    [SerializeField] private LayerMask groundLayer; // Capa que representa el suelo

    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private bool isGrounded = true;
    private bool isDead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Inicia el ciclo de saltos
        StartCoroutine(JumpRoutine());
    }

    private void Update()
    {
        // Verifica si está tocando el suelo usando BoxCast
        isGrounded = IsTouchingGround();
        anim.SetBool("isGrounded", isGrounded);
    }

    IEnumerator JumpRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(jumpInterval);

            if (isGrounded && player != null)
            {
                JumpTowardsPlayer();
            }
        }
    }

    private void JumpTowardsPlayer()
    {
        if (isDead || player == null) return;

        // Calcular dirección hacia el jugador
        Vector2 direction = (player.position - transform.position).normalized;

        // Calcular velocidad necesaria para alcanzar al jugador
        float jumpVelocityX = direction.x * jumpSpeed;
        float jumpVelocityY = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));

        // Aplicar velocidad al Rigidbody2D
        rb.velocity = new Vector2(jumpVelocityX, jumpVelocityY);
        anim.SetTrigger("jump");

        // Voltear el Slime hacia la dirección del jugador
        transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
    }

    private bool IsTouchingGround()
    {
    // Aumenta ligeramente el tamaño del boxcast en X
    Vector2 boxSize = new Vector2(
        boxCollider.bounds.size.x * 1.2f, // Aumenta el ancho en un 20%
        boxCollider.bounds.size.y        // Mantiene la altura
    );

    RaycastHit2D hit = Physics2D.BoxCast(
        boxCollider.bounds.center,
        boxSize,
        0f,
        Vector2.down,
        0.1f,
        groundLayer
    );

    return hit.collider != null;
}


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Fireball")) // Si es golpeado por una Fireball
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        
        if (health <= 0)
        {
            Die();
        } else {
            anim.SetTrigger("hurt");
        }
    }

    private void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero; // Detiene el movimiento
        rb.gravityScale = 0; // Evita que siga cayendo
        GetComponent<Collider2D>().enabled = false; // Desactiva colisión
        anim.SetTrigger("die");
        StartCoroutine(DestroyAfterAnimation());
    }

    IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }
}
