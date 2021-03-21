using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;

    private Transform ownerTransform;

    private Transform ownerCamera = null;

    private Joystick joystickMove;
    private FixedTouchField touchField;

    [Space]
    [SerializeField] private Vector2 pitchYMinMaxClamp = new Vector2(-90.0f, 45.0f);

    [SerializeField] private float mouseSensitivity = 3.5f;
    [SerializeField] private float touchSensitivity = 1.5f;

    [Range(0.0f, 0.5f)]
    [SerializeField] private float touchSmoothTime = 0.03f;
    [Range(0.0f, 0.5f)]
    [SerializeField] private float mouseSmoothTime = 0.03f;

    [Space]
    [Title("Speed")]
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private float maxRunSpeed = 5.5f;
    [SerializeField] private float maxWalkSpeed = 3.0f;
    [SerializeField] private float maxSlopeSpeed = 2.0f;
    [ReadOnly] [SerializeField] private float currentSpeed = 0;
    private float speedTimePosition;

    [Title("Endurance")]
    [SerializeField] private float maxEndurance = 100;
    [ReadOnly] [SerializeField] private float currentEndurance;


    [SerializeField] private float gravity = -13.0f;
    [Range(0.0f, 0.5f)]
    [SerializeField] private float moveSmoothTime = 0.3f;

    [Space]
    [SerializeField] private float slopeForce;
    [SerializeField] private float slopeForceRayLength;

    [Space]
    [ReadOnly] [SerializeField] private float currentSensitivity;
    [SerializeField] private float currentSmoothTime;

    private bool isMobileControll = false;

    private bool speedUp = false;

    private float cameraPitch = 0.0f;
    private float velocityY = 0.0f;

    private Vector2 currentDir = Vector2.zero;
    private Vector2 currentDirVelocity = Vector2.zero;

    private Vector2 currentMouseDelta = Vector2.zero;
    private Vector2 currentMouseDeltaVelocity = Vector2.zero;

    private Vector2 targetDelta;
    private Vector2 targetDir;

    [Space]
    public float magnitude;
    public float roughness;
    public float fadeInTime;
    public float fadeOutTime;



    public void Setup(Player owner, bool isMobileControll = false)
	{
        this.isMobileControll = isMobileControll;
        this.ownerCamera = owner.playerCamera.Transform;
        this.ownerTransform = owner.Transform;
        
        PlayerUI playerUI = owner.playerUI;

        joystickMove = playerUI.joystickMove;
        touchField = playerUI.touchField;

        //speed
        currentSpeed = maxWalkSpeed;

        playerUI.buttonSpeedUp.onPressed += () => { speedUp = true; };
		playerUI.buttonSpeedUp.onUnPressed += () => { currentSpeed = maxWalkSpeed; speedUp = false; };
    }

    public void UpdateLook()
    {
		if(!isMobileControll)
		{
            currentSensitivity = mouseSensitivity;
            currentSmoothTime = mouseSmoothTime;
            targetDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }
		else
		{
            currentSensitivity = touchSensitivity;
            currentSmoothTime = touchSmoothTime;
            targetDelta = touchField.Direction;
            targetDelta.Normalize();
        }


        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetDelta, ref currentMouseDeltaVelocity, currentSmoothTime);

        cameraPitch -= currentMouseDelta.y * currentSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, pitchYMinMaxClamp.x, pitchYMinMaxClamp.y);

        ownerCamera.localEulerAngles = Vector3.right * cameraPitch;
        ownerTransform.Rotate(Vector3.up * currentMouseDelta.x * currentSensitivity);
    }

    public void UpdateMovement()
    {
        //direction move left right
        if(!isMobileControll)
            targetDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        else
            targetDir = joystickMove.Direction;

		if(targetDir == Vector2.zero)
		{
			currentDir = Vector2.zero;

            PlayerCamera.Instance.StartIdleShake();
        }
		else
		{
            targetDir.Normalize();
            currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);


            if(Input.GetKeyDown(KeyCode.LeftShift))
            {
                speedUp = true;
            }
            else if(Input.GetKeyUp(KeyCode.LeftShift))
            {
                speedTimePosition = 0;
                currentSpeed = maxWalkSpeed; speedUp = false;
            }
            if(speedUp)
            {
                if(currentSpeed != maxRunSpeed)
                {
                    speedTimePosition += Time.deltaTime;
                    currentSpeed = Mathf.Lerp(maxWalkSpeed, maxRunSpeed, accelerationCurve.Evaluate(speedTimePosition));
                }
            }
        }


        if(OnSlope())
        {
            //currentSpeed = maxSlopeSpeed;
        }

        //up down velocity
        if(characterController.isGrounded)
            velocityY = 0.0f;
        velocityY += gravity * Time.deltaTime;

        Vector3 velocity = (ownerTransform.forward * currentDir.y + ownerTransform.right * currentDir.x) * currentSpeed + Vector3.up * velocityY;
        characterController.Move(velocity * Time.deltaTime);
    }





    /// <summary>
    /// Проверка, залез ли на слон.
    /// </summary>
    /// <returns></returns>
    private bool OnSlope()
	{
        RaycastHit hit;

        if(Physics.Raycast(ownerTransform.position, Vector3.down, out hit, characterController.height / 2 * slopeForceRayLength))
		{
            if(hit.normal != Vector3.up)
                return true;
		}
        return false;
	}
}