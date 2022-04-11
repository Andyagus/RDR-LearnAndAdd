using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementInput : MonoBehaviour
{

    public float Velocity;
    public float InputX;
    public float InputZ;
    public Vector3 desiredMoveDirection;
    public bool blockRotationPlayer;
    public float desiredRotationSpeed = 0.1f;
    public Animator anim;
    public float Speed;
    public float allowPlayerRotation = 0.1f;
    public Camera cam;
    public CharacterController controller;
    public bool isGrounded;
    private float verticalVel;
    private Vector3 moveVector;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        InputMagnitude();
        isGrounded = controller.isGrounded;

        if(isGrounded == true)
        {
            verticalVel -= 0;
        }
        else
        {
            verticalVel -= 2;
        }

        moveVector = new Vector3(0, verticalVel, 0);
        controller.Move(moveVector);
    }

    void PlayerMoveAndRotation()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.y = right.y = 0;

        forward.Normalize();
        right.Normalize();

        desiredMoveDirection = forward * InputZ + right * InputX;

        if (!blockRotationPlayer)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
            controller.Move(desiredMoveDirection * Time.deltaTime * Velocity);
        }
    }

    public void LookAt(Vector3 pos)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), desiredRotationSpeed);
    }

    public void RotateToCamera()
    {

    }

    void InputMagnitude()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        Speed = new Vector2(InputX, InputZ).sqrMagnitude;

        if(Speed > allowPlayerRotation)
        {
            PlayerMoveAndRotation();
        }
        
    }
}
