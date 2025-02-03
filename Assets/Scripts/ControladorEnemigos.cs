using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorEnemigos : MonoBehaviour
{
    public Transform player;
    public float detectionRadius;
    public float speed;

    private Rigidbody2D rb;
    private Vector2 movement;
    
    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody2D>();   
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer < detectionRadius)
            {
                Vector2 direction = (player.position - transform.position).normalized;

                movement = new Vector2(direction.x, 0);
            }
            
            else
            {
                movement = Vector2.zero;
            }
            //Debug.Log("ola");

            if (transform.position == player.position)
            {
                movement = new Vector2(0, 0);

            }
        }
        
        if(rb != null)
        {
            rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
