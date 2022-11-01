using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public Gun equippedWeapon;
    public bool isShootingDisabled = true;

    [SerializeField] Animator handAnimator;
    [SerializeField] Transform firePoint;
    [SerializeField] Camera playerCamera;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject armsContainer;
    [SerializeField] GameObject gunContainer;
    [SerializeField] GameObject wallHitDecalPrefab;
    [SerializeField] GameObject projectilePrefab;

    private PlayerController playerController;
    private PlayerStats playerStats;

    private float lastTimeShot = 0f;
    private int currentRecoilIndex = 0;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerStats = GetComponent<PlayerStats>();
        equippedWeapon = new AK47();
    }

    void Update()
    {
        bool isTryingToShoot = Input.GetKey(KeyCode.Mouse0);

        if (isTryingToShoot)
        {
            HandleShooting();
        }
        else // When the player releases the trigger, the gun spray will stop and the gun will return to its original position
        {
            playerController.SetGunRotation(
                Vector3.Lerp(
                    playerController.gunRotation,
                    Vector3.zero, 
                    equippedWeapon.fireRatePerSecond * Time.deltaTime
                )
            );
        }
    }

    void HandleShooting()
    {
        PlayFireAnimation();
        
        if (Time.time - lastTimeShot >= 1 / equippedWeapon.fireRatePerSecond) // If true player(user) can shoot again
        {
            HandleGunRecoil();
            
            RaycastHit hit;
            if (Physics.Raycast(
                firePoint.transform.position,
                firePoint.transform.TransformDirection(Vector3.forward),
                out hit,
                equippedWeapon.fallOffDistance)) // If true player(user) successfully hit something
            {
                Instantiate(wallHitDecalPrefab, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal)); // Wall bullet tracks
            }

            lastTimeShot = Time.time;
        }
    }

    void HandleGunRecoil()
    {
        if (Time.time - lastTimeShot >= equippedWeapon.recoilResetTimeSeconds)
        {
            playerController.SetGunRotation(playerController.gunRotation + equippedWeapon.recoilPattern[0]); // First bullet so the gun has no ricochet
            currentRecoilIndex = 1;
        }
        else
        {
            playerController.SetGunRotation(playerController.gunRotation +
                                            equippedWeapon.recoilPattern[currentRecoilIndex]);

            if (currentRecoilIndex + 1 <= equippedWeapon.recoilPattern.Length - 1) // If it needs to be reloaded or not
            {
                currentRecoilIndex += 1;
            }
            else // All bullets in magazine been used, reloading the magazine
            {
                currentRecoilIndex = 0;
            }
        }
    }

    void PlayFireAnimation()
    {
        handAnimator.Play("Fire");
    }
}