using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Swing Settings")]
    public float swingForce = 10f;
    public float maxSwingDistance = 10f;
    public LayerMask grappableLayer;
    
    [Header("Collision Settings")]
    public float bounceForce = 5f;
    public LayerMask collisionLayers;
    
    [Header("Visual Effects")]
    public ParticleSystem grappleParticles;
    public ParticleSystem trailParticles;
    public float minSpeedForTrail = 5f;
    
    private LineRenderer ropeRenderer;
    private SpringJoint2D ropeJoint; // Changed to SpringJoint2D for better physics
    private Rigidbody2D rb;
    private bool isSwinging = false;
    private Vector2 grapplePoint;
    private Transform attachedPlatform;
    private Vector2 grappleLocalPoint;
    private ContactFilter2D contactFilter;
    private ContactPoint2D[] contactPoints = new ContactPoint2D[10];

    public int Health;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ropeRenderer = GetComponent<LineRenderer>();
        
        // Replace DistanceJoint2D with SpringJoint2D
        ropeJoint = GetComponent<SpringJoint2D>();
        if (ropeJoint == null)
        {
            ropeJoint = gameObject.AddComponent<SpringJoint2D>();
        }
        
        // Setup rope joint for better physics
        ropeJoint.enabled = false;
        ropeJoint.autoConfigureDistance = false;
        ropeJoint.frequency = 1.5f; // Adjust this for rope stiffness
        ropeJoint.dampingRatio = 0.8f; // Adjust this for rope bounce
        
        // Setup rope renderer
        ropeRenderer.enabled = false;
        
        // Setup rigidbody for better collisions
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.mass = 1f;
        rb.gravityScale = 2f;
        
        // Setup collision detection
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
        
        // Initialize particle systems
        if (grappleParticles) grappleParticles.Stop();
        if (trailParticles) trailParticles.Stop();

        Health = 5;
        GameManager.Instance.UpdateHealth(Health);
    }
    
    void FixedUpdate()
    {
        // Check for collisions
        int contactCount = rb.GetContacts(contactFilter, contactPoints);
        for (int i = 0; i < contactCount; i++)
        {
            if (contactPoints[i].collider.CompareTag("Obstacle"))
            {
                GameManager.Instance.GameOver();
                return;
            }
            if (contactPoints[i].collider.CompareTag("Projectile"))
            {
                Health--;
                GameManager.Instance.UpdateHealth(Health);
                if (Health <= 0)
                {
                GameManager.Instance.GameOver();
                return;
                }
                else
                {
                    return;
                }
            }
            
        }
        if(transform.position.y < -30f){
            GameManager.Instance.GameOver();
            return;
        }
        
        if (isSwinging)
        {
            UpdateSwingPhysics();
        }
    }
    
    void Update()
    {
        // Mouse input for swinging
        if (Input.GetMouseButtonDown(0))
        {
            StartSwing();
        }
        if (Input.GetMouseButtonUp(0))
        {
            EndSwing();
        }
        
        // Update rope visual
        if (isSwinging)
        {
            UpdateRopeVisual();
        }
        
        // Update trail particles based on speed
        UpdateTrailParticles();
    }
    
    void UpdateTrailParticles()
    {
        if (trailParticles != null)
        {
            if (rb.velocity.magnitude > minSpeedForTrail)
            {
                if (!trailParticles.isPlaying)
                {
                    trailParticles.Play();
                }
                var main = trailParticles.main;
                main.startSpeed = rb.velocity.magnitude * 0.1f;
                trailParticles.transform.rotation = Quaternion.LookRotation(Vector3.forward, rb.velocity);
            }
            else if (trailParticles.isPlaying)
            {
                trailParticles.Stop();
            }
        }
    }
    
    void StartSwing()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, mousePos - (Vector2)transform.position, 
            maxSwingDistance, grappableLayer);
            
        if (hit.collider != null)
        {
            isSwinging = true;
            attachedPlatform = hit.transform;
            
            // Store both world and local grapple points
            grapplePoint = hit.point;
            grappleLocalPoint = attachedPlatform.InverseTransformPoint(grapplePoint);
            
            // Setup rope joint
            ropeJoint.connectedAnchor = grapplePoint;
            ropeJoint.distance = Vector2.Distance(transform.position, grapplePoint) * 0.8f;
            ropeJoint.enabled = true;
            ropeRenderer.enabled = true;
            
            // Play grapple particle effect
            if (grappleParticles != null)
            {
                grappleParticles.transform.position = grapplePoint;
                grappleParticles.Play();
            }
            
            // Add initial swing force
            Vector2 swingDirection = (grapplePoint - (Vector2)transform.position).normalized;
            rb.AddForce(swingDirection * swingForce, ForceMode2D.Impulse);
        }
    }
    
    void EndSwing()
    {
        isSwinging = false;
        ropeJoint.enabled = false;
        ropeRenderer.enabled = false;
        attachedPlatform = null;
        
        // Add final swing momentum
        if (rb.velocity.magnitude < swingForce)
        {
            rb.AddForce(rb.velocity.normalized * swingForce, ForceMode2D.Impulse);
        }
    }
    
    void UpdateSwingPhysics()
    {
        if (attachedPlatform != null)
        {
            // Update grapple point position based on platform movement
            grapplePoint = attachedPlatform.TransformPoint(grappleLocalPoint);
            ropeJoint.connectedAnchor = grapplePoint;
        }
    }
    
    void UpdateRopeVisual()
    {
        ropeRenderer.SetPosition(0, transform.position);
        ropeRenderer.SetPosition(1, grapplePoint);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Double-check obstacle collision
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            GameManager.Instance.GameOver();
            return;
        }
        
        // Handle bouncing off other surfaces
        if ((collisionLayers.value & (1 << collision.gameObject.layer)) != 0)
        {
            Vector2 normal = collision.contacts[0].normal;
            float dotProduct = Vector2.Dot(rb.velocity, normal);
            
            if (dotProduct < 0)
            {
                Vector2 bounceDirection = Vector2.Reflect(rb.velocity.normalized, normal);
                rb.velocity = bounceDirection * Mathf.Max(bounceForce, rb.velocity.magnitude * 0.8f);
            }
        }
    }
}