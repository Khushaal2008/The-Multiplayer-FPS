using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField] private float lookSensitivity = 3f;

    private PlayerMotor motor;

    [SerializeField]
    private float thrusterForce = 1000f;
    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;
    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f;
    private float thrusterFuelAmount = 1f;

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }

    [SerializeField]
    private LayerMask environmentMask;

    [Header("Spring Settings:")]
    
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 20f;

    private ConfigurableJoint joint;



    //private Animator animator;

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();

        SetJointSettings(jointSpring);
        //animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(PauseMenu.IsOn)
        return;

        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");

        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;

        Vector3 _velocity = (_movHorizontal + _movVertical) * speed;

       // animator.SetFloat("forwardVelocity",_zMov);

        motor.Move(_velocity);

        float _yRot = Input.GetAxisRaw("Mouse X");
        
        Vector3 _rotation = new Vector3(0f,_yRot,0f) * lookSensitivity;

        motor.Rotate(_rotation);


        float _xRot = Input.GetAxisRaw("Mouse Y");
        
        float _cameraRotationX = _xRot * lookSensitivity;

        motor.RotateCamera(_cameraRotationX);
        
        Vector3 _thrusterForce = Vector3.zero;
        if(Input.GetButton("Jump") && thrusterFuelAmount > 0f)
        {
            
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if(thrusterFuelAmount >= 0.1f)
            {
            _thrusterForce = Vector3.up * thrusterForce;
            SetJointSettings(0f);
            }
        }
        else
        {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
            SetJointSettings(jointSpring);
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount,0f,1f);

        motor.ApplyThruster(_thrusterForce);

    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive{ positionSpring = jointSpring, maximumForce = jointMaxForce };
    }

    

    
}
