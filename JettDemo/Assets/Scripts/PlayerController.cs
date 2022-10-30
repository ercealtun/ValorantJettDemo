using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] Transform groundCheckTransform;
    [SerializeField] LayerMask groundLayer;
    public float mouseSensivity { get; private set; } = 100f;
    public float xRotation { get; private set; } = 0f;
    public bool isGrounded { get; private set; } = false;
    public bool isWalking { get; private set; } = false;
    public bool isJumping { get; private set; } = false;
    public bool isCrouching { get; private set; } = false;
    
    [System.NonSerialized] public Vector3 jumpVelocity = Vector3.zero; // Vector3 is a struct, so I got to define as System.NonSerialized

    private CharacterController characterController;
    private PlayerStats playerStats;
    
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerStats = GetComponent<PlayerStats>();
        Cursor.lockState = CursorLockMode.Locked; // No cursor on gameplay screen
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheckTransform.position, 0.4f, groundLayer); // As long as the game is active (the update function is working), this code sentence looks at whether the player touches the ground or not
        
        HandleJumpInput();
        HandleMovement();
        HandleMouseLook();
    }
    
    void HandleJumpInput()
    {
        bool isTryingToJump = Input.GetKeyDown(KeyCode.Space); // GetKeyDown returns true when user presses space key

        if (isTryingToJump && isGrounded) { isJumping = true; } // If user pressed space key
        else { isJumping = false; }

        if (isGrounded && jumpVelocity.y < 0f) { jumpVelocity.y = -2f; }

        if (isJumping) { jumpVelocity.y = Mathf.Sqrt(playerStats.jumpHeight * -2f * playerStats.gravity); } // Mathematical equation of jumping

        jumpVelocity.y += playerStats.gravity * Time.deltaTime; // Backing to the ground after jumping
        
        characterController.Move(jumpVelocity * Time.deltaTime);
    }

    void HandleMovement()
    {
        float verticalInput = Input.GetAxis("Vertical"); // W - S keys 
        float horizontalInput = Input.GetAxis("Horizontal"); // A - D keys
        isWalking = Input.GetKey(KeyCode.LeftShift); // Using left shift key for silence walk
        isCrouching = Input.GetKey(KeyCode.LeftControl); // Using left ctrl key for crouching

        if (isCrouching) { HandleCrouch(); }
        else { HandleStand(); }
        
        Vector3 movementVector = Vector3.ClampMagnitude(transform.right * horizontalInput + transform.forward * verticalInput, 1.0f);  // Using ClampMagnitude I set max move speed to 1 unit
        
        if  (isCrouching) { characterController.Move(movementVector * playerStats.crouchingMovementSpeed * Time.deltaTime); }
        else if (isWalking) { characterController.Move(movementVector * playerStats.walkingMovementSpeed * Time.deltaTime); }
        else { characterController.Move(movementVector * playerStats.runningMovementSpeed * Time.deltaTime); }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limitation of a player's perspective in FPS mode
        
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleCrouch()
    {
        if (characterController.height > playerStats.crouchHeightY) // Checking if player standing
        {
            UpdateCharacterHeight(playerStats.crouchHeightY); // Crouching
            if (characterController.height - 0.05f <= playerStats.crouchHeightY) { characterController.height = playerStats.crouchHeightY; } 
        }
    }

    void HandleStand()
    {
        if (characterController.height < playerStats.standingHeightY) // Checking if player crouches
        {
            float lastHeight = characterController.height;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.up, out hit, playerStats.standingHeightY)) // If player hits something before it reaches its standing height
            {
                if (hit.distance < playerStats.standingHeightY - playerStats.crouchHeightY) 
                {
                    UpdateCharacterHeight(playerStats.crouchHeightY + hit.distance);
                    return;
                }
            }
            else
            {
                UpdateCharacterHeight(playerStats.standingHeightY); // Standing
            }

            if (characterController.height + 0.05f >= playerStats.standingHeightY) { characterController.height = playerStats.standingHeightY; }

            transform.position += new Vector3(0, (characterController.height - lastHeight) / 2, 0);
        }
    }

    void UpdateCharacterHeight(float newHeight)
    {
        characterController.height = Mathf.Lerp(characterController.height, newHeight, playerStats.crouchSpeed * Time.deltaTime); 
    }
}
