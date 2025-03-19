using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GrapplingHook : MonoBehaviour
{
    [Header("Hook Settings")]
    public float maxGrappleDistance = 25f;
    public float grappleSpeed = 15f;
    public LayerMask grappleLayerMask;

    [Header("References")]
    public Transform cameraTransform;
    public Transform playerBody;

    private LineRenderer lineRenderer;
    private Vector3 grapplePoint;
    private bool isGrappling = false;

    private CharacterController characterController;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        characterController = playerBody.GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // PPM do zaczepienia
        {
            StartGrapple();
        }

        if (Input.GetMouseButtonUp(1)) // Puszczenie PPM = anulowanie
        {
            StopGrapple();
        }

        if (isGrappling)
        {
            GrappleMovement();
            DrawRope();
        }
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, maxGrappleDistance, grappleLayerMask))
        {
            grapplePoint = hit.point;
            isGrappling = true;

            // Start liny
            lineRenderer.positionCount = 2;
        }
    }

    void GrappleMovement()
    {
        // Oblicz kierunek do punktu zaczepienia
        Vector3 direction = (grapplePoint - playerBody.position).normalized;
        float distance = Vector3.Distance(playerBody.position, grapplePoint);

        // Ruch gracza w stronê punktu
        Vector3 move = direction * grappleSpeed * Time.deltaTime;

        // Zatrzymanie gdy jesteœmy bardzo blisko punktu
        if (distance < 2f)
        {
            StopGrapple();
        }
        else
        {
            characterController.Move(move);
        }
    }

    void StopGrapple()
    {
        isGrappling = false;
        lineRenderer.positionCount = 0;
    }

    void DrawRope()
    {
        lineRenderer.SetPosition(0, cameraTransform.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }
}
