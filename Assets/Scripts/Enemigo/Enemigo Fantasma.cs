using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEnemy : MonoBehaviour
{
    public Transform player; // Referencia al jugador
    public float speed = 2.5f; // Velocidad del fantasma
    public float detectionRange = 3f; // Distancia para cambiar animación
    public int damage = 1; // Daño que causa al jugador
    private Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player != null)
        {
            // Mover el fantasma hacia el jugador
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            
            // Determinar si el jugador está cerca para cambiar la animación
            float distance = Vector2.Distance(transform.position, player.position);
            animator.SetBool("isNear", distance <= detectionRange);

            // Voltear el sprite según la dirección del jugador
            if (player.position.x < transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}

