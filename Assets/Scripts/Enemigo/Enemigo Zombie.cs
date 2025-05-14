using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoZombie : MonoBehaviour
{
    public float speed = 2f;
    public int health = 3;
    public string limitTag = "limite";

    private Animator animator;
    private Rigidbody2D rb;
    private bool facingRight = true;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isDead)
        {
            Move();
        }
    }

    void Move()
    {
        rb.velocity = new Vector2(speed * (facingRight ? 1 : -1), rb.velocity.y);
        animator.SetBool("Caminar", true);
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void TakeDamage()
    {
        if (isDead) return;
        health--;

        if (health <= 0)
        {
            Die();
        } else {
            animator.SetTrigger("Damage");
        }
    }

    private void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero; // Detiene el movimiento
        rb.gravityScale = 0; // Evita que siga cayendo
        GetComponent<Collider2D>().enabled = false; // Desactiva colisión
        StartCoroutine(DestroyAfterAnimation());
    }

    IEnumerator DestroyAfterAnimation()
{
    animator.SetTrigger("Die");

    // Esperar un frame para que la animación cambie
    yield return null;

    // Obtener la duración correcta de la animación actual
    float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;

    // Esperar la duración completa de la animación antes de destruir el objeto
    yield return new WaitForSeconds(animationLength-0.35f);

    Destroy(gameObject);
}

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(limitTag))
        {
            Destroy(gameObject);
        }

        if (collision.CompareTag("Fireball"))
        {
            TakeDamage();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        Vector2 contactPoint = collision.GetContact(0).point;
        Vector2 zombiePosition = transform.position;
        
        if ((contactPoint.x > zombiePosition.x && facingRight) || (contactPoint.x < zombiePosition.x && !facingRight))
        {
            Flip();
        }
    }
}
