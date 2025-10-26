using UnityEngine;

public class HelicopterPatrol : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float flyDuration = 5f;
    public float rotateDuration = 2f;

    [Header("Player Interaction")]
    public Transform player;
    public float triggerDistance = 100f;
    public float approachSpeed = 20f;
    public float hoverHeight = 30f;
    public float hoverRadius = 40f;
    public float hoverDuration = 30f;
    public float hoverRotateSpeed = 30f;

    [Header("Missile Settings")]
    public GameObject missilePrefab;
    public Transform missileDropPoint;

    private Vector3 startPosition;
    private Quaternion targetRotation;
    private bool flyingForward = true;
    private bool rotating = false;
    private float timer = 0f;
    private float rotateTimer = 0f;
    private bool approachingPlayer = false;
    private bool hovering = false;
    private float hoverTimer = 0f;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (Input.GetKeyDown(KeyCode.K) && distanceToPlayer <= triggerDistance)
        {
            approachingPlayer = true;
            hovering = false;
        }

        // --- Thả tên lửa ---
        if (Input.GetKeyDown(KeyCode.L) && distanceToPlayer <= triggerDistance)
        {
            DropMissile();
        }

        if (hovering)
        {
            HoverAroundPlayer();
            return;
        }

        if (approachingPlayer)
        {
            ApproachPlayer();
            return;
        }

        PatrolMovement();
    }

    private void PatrolMovement()
    {
        if (rotating)
        {
            rotateTimer += Time.deltaTime;
            float t = rotateTimer / rotateDuration;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);

            if (t >= 1f)
            {
                rotating = false;
                rotateTimer = 0f;
            }
            return;
        }

        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        timer += Time.deltaTime;

        if (timer >= flyDuration)
        {
            timer = 0f;
            flyingForward = !flyingForward;
            RotateHelicopter();
        }
    }

    private void RotateHelicopter()
    {
        rotating = true;
        targetRotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);
    }

    private void ApproachPlayer()
    {
        Vector3 targetPos = player.position + Vector3.up * hoverHeight;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, approachSpeed * Time.deltaTime);

        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0;
        if (lookPos != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * 2f);

        float distance = Vector3.Distance(transform.position, targetPos);
        if (distance < 5f)
        {
            approachingPlayer = false;
            hovering = true;
            hoverTimer = 0f;
        }
    }

    private void HoverAroundPlayer()
    {
        hoverTimer += Time.deltaTime;

        transform.RotateAround(player.position, Vector3.up, hoverRotateSpeed * Time.deltaTime);

        Vector3 desiredPos = transform.position;
        desiredPos.y = player.position.y + hoverHeight;
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * 2f);

        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 2f);

        if (hoverTimer >= hoverDuration)
        {
            hovering = false;
            flyingForward = true;
        }
    }

    private void DropMissile()
    {
        if (missilePrefab == null || missileDropPoint == null)
        {
            Debug.LogWarning("⚠️ Missing missilePrefab or missileDropPoint!");
            return;
        }

        Instantiate(missilePrefab, missileDropPoint.position, missileDropPoint.rotation);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}
