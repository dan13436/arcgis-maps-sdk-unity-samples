using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ContinuousMovement : MonoBehaviour
{
    public XRNode leftInputSource;
    public XRNode rightInputSource;
    private Vector2 leftInputAxis;
    private Vector2 rightInputAxis;
    private CharacterController controller;
    [SerializeField] private float speed;
    [SerializeField] private float upSpeed;
    [SerializeField] private InputActionProperty IncreaseSpeedAction;
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float speedAccelerator = 0.2f;
    private bool stillGoingInSameDirection = false;
    private float finalSpeed;
    private XROrigin rig;
    public LayerMask groundLayer;
    private float fallSpeed;
    private float heightOffset = 0.2f;

    private GameObject menu;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        rig = GetComponent<XROrigin>();
        menu = GameObject.FindWithTag("VRCanvas");
    }
    private void FixedUpdate()
    {
        FollowHeadset();

        Quaternion headYaw = Quaternion.Euler(0, rig.Camera.transform.eulerAngles.y, 0);
        Vector3 direction = headYaw * new Vector3(leftInputAxis.x, 0, leftInputAxis.y);
        Vector3 up = new Vector3(0, rightInputAxis.y, 0);

        if (IncreaseSpeedAction.action.ReadValue<float>() > 0.5f)
        {
            if (!stillGoingInSameDirection)
            {
                finalSpeed = speed * speedMultiplier;
            }

            if (direction != Vector3.zero)
            {
                finalSpeed += speedAccelerator;
                stillGoingInSameDirection=true;
            }
            else
            {
                finalSpeed = speed * speedMultiplier;
                stillGoingInSameDirection= false;
            }
        }
        else
        {
            finalSpeed = speed;
        }

        if (!menu.activeSelf)
        {
            controller.Move(direction * finalSpeed * Time.fixedDeltaTime);
            controller.Move(up * upSpeed * Time.fixedDeltaTime);
        }

    }
    // Update is called once per frame
    void Update()
    {
        UnityEngine.XR.InputDevice leftDevice = InputDevices.GetDeviceAtXRNode(leftInputSource);
        leftDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out leftInputAxis);
        UnityEngine.XR.InputDevice rightDevice = InputDevices.GetDeviceAtXRNode(rightInputSource);
        rightDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out rightInputAxis);
    }
    void FollowHeadset()
    {
        controller.height = rig.CameraInOriginSpaceHeight + heightOffset;
        Vector3 capsuleCenter = transform.InverseTransformPoint(rig.Camera.transform.position);
        controller.center = new Vector3(capsuleCenter.x, controller.height / 2 + controller.skinWidth, capsuleCenter.z);
    }
}
