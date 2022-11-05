using System;
using UnityEngine;

public class JettSmokeProjectile : MonoBehaviour
{
    [SerializeField] private GameObject smokeBallPrefab;

    public bool isControlled;

    private Vector3 startingPosition;
    private float particleMovementSpeed = 20.0f;
    private float maxDistance = 70.0f;
    private float distanceTraveled = 0f;
    private Camera playerCamera;

    private float downwardForce = -2.0f;
    private float downwardForceIncrement = -3.0f;
    private 
    
    void Start()
    {
        startingPosition = transform.position;
    }

 
    void Update()
    {
        if (isControlled) { transform.rotation = playerCamera.transform.rotation; } // Smoke direction change according to camera angle

        Vector3 movementVector = (transform.forward * particleMovementSpeed * Time.deltaTime); // Movement of smoke
        if (!isControlled) // Smoke falling down
        {
            downwardForce += downwardForceIncrement * Time.deltaTime;
            movementVector += (transform.up * downwardForce * Time.deltaTime);
        }

        Vector3 newPosition = transform.position + movementVector;
        distanceTraveled = Vector3.Distance(startingPosition, newPosition);
        
        if(distanceTraveled > maxDistance){ OnCreateSmokeBall(transform.position);} // If we hit max distance smoke will be created at this place
        else {transform.position += movementVector;} // Else continue to move
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCreateSmokeBall(collision.contacts[0].point);
    }


    /*
     Wherever the character is looking 
     (that is, wherever the character camera is facing), the smoke will move in that direction.
     */
    public void InitializeValues(bool _isControlled,Camera _playerCamera) 
    {
        isControlled = _isControlled;
        playerCamera = _playerCamera;
    }

    public void SetIsControlled(bool _isControlled)
    {
        isControlled = _isControlled;
    }

    void OnCreateSmokeBall(Vector3 position)
    {
        Instantiate(smokeBallPrefab, position, transform.rotation); // Instantiate the bigger ball
        Destroy(this.gameObject); // Destroy smaller smoke ball (JettSmokeProjectile prefab)
    }
}
