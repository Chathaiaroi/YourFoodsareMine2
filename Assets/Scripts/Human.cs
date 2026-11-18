
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Human : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Player player;

    [Header("Vision (FOV)")]
    [SerializeField] private float viewDistance = 8f;
    [SerializeField, Range(10f, 360f)] private float viewAngle = 90f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float catchDistance = 1.2f;
    [SerializeField] private float loseSightTime = 2f;

    [Header("ReturnHome")]
    [SerializeField] private float returnSpeed = 2f;
    [SerializeField] private float arriveDistance = 0.2f;

    private HumanState state;
    private Renderer rend;


    private Vector3 homePosition;
    private Quaternion homeRotation;
    public float LoseSightTime => loseSightTime;

    public bool IsCloseToCat
    {
        get
        {
            if (player == null || !player.enabled) return false;
            Vector3 flat = player.transform.position - transform.position;
            flat.y = 0f;
            return flat.magnitude <= catchDistance;
        }
    }

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        homePosition = transform.position;
        homeRotation = transform.rotation;
        if (player == null)
        {
            Debug.LogWarning("Human: ยังไม่ได้ลาก Cat ใส่ช่อง Cat ใน Inspector");
        }
        ChangeState(new IdleState());
    }

    void Update()
    {
        Vector3 flat = player.transform.position - transform.position; flat.y = 0;

        if (state != null)
        {
            state.Update(this);
        }
    }

    public void ChangeState(HumanState next)
    {
        state = next;
        state.Enter(this);
    }

    public bool Detect()
    {
        if (player == null || !player.enabled || !player.IsVisible) return false;

        Vector3 flat = player.transform.position - transform.position;
        flat.y = 0f;
        if (flat.magnitude > viewDistance) return false;
        if (Vector3.Angle(transform.forward, flat) > viewAngle * 0.5f) return false;

        Vector3 eye = transform.position + Vector3.up * 0.5f;
        Vector3 toCat = player.transform.position - eye;
        if (Physics.Raycast(eye, toCat.normalized, toCat.magnitude, obstacleMask)) return false;

        return true;
    }

    public void Chase()
    {
        Vector3 dir = player.transform.position - transform.position;
        dir.y = 0f;
        if (dir == Vector3.zero) return;

        Quaternion look = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
        transform.position += dir.normalized * chaseSpeed * Time.deltaTime;
    }

    public bool MoveToHome()
    {

        Vector3 dir = homePosition - transform.position;
        dir.y = 0f;

        if (dir.magnitude <= arriveDistance)
        {
            Debug.Log($"Arrived. home= {homePosition} actual{transform.position}");

            transform.position = new Vector3(homePosition.x, transform.position.y, homePosition.z);
            transform.rotation = homeRotation;   // หันกลับทิศเดิม
            return true;
        }

        Quaternion look = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
        transform.position += dir.normalized * returnSpeed * Time.deltaTime;
        return false;
    }

    public void CatchCat()
    {
        player.Caught();
    }

    // ไว้ดูสถานะด้วยตา: เขียว = Idle, แดง = Chase
    public void SetColor(Color color)
    {
        if (rend != null)
        {
            rend.material.color = color;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 left = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle * 0.5f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, left * viewDistance);
        Gizmos.DrawRay(transform.position, right * viewDistance);
        Gizmos.DrawWireSphere(transform.position, catchDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Application.isPlaying ? homePosition : transform.position, 0.3f);
    }
}
