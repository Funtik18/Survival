using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Player owner = null;
    private CharacterController characterController = null;
    private Transform ownerTransform = null;

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
    [SerializeField] private float maxRunSpeed = 10.0f;
    [SerializeField] private float maxWalkSpeed = 6.0f;
    [SerializeField] private float maxSlopeSpeed = 3.0f;
    [SerializeField] private float gravity = -13.0f;
    [Range(0.0f, 0.5f)]
    [SerializeField] private float moveSmoothTime = 0.3f;

    [Space]
    [SerializeField] private float slopeForce;
    [SerializeField] private float slopeForceRayLength;

    [Space]
    [ReadOnly] [SerializeField] private float currentSensitivity;
    [ReadOnly] [SerializeField] private float currentSmoothTime;
    [ReadOnly][SerializeField] private float currentSpeed;

    private bool isMobileControll = false;

    private float cameraPitch = 0.0f;
    private float velocityY = 0.0f;

    private Vector2 currentDir = Vector2.zero;
    private Vector2 currentDirVelocity = Vector2.zero;

    private Vector2 currentMouseDelta = Vector2.zero;
    private Vector2 currentMouseDeltaVelocity = Vector2.zero;

    private Vector2 targetDelta;
    private Vector2 targetDir;

    public void Setup(Player owner, Transform ownerCamera, bool isMobileControll = false)
	{
        this.isMobileControll = isMobileControll;

        characterController = owner.characterController;
        ownerTransform = owner.transform;
        this.ownerCamera = ownerCamera;

        joystickMove = owner.playerUI.joystickMove;
        touchField = owner.playerUI.touchField;
    }

    public void UpdateMouseLook()
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

		targetDir.Normalize();
		currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        //up down velocity
        if(characterController.isGrounded)
            velocityY = 0.0f;
        velocityY += gravity * Time.deltaTime;

        //speed
        currentSpeed = maxWalkSpeed;

		if(OnSlope())
		{
			currentSpeed = maxSlopeSpeed;
		}

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