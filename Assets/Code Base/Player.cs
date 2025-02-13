using System;
using UnityEngine;
using Random = UnityEngine.Random;
public enum MoveType
{
    Walk,
    Run,
    Air
}
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CharacterInputController))] 
[RequireComponent(typeof(AudioSource))]
public class Player : SingletonBase<Player>
{
    
    [Header("Character Settings")]
    [Range(1f,10f)][SerializeField] private float maxSpeedWalk = 3f;
    [Range(1f,10f)][SerializeField] private float maxSpeedRun = 6f;
    [SerializeField] private Transform cameraPos;
    [SerializeField] private float stepRateWalk = 0.6f; 
    [SerializeField] private float stepRateRun = 0.6f; 
    [SerializeField] private AudioClip[] stepSounds; 
    public Transform CameraPos => cameraPos;
    private AudioSource audioSource;
    private Rigidbody rb;
    private Vector3 moveVector;
    private float stepTime;

    public bool isMove;
    private void Awake()
    {
        Init();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    
    private void Start()
    {
        ReturnCamera();
    }

    public void ReturnCamera()
    {
        OnePersonCamera.Instance.SetTarget(cameraPos,TypeMoveCamera.WithRotation,false,true);
    }
    private const float Acceleration = 50f;
    private const float AirMoveLimit = 0.400f;
    
    public void Move(Vector3 direction,MoveType moveType)
    {
        if (direction.magnitude > 1) direction /= 2;

        stepTime += Time.deltaTime;

        if (direction == Vector3.zero && moveType != MoveType.Air)
        {
            isMove = false;
            rb.linearVelocity = Vector3.zero;
            return;
        }
        
        rb.AddRelativeForce(direction * (Acceleration),ForceMode.Acceleration);
        
        switch (moveType)
        {
            case MoveType.Walk:
            {
                isMove = true;
                if (rb.linearVelocity.magnitude >= maxSpeedWalk)
                {
                    StepsPlay(stepRateWalk);
                    rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeedWalk);
                }

                break;
            }
            
            case MoveType.Run:
            {
                isMove = true;
                if (rb.linearVelocity.magnitude >= maxSpeedRun)
                {
                    StepsPlay(stepRateRun);
                    rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeedRun);
                }

                break;
            }
    
            case MoveType.Air:
                rb.linearVelocity = new Vector3(Mathf.Clamp(rb.linearVelocity.x,0,AirMoveLimit), rb.linearVelocity.y, Mathf.Clamp(rb.linearVelocity.z,0,AirMoveLimit));
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(moveType), moveType, null);
        }
    }

    private void StepsPlay(float stepLenght)
    {
        if (stepSounds.Length == 0) return;
        if (stepTime >= stepLenght)
        {
            audioSource.PlayOneShot(stepSounds[Random.Range(0,stepSounds.Length)]);
            stepTime = 0;
        }
    }
    
    public void CharacterRotate(Quaternion rotation)
    {
        transform.rotation = rotation;
    }
}
