using UnityEngine;

public class JettController : MonoBehaviour
{
    public bool isDashing;
    public bool isThrowingSmoke = false;

    private int dashAttempts;
    private float dashStartTime;

    [SerializeField] private ParticleSystem forwardDashParticleSystem;
    [SerializeField] private ParticleSystem backwardDashParticleSystem;
    [SerializeField] private ParticleSystem rightDashParticleSystem;
    [SerializeField] private ParticleSystem leftDashParticleSystem;
    [SerializeField] private GameObject smokeProjectile;
    [SerializeField] private Transform smokeFiringTransform;
    [SerializeField] private Camera playerCamera;

    JettSmokeProjectile currentSmokeProjectile;
    private float lastTimeSmokeEnded = 0f;
    private float smokeDelaySeconds = 0.3f;

    private PlayerController playerController;
    private CharacterController characterController;
    private PlayerWeapon playerWeapon;
    
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        playerWeapon = GetComponent<PlayerWeapon>();
    }

    
    void Update()
    {
        HandleDash();
        HandleSmoke();
    }
    
    #region Dashing
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
    #endregion

    #region Smoke

    void HandleSmoke()
    {
        bool isTryingToThrowSmoke = Input.GetKeyDown(KeyCode.C);

        if (isTryingToThrowSmoke && Time.time - lastTimeSmokeEnded >= smokeDelaySeconds)
        {
            ThrowSmoke();
        }

        if (isThrowingSmoke)
        {
            bool isControlled = Input.GetKey(KeyCode.C);
            currentSmokeProjectile.SetIsControlled(isControlled);

            bool isStoppingControl = Input.GetKeyUp(KeyCode.C);
            if (isStoppingControl)
            {
                OnThrowingSmokeEnd();
            }
        }
    }

    void ThrowSmoke()
    {
        isThrowingSmoke = true;

        GameObject _smokeProjectile = Instantiate(smokeProjectile, smokeFiringTransform.position, playerCamera.transform.rotation);
        currentSmokeProjectile = _smokeProjectile.GetComponent<JettSmokeProjectile>();
        currentSmokeProjectile.InitializeValues(false,playerCamera);
    }

    void OnThrowingSmokeEnd()
    {
        lastTimeSmokeEnded = Time.time;
        isThrowingSmoke = false;
        currentSmokeProjectile.SetIsControlled(false);
        currentSmokeProjectile = null;
    }
    
    #endregion
    
}
