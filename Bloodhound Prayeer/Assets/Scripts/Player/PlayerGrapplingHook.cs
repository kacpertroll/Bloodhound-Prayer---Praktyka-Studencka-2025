using UnityEngine;

public class PlayerGrapplingHook : MonoBehaviour
{
    [Header("Grappling Settings")]
    public float maxGrappleDistance = 20f;
    public float grappleSpeed = 15f;
    public LayerMask grappleLayer;
    [Space]
    [Header("References")]
    public Transform grappleOrigin;
    public LineRenderer lineRenderer;
    private CharacterController controller;

    private Vector3 grapplePoint;
    private bool isGrappling = false;
    private Vector3 grappleMomentum;
    private bool isGrappleDashing = false;
    private float grappleDashTimer = 0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Debug.Log("Is Grounded?: " + controller.isGrounded);

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StartGrapple();
        }
        if (isGrappling)
        {
            ContinueGrapple();
        }
        if (Input.GetKeyUp(KeyCode.Mouse1) && isGrappling)
        {
            StopGrapple();
        }

        if (isGrappleDashing && !controller.isGrounded)
        {
            controller.Move(grappleMomentum * Time.deltaTime);
            grappleDashTimer -= Time.deltaTime;
        }

        if (controller.isGrounded || grappleDashTimer <= 0f)
        {
            grappleMomentum = Vector3.zero;
            isGrappleDashing = false;
        }

        if (controller.collisionFlags != CollisionFlags.None)
        {
            grappleMomentum = Vector3.zero;
            isGrappleDashing = false;
        }
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(grappleOrigin.position, grappleOrigin.forward, out hit, maxGrappleDistance, grappleLayer))
        {
            grapplePoint = hit.point;
            isGrappling = true;
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, grappleOrigin.position);
            lineRenderer.SetPosition(1, grapplePoint);
            grappleMomentum = Vector3.zero;
        }
    }

    void ContinueGrapple()
    {
        if (!isGrappling) return; // Fajny sposób na oszczêdzanie zasobów

        Vector3 direction = (grapplePoint - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, grapplePoint);

        Vector3 grappleMovement = direction * grappleSpeed * Time.deltaTime;
        Vector3 inputDirection = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");  // Mo¿na siê poruszaæ w powietrzu
        Vector3 airControlMovement = inputDirection * grappleSpeed * 0.5f * Time.deltaTime;

        Vector3 finalMovement = grappleMovement + airControlMovement;
        controller.Move(finalMovement);

        lineRenderer.SetPosition(0, grappleOrigin.position);
        grappleMomentum = (grappleMovement + airControlMovement) * 0.8f;

        if (distance < 5f) // Je¿eli znajdziemy siê za blisko obiektu do którego siê przyci¹gami, to grapple siê koñczy
        {
            StopGrapple();
        }
    }

    void StopGrapple()
    {
        isGrappling = false;
        lineRenderer.enabled = false;

        isGrappleDashing = true;
        grappleDashTimer = 3f; // Ustawienie timera dla trwania momentum grappla
        grappleMomentum = (grapplePoint - transform.position).normalized * grappleSpeed; // Ustawienie momentum
    }
}