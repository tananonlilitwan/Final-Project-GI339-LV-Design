using System.Collections;
using UnityEngine;

public class GhostAI : MonoBehaviour
{
    public Transform player;
    public Transform[] waypoints;
    public float moveSpeed = 3f;
    public float chaseSpeed = 5f;
    public float hideDuration = 5f;
    public float fleeDuration = 5f;
    public float distanceThreshold = 0.5f;

    private int timesShot = 0;
    private float stateTimer = 0f;
    private bool isFleeing = false;
    private bool isHiding = false;

    private Transform currentWaypoint;
    
    [SerializeField] private SkinnedMeshRenderer ghostRenderer;

    void Update()
    {
        if (isHiding)
        {
            Debug.Log("🔵 GHOST STATE: Hiding...");
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                isHiding = false;
                timesShot = 0;
                Debug.Log("🟢 GHOST STATE: Done Hiding, Ready to Chase");
                //gameObject.SetActive(true);
            }
            return;
        }

        if (isFleeing)
        {
            Debug.Log("🟡 GHOST STATE: Fleeing...");
            FleeFromPlayer();

            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                isFleeing = false;
                Debug.Log("🟢 GHOST STATE: Done Fleeing, Ready to Chase");
            }
        }
        else
        {
            Debug.Log("🔴 GHOST STATE: Chasing Player...");
            ChasePlayer();
        }
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * chaseSpeed * Time.deltaTime;
    }

    void FleeFromPlayer()
    {
        if (currentWaypoint == null || Vector3.Distance(transform.position, currentWaypoint.position) < distanceThreshold)
        {
            currentWaypoint = waypoints[Random.Range(0, waypoints.Length)];
        }

        Vector3 direction = (currentWaypoint.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    public void OnHitByBullet()
    {
        timesShot++;
        Debug.Log("💥 GHOST HIT! Total Hits: " + timesShot);

        if (timesShot >= 5)
        {
            Debug.Log("😵 GHOST STATE: Too many hits, hiding...");
            HideGhost();
        }
        else
        {
            Debug.Log("🏃‍♂️ GHOST STATE: Hit but still fleeing...");
            isFleeing = true;
            stateTimer = fleeDuration;
        }
    }

    /*void HideGhost()
    {
        isFleeing = false;
        isHiding = true;
        stateTimer = hideDuration;
        gameObject.SetActive(false); // ซ่อนตัว แต่ไม่ลบ
    }*/
    
    void HideGhost()
    {
        isFleeing = false;
        isHiding = true;
        StartCoroutine(HideRoutine());
    }
    IEnumerator HideRoutine()
    {
        Debug.Log("👻 Hiding Ghost (disabling visuals + collider)");
        
        if (ghostRenderer != null)
            ghostRenderer.enabled = false;

        Collider ghostCollider = GetComponent<Collider>();
        if (ghostCollider != null)
            ghostCollider.enabled = false;

        yield return new WaitForSeconds(hideDuration);
        
        // 🔀 ย้ายตำแหน่ง Ghost ไปยังจุด Waypoint แบบสุ่ม
        currentWaypoint = waypoints[Random.Range(0, waypoints.Length)];
        transform.position = currentWaypoint.position;
        
        Debug.Log("👻 Returning from hide!");

        if (ghostRenderer != null)
            ghostRenderer.enabled = true;

        if (ghostCollider != null)
            ghostCollider.enabled = true;

        isHiding = false;
        timesShot = 0;
    }
}
