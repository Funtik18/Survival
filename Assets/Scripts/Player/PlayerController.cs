using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;

    #region Properties
    private Transform ownerTransform;
    private Transform OwnerTransform
    {
        get
        {
            if (ownerTransform == null)
                ownerTransform = Player.Instance.transform;
            return ownerTransform;
        }
    }

    private Transform ownerCamera;
    private Transform OwnerCamera
    {
        get
        {
            if(ownerCamera == null)
                ownerCamera = Player.Instance.playerCamera.transform;
            return ownerCamera;
        }
    }

    private PlayerControlUI controlUI;
    private PlayerControlUI ControlUI
    {
        get
        {
            if (controlUI == null)
                controlUI = Player.Instance.playerUI.controlUI;
            return controlUI;
        }
    }
    #endregion

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

    private bool speedUp = false;

    private float cameraPitch = 0.0f;
    private float velocityY = 0.0f;

    private Vector2 currentDirection = Vector2.zero;
    private Vector2 currentDirVelocity = Vector2.zero;

    private Vector2 currentMouseDelta = Vector2.zero;
    private Vector2 currentMouseDeltaVelocity = Vector2.zero;

    private Vector2 targetLookDirection;
    private Vector2 targetMoveDirection;

    public void Setup()
	{
        currentSpeed = maxWalkSpeed;
    }
    public void UpdatePCLook()
    {
        currentSensitivity = mouseSensitivity;
        currentSmoothTime = mouseSmoothTime;
        targetLookDirection = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        UpdateLook();
    }
    public void UpdateMobileLook()
    {
        currentSensitivity = touchSensitivity;
        currentSmoothTime = touchSmoothTime;
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
            currentDirection = Vector2.SmoothDamp(currentDirection, targetMoveDirection, ref currentDirVelocity, moveSmoothTime);


            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                speedUp = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                speedTimePosition = 0;
                currentSpeed = maxWalkSpeed; 
                speedUp = false;
            }
        }


        UpdateSlope();
        UpdateGravity();
        UpdateVelocity();
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
            currentDirection = Vector2.SmoothDamp(currentDirection, targetMoveDirection, ref currentDirVelocity, moveSmoothTime);

            if (speedUp)
            {
                if (currentSpeed != maxRunSpeed)
                {
                    speedTimePosition += Time.deltaTime;
                    currentSpeed = Mathf.Lerp(maxWalkSpeed, maxRunSpeed, accelerationCurve.Evaluate(speedTimePosition));
                }
            }
        }

        UpdateSlope();
        UpdateGravity();
        UpdateVelocity();
    }

    private void UpdateLook()
    {
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetLookDirection, ref currentMouseDeltaVelocity, currentSmoothTime);

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
    private void UpdateGravity()
    {
        if (characterController.isGrounded)
            velocityY = 0.0f;
        velocityY += gravity * Time.deltaTime;
    }
    private void UpdateVelocity()
    {
        Vector3 velocity = (OwnerTransform.forward * currentDirection.y + OwnerTransform.right * currentDirection.x) * currentSpeed + Vector3.up * velocityY;
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