using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Character Controller")]
    //no need to pass anything, will be setup in start.
    public CharacterController characterController; 

    [Header("Movement Variables")]
    [SerializeField] public float speed = 10f;
    [SerializeField] private bool isGround = true;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDist = 0.4f;
    [SerializeField] private LayerMask groundMask;


    //internal variables
    Vector3 velocity;
    float footstepSoundThreshold = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGround = Physics.CheckSphere(groundCheck.position, groundDist, groundMask);

        if (isGround && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    public void MovePlayer()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        characterController.Move(speed * Time.deltaTime * move);

        AudioManager.instance.PlayPlayerFootstep(move, footstepSoundThreshold, isGround, PlayerBehaviour.instance.playerAge, this.gameObject);

        if (Input.GetButtonDown("Jump") && isGround)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
