using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniLanza : MonoBehaviour
{
    public float speed = 10f;
    private bool lanzada = false;

    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.Play("AparecerMiniLanza");
    }

    // Llamado al final de la animación "AparecerMiniLanza" como evento de animación
    public void Lanzar()
    {
        lanzada = true;
        rb.velocity = Vector2.down * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (lanzada && !collision.collider.CompareTag("Boss")) // evita colisiones con el jefe si es necesario
        {
            rb.velocity = Vector2.zero;
            lanzada = false;
            animator.Play("DesaparecerMiniLanza");
        }
    }


    // Llamado al final de la animación "DesaparecerMiniLanza" como evento
    public void Destruir()
    {
        Destroy(gameObject);
    }
}
