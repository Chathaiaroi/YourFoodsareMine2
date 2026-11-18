using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;

using UnityEngine.TextCore.Text;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float hideSpeed = 1.5f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float gravity = -19.62f;

    [Header("Stamina")]
    [SerializeField] private StaminaSystem stamina = new StaminaSystem();

    [Header("Interact")]
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;

    public StaminaSystem Stamina => stamina;

    public bool IsHiding { get; private set; }
    public bool IsVisible => !IsHiding;   

    public event Action<float, float> OnStaminaChanged;
    public event Action OnCaught;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        //animator = GetComponent<Animator>();
        stamina.Init();
    }

    void Update()
    {
        ApplyGravity();

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        IsHiding = Input.GetKey(KeyCode.H);
        bool wantsRun = Input.GetKey(KeyCode.LeftShift)
                        && moveDirection != Vector3.zero
                        && !IsHiding && stamina.CanRun;

       stamina.Tick(wantsRun,Time.deltaTime);

        float speed = IsHiding ? hideSpeed : (wantsRun ? runSpeed : walkSpeed);
        Movement(moveDirection, speed);
        Turn(moveDirection);

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interaction();
        }
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void Movement(Vector3 moveDirection, float speed)
    {
        controller.Move(moveDirection * speed * Time.deltaTime);
        if(animator != null) 
        {
            animator.SetFloat("MoveSpeed", moveDirection.magnitude * speed);
            animator.SetBool("isHiding", IsHiding);
        }
    }

    public void Turn(Vector3 moveDirection)
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void Interaction()
    {
        //Vector3 origin = transform.position;
        //Debug.DrawRay(origin, transform.forward * interactRange, Color.red, 2f);

        //RaycastHit hit;
        //// ยิงโดยไม่กรอง Layer เพื่อดูว่าโดนอะไรบ้าง
        //if (Physics.Raycast(origin, transform.forward, out hit, interactRange))
        //{
        //    Debug.Log($"Hit: {hit.collider.name} / Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
        //}
        //else
        //{
        //    Debug.Log("Hit nothing at all");
        //}


        Debug.Log("E pressed");
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.4f, transform.forward, out hit, interactRange, interactableLayer))
        {
            Debug.Log("Ray hit: " + hit.collider.name);
            FoodItem food = hit.collider.GetComponent<FoodItem>();
            Debug.Log("Food found" + (food != null));
            if (food != null)
            {
                food.Eat(this);
            }
        }
        else
        {
            Debug.Log("Ray hit nothing");
        }
    }
    public void Caught()
    {
        Debug.Log("Cat was caught!");
        enabled = false;
        if (animator != null) animator.SetBool("isCaught", true);
        OnCaught?.Invoke();
    }

    public void RespawnAt(Vector3 pos)
    {
        controller.enabled = false;
        transform.position = pos;
        transform.rotation = Quaternion.identity;
        if (animator != null)  animator.SetBool("isCaught", false);
        controller.enabled = true;

        stamina.Init();
        enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * interactRange);
    }
}

