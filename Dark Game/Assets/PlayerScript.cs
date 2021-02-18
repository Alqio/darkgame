using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Collider2D outCollider;
    public Collider2D inCollider;
    public float speed;
    private Rigidbody2D rb;

    private Vector2 move;

    public Radar radar;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        move.x = Input.GetAxisRaw("Horizontal");
        move.y = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(KeyCode.Space)){
            Instantiate(radar, transform.position, transform.rotation);
        }
        if (Input.GetMouseButtonDown(0)) {
             Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
             target.z = transform.position.z;
             Instantiate(radar, target, transform.rotation);
         }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }

}
