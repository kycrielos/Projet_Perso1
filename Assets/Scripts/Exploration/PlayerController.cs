using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Input
    private float inputx;
    private float inputy;
    private float firePressedTimer;

    //Movement
    public float speed;
    Vector3 moveDirection;

    //Smoothing Movement
    public float accelerationSpeed;
    public float decelerationSpeed;
    private float accelerationValue;
    private bool inputDelayIsActive;
    private float inputDelayTimer;

    //Component
    public GameObject cam;
    private CharacterController controller;

    //Rotate
    float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        GameManager.Instance.player = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.playerCanMove)
        {
            GetInput();
            Move();
        }
        else
        {
            accelerationValue = 0;
        }
    }

    void Move()
    {
        //Speed acceleration, value from 0 to 1
        if (Mathf.Abs(inputx) > 0 || Mathf.Abs(inputy) > 0)
        {
            if (accelerationValue < 1)
            {
                accelerationValue += Time.deltaTime * accelerationSpeed;
            }
            else
            {
                accelerationValue = 1;
            }
        }
        else
        {
            if (accelerationValue > 0)
            {
                accelerationValue -= Time.deltaTime * decelerationSpeed;
            }
            else
            {
                accelerationValue = 0;
            }
        }

        //Transform the player input into usable Vector
        moveDirection = new Vector3(inputx, 0, inputy);
        Vector3.Normalize(moveDirection);

        //Move the player
        if (moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(EasingCurve(speed) * Time.deltaTime * moveDir);
        }
        else
        {
            controller.Move(EasingCurve(speed) * Time.deltaTime * transform.forward);
        }
    }

    //Smooth Movement
    //Curve is easeOutCirc from easings.net
    float EasingCurve(float maxSpeed)
    {
        return maxSpeed * Mathf.Sqrt(1 - Mathf.Pow(accelerationValue - 1, 2));
    }

    void GetInput()
    {
        //Delay of 3 frames if the player is stopping in diagonal to not make weird movement if the player release the 2 button with a little delay between

        if (Mathf.Abs(inputx) > 0 && Mathf.Abs(inputy) > 0 && Input.GetAxisRaw("Horizontal") == 0
             || Mathf.Abs(inputx) > 0 && Mathf.Abs(inputy) > 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            inputDelayIsActive = true;
        }

        if (inputDelayIsActive)
        {
            inputDelayTimer += Time.deltaTime;
            if (inputDelayTimer > 0.05f)
            {
                inputDelayTimer = 0;
                inputDelayIsActive = false;
            }
        }

        //Movement input
        if (!inputDelayIsActive)
        {
            inputx = Input.GetAxisRaw("Horizontal");
            inputy = Input.GetAxisRaw("Vertical");
        }

        /*if (GameManager.Instance.bossMode)
        {
            if (inputx > 0 || inputy > 0)
            {
                BossTimer.timerRunning = true;
            }
        }*/

        //Interaction input
        if (Input.GetButtonDown("Fire1"))
        {
            GameManager.Instance.firePressed = true;
            firePressedTimer = 0;
        }

        //Make the player action input last a little longer after its released (so player can press the button just before entering the trigger and make things more dynamic)
        if (GameManager.Instance.firePressed)
        {
            firePressedTimer += Time.deltaTime;
            if (firePressedTimer > 0.2f)
            {
                GameManager.Instance.firePressed = false;
            }
        }
    }
}
