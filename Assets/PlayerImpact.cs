using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerImpact : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Shake cameraShake;

    public float ImpactSpeed { get; set; } = 0f;

    
    private float impactActionWindow = 0.5f;
    private float impactActionTimer = 0f;
    private bool actionWindowIsActive = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (actionWindowIsActive)
        {
            impactActionTimer += Time.deltaTime;

            if(impactActionTimer >= impactActionWindow)
            {
                RemoveImpactSpeed();
                actionWindowIsActive = false;
            }
        }
        
    }

    // stop player's velecoity on impact but allows them to move through enemies still
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            StopCoroutine(cameraShake.Shaking());
            StartCoroutine(cameraShake.Shaking());

            rb.velocity = rb.velocity.normalized;
            
            ResetImpactSpeed();
        }
        // walls
        // if(other.gameObject.layer == 6)
        // {
        //     ResetImpactSpeed();

        //     if(!GetComponent<PlayerMovement>().isWallJumping)
        //     {
        //         Debug.Log("collision");
        //         rb.velocity = Vector2.zero;
        //     }
        // }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Debug.Log("collision " + rb.velocity);
        // Debug.Log("other " + other.gameObject);
        //Debug.Log(gameObject.GetComponent<Rigidbody2D>().velocity);
        ResetImpactSpeed();

        if(gameObject.GetComponent<PlayerMovement>().wallJumpQueued)
        {
            Debug.Log("Starting wj");
            gameObject.GetComponent<PlayerMovement>().StartWallJump();
            gameObject.GetComponent<PlayerMovement>().wallJumpQueued = false;
        }
    }

    void OnCollisionStay2D(Collision2D collisionInfo)
    {
        //Debug.Log("Stay collision " + rb.velocity);
        if(gameObject.GetComponent<PlayerMovement>().wallJumpQueued)
        {
            Debug.Log("Starting wj");
            gameObject.GetComponent<PlayerMovement>().StartWallJump();
            gameObject.GetComponent<PlayerMovement>().wallJumpQueued = false;
        }
    }

    // void OnCollisionExit2D(Collision2D collisionInfo) 
    // {
    //         print("Collision Out: " + gameObject.name);
    // }

    private void ResetImpactSpeed()
    {
        ImpactSpeed += rb.velocity.magnitude;

        // Window to input a dash to accumulate speed based on how fast the player was going at impact
        impactActionTimer = 0f;
        actionWindowIsActive = true;
    }

    private void RemoveImpactSpeed()
    {
        ImpactSpeed = 0;
    }
}
