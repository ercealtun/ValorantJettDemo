using UnityEngine;

public class JettController : MonoBehaviour
{
    public bool isDashing;

    private int dashAttempts;
    private float dashStartTime;

    [SerializeField] private ParticleSystem forwardDashParticleSystem;
    [SerializeField] private ParticleSystem backwardDashParticleSystem;
    [SerializeField] private ParticleSystem rightDashParticleSystem;
    [SerializeField] private ParticleSystem leftDashParticleSystem;

    private PlayerController playerController;
    private CharacterController characterController;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleDash();
    }

    void HandleDash()
    {
        bool isTryingToDash = Input.GetKeyDown(KeyCode.E);

        if (isTryingToDash && !isDashing)
        {
            if (dashAttempts <= 50)
            {
                OnStartDash();
            }
        }

        if (isDashing)
        {
            if (Time.time - dashStartTime <= 0.4f)
            {
                if (playerController.movementVector.Equals(Vector3.zero)) // It means user is not inputting anyting like backward, left, right dashing. -- Just forward dashing
                {
                    characterController.Move(transform.forward * 30f * Time.deltaTime);
                }
                else // Inputting with E + ( A OR S OR D ) -- left, backward and right dashing
                {
                    characterController.Move(playerController.movementVector.normalized * 30f * Time.deltaTime);
                }
            }
            else
            {
                OnEndDash();
            }
        }
    }

    void OnStartDash()
    {
        isDashing = true;
        dashStartTime = Time.time;
        dashAttempts += 1;
        
        PlayDashParticles();
    }

    void OnEndDash()
    {
        isDashing = false;
        dashStartTime = 0;
    }

    void PlayDashParticles()
    {
        Vector3 inputVector = playerController.inputVector;

        if (inputVector.z > 0 && Mathf.Abs(inputVector.x) <= inputVector.z)
        {
             forwardDashParticleSystem.Play();
             return;
        }

        if (inputVector.z < 0 && Mathf.Abs(inputVector.x) <= Mathf.Abs(inputVector.z))
        {
            backwardDashParticleSystem.Play();
            return;
        }

        if (inputVector.x > 0)
        {
            rightDashParticleSystem.Play();
            return;
        }
        
        if (inputVector.x < 0)
        {
            leftDashParticleSystem.Play();
            return;
        }
        
        forwardDashParticleSystem.Play(); // Default
    }
}
