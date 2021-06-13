using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;

    [Title("Angle")]
    [SerializeField] private Vector2 pitchYMinMaxClamp = new Vector2(-90.0f, 45.0f);

    [Title("Sensitivity-Smooth")]
    [SerializeField] private float sensitivity = 1.5f;
    [Range(0.0f, 0.5f)]
    [SerializeField] private float smooth = 0.03f;

    [Space]
    [Range(0.0f, 0.5f)]
    [SerializeField] private float moveSmooth = 0.3f;
    
    [Title("Speed")]
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private float maxRunSpeed = 5.5f;
    [SerializeField] private float maxWalkSpeed = 3.0f;
    [SerializeField] private float maxSlopeSpeed = 2.0f;
    [ReadOnly] [SerializeField] private float currentSpeed = 0;

    [Title("Slope")]
    [SerializeField] private float slopeForce;
    [SerializeField] private float slopeForceRayLength;
    [Space]
    [SerializeField] private float gravity = -13.0f;

    #region Properties
    private Transform ownerTransform;
    private Transform OwnerTransform
    {
        get
        {
            if (ownerTransform == null)
                ownerTransform = GeneralAvailability.Player.transform;
            return ownerTransform;
        }
    }

    private Transform ownerCamera;
    private Transform OwnerCamera
    {
        get
        {
            if (ownerCamera == null)
                ownerCamera = GeneralAvailability.Player.Camera.transform;
            return ownerCamera;
        }
    }

    private PlayerControlUI controlUI;
    private PlayerControlUI ControlUI
    {
        get
        {
            if (controlUI == null)
                controlUI = GeneralAvailability.PlayerUI.controlUI;
            return controlUI;
        }
    }
    #endregion

    private PlayerStatus status;

    private bool isSpeedUpBlocked = false;
    private bool isSpeedUp = false;
    private float speedTimePosition;

    private float cameraPitch = 0.0f;

    private float currentSensitivity;
    private float currentSmooth;

    private Vector2 currentDirection = Vector2.zero;
    private Vector2 currentDirVelocity = Vector2.zero;

    private Vector2 currentMouseDelta = Vector2.zero;
    private Vector2 currentMouseDeltaVelocity = Vector2.zero;

    private Vector2 targetLookDirection;
    private Vector2 targetMoveDirection;

    public void Setup(Player player)
	{
        this.status = player.Status;
        currentSpeed = maxWalkSpeed;

        currentSensitivity = sensitivity;
        currentSmooth = smooth;
    }

    public void Enable(bool trigger)
    {
        characterController.enabled = trigger;
    }

    public void SetLookSensitivity(float sensitivity)
    {
        currentSensitivity = sensitivity;
    }
    public void ResetLookSensitivity()
    {
        currentSensitivity = sensitivity;
    }


    public void UpdatePCLook()
    {
        targetLookDirection = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        UpdateLook();
    }
    public void UpdateMobileLook()
    {
        targetLookDirection = ControlUI.fieldLook.Direction;
        targetLookDirection.Normalize();

        UpdateLook();
    }


    public void UpdatePCMovement()
    {
        targetMoveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (targetMoveDirection == Vector2.zero)
        {
            currentDirection = Vector2.zero;

            PlayerCamera.Instance.StartIdleShake();
        }
        else
        {
            targetMoveDirection.Normalize();
            currentDirection = Vector2.SmoothDamp(currentDirection, targetMoveDirection, ref currentDirVelocity, moveSmooth);

            if (Input.GetKeyDown(KeyCode.LeftShift))
                SpeedUp();
            else if (Input.GetKeyUp(KeyCode.LeftShift))
                SpeedDown();

            if (isSpeedUp)
            {
                if (currentSpeed != maxRunSpeed)
                {
                    speedTimePosition += Time.deltaTime;
                    currentSpeed = Mathf.Lerp(maxWalkSpeed, maxRunSpeed, accelerationCurve.Evaluate(speedTimePosition));
                }
            }
        }
    }
    public void UpdateMobileMovement()
    {
        targetMoveDirection = ControlUI.joystickMove.Direction;

        if (targetMoveDirection == Vector2.zero)
        {
            currentDirection = Vector2.zero;

            PlayerCamera.Instance.StartIdleShake();
        }
        else
        {
            targetMoveDirection.Normalize();
            currentDirection = Vector2.SmoothDamp(currentDirection, targetMoveDirection, ref currentDirVelocity, moveSmooth);

            if (isSpeedUp)
            {
                if (currentSpeed != maxRunSpeed)
                {
                    speedTimePosition += Time.deltaTime;
                    currentSpeed = Mathf.Lerp(maxWalkSpeed, maxRunSpeed, accelerationCurve.Evaluate(speedTimePosition));
                }
            }
        }
    }
    public void UpdateMovement()
    {
        UpdateSlope();
        UpdateVelocity();
    }
    public void UpdateGravity()
    {
        Vector3 velocity = Vector3.zero;
        if (!characterController.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity);
        }
    }


    public void SpeedUp()
    {
        if (status.IsCanRunning)
        {
            isSpeedUp = true;
        }
        else
        {
            SpeedDown();
        }
    }
    public void SpeedDown()
    {
        isSpeedUp = false;
        speedTimePosition = 0;
        currentSpeed = maxWalkSpeed;
    }




    private void UpdateLook()
    {
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetLookDirection, ref currentMouseDeltaVelocity, currentSmooth);

        cameraPitch -= currentMouseDelta.y * currentSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, pitchYMinMaxClamp.x, pitchYMinMaxClamp.y);

        OwnerCamera.localEulerAngles = Vector3.right * cameraPitch;

        OwnerTransform.Rotate(Vector3.up * currentMouseDelta.x * currentSensitivity);
    }

    private void UpdateSlope()
    {
        if (OnSlope())
        {
            //currentSpeed = maxSlopeSpeed;
        }
    }

    private void UpdateVelocity()
    {
        if(currentDirection != Vector2.zero)
        {
            if (isSpeedUp)
            {
                status.states.CurrentState = PlayerStates.State.Sprinting;
            }
            else
            {
                status.states.CurrentState = PlayerStates.State.Walking;
            }
        }
        else
        {
            status.states.CurrentState = PlayerStates.State.Standing;
        }

        if (isSpeedUp && !status.IsCanRunning)
            SpeedDown();

        Vector3 velocity = (OwnerTransform.forward * currentDirection.y + OwnerTransform.right * currentDirection.x) * currentSpeed;
        characterController.Move(velocity * Time.deltaTime);
    }


    /// <summary>
    /// Проверка, залез ли на слон.
    /// </summary>
    /// <returns></returns>
    private bool OnSlope()
	{
        RaycastHit hit;

        if(Physics.Raycast(OwnerTransform.position, Vector3.down, out hit, characterController.height / 2 * slopeForceRayLength))
		{
            if(hit.normal != Vector3.up)
                return true;
		}
        return false;
	}
}