using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrapple : MonoBehaviour
{
    [SerializeField]
    private InputActionReference grapple, pointerPos, movement;
    private LineRenderer lineRenderer;
    Rigidbody2D rb;

    private float maxGrappleDistance = 6;
    private float initialGrappleSpeed = 1.5f;
    private float grappleAcceleration = 1.5f;
    private int grappleFrames = 120;
    private bool grappling = false;
    private Vector2 grappleLocation;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if(grappling)
        {
            // animate the grapple hook while actively grappling
            Vector3 start = new Vector3(gameObject.transform.position.x,gameObject.transform.position.y,0);

            //lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, grappleLocation);

            // Stop grappling when player reaches target
            if(Vector2.Distance(rb.position, grappleLocation) <= 0.5)
            {
                grappling = false;
            }
        }
    }

     private void OnEnable()
    {
        grapple.action.performed += StartGrapple;
    }

    private void StartGrapple(InputAction.CallbackContext context)
    {
        Vector2 grappleDirection = GetGrappleDirection();

        // project settings -> physics2D -> quries start in colliders unchecked so raycast does not detect origin
        RaycastHit2D hitTarget = Physics2D.Raycast(gameObject.transform.position, grappleDirection, distance: maxGrappleDistance);

        if(hitTarget)
        {
            grappleLocation = hitTarget.point;
            StartCoroutine(PerformGrapple());
        }
        else 
        {
            grappleLocation = rb.position + grappleDirection.normalized * maxGrappleDistance;
            StartCoroutine(PerformMissedGrapple());
        }

    }

    private Vector2 GetGrappleDirection()
    {
        Vector2 mousePos = pointerPos.action.ReadValue<Vector2>();
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 direction = mousePos - rb.position;

        return direction.normalized;
    }

    IEnumerator PerformGrapple()
    {
        yield return StartCoroutine(AnimateGrappleShot());

        movement.action.Disable();
        grapple.action.Disable();
        grappling = true;
        
        rb.velocity = (grappleLocation - rb.position).normalized * initialGrappleSpeed;
        rb.drag = 0;

        while (grappling)
        {
            // Temp fix for bug where movement does not get disabled if a move input is pressed while being disabled
            if(movement.action.enabled)
            {
                movement.action.Disable();
                rb.velocity = (grappleLocation - rb.position).normalized * initialGrappleSpeed;
            }

            rb.velocity = rb.velocity.magnitude * grappleAcceleration * rb.velocity.normalized;
            yield return new WaitForSeconds(0.1f);
        }

        rb.velocity = Vector2.zero;
        rb.drag = 3;
        movement.action.Enable();
        grapple.action.Enable();
        lineRenderer.enabled = false;
    }

    // Shows where the grapple hook went
    IEnumerator PerformMissedGrapple()
    {
        yield return StartCoroutine(AnimateGrappleShot());

        grappling = true;
        grapple.action.Disable();
        
        yield return new WaitForSeconds(0.5f);

        grappling = false;
        lineRenderer.enabled = false;
        grapple.action.Enable();
    }

    IEnumerator AnimateGrappleShot()
    {
        lineRenderer.enabled = true;
        float grappleAnimationFramesElapsed = 0;
        float interpolationRatio;

        do
        {
            Vector3 start = new Vector3(gameObject.transform.position.x,gameObject.transform.position.y,0);

            interpolationRatio = grappleAnimationFramesElapsed / grappleFrames;

            Vector3 interpolatedPos = Vector3.Lerp(start, grappleLocation, interpolationRatio);

            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, interpolatedPos);

            grappleAnimationFramesElapsed = (grappleAnimationFramesElapsed + 1) % (grappleFrames + 1);  // reset elapsedFrames to zero after it reached (interpolationFramesCount + 1)
            yield return null; // wait for the end of frame
        }
        while (interpolationRatio < 1);
        
    }
}
