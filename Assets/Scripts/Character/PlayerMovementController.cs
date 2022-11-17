using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public MovementState _state;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _sprintSpeed;
    [SerializeField] private float _groundDrag;
    [SerializeField] private float _airMultiplier;
    [SerializeField] private bool _exitingSlope;
    [SerializeField] private float _maxSlopeAngle;
    [Header("Keybinds")]
    [SerializeField] private KeyCode _sprintKey = KeyCode.LeftShift;
    [Header("Ground Check")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private bool _grounded;
    [SerializeField] private Transform _orientation;

    private float _horizontalInput;
    private float _verticalInput;
    private Vector3 _moveDirection;
    private RaycastHit slopeHit;


    private Rigidbody _cachedRigidbody;
    private Rigidbody CachedRigidbody
    {
        get
        {
            if (_cachedRigidbody == null)
                _cachedRigidbody = GetComponent<Rigidbody>();
            return _cachedRigidbody;
        }
    }


    private void Start()
    {
        CachedRigidbody.freezeRotation = true;
    }

    private void Update()
    {
        // ground check
        _grounded = Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        //handle drag
        if(_grounded) CachedRigidbody.drag = _groundDrag;
        else CachedRigidbody.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }


    private void MyInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void StateHandler()
    {
        //Mode - Sprinting
        if(_grounded && Input.GetKey(_sprintKey))
        {
            _state = MovementState.Sprinting;
            _moveSpeed = _sprintSpeed;
        }

        //Mode - walking
        else if(_grounded)
        {
            _state = MovementState.Walking;
            _moveSpeed = _walkSpeed;
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        _moveDirection = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;
        
        //on slope
        if(OnSlope() && !_exitingSlope)
        {
            //CachedRigidbody.velocity = _moveSpeed * 20f * GetSlopeMoveDirection() * Time.fixedDeltaTime;

            CachedRigidbody.AddForce(_moveSpeed * 20f * GetSlopeMoveDirection(), ForceMode.Force);

            if (CachedRigidbody.velocity.y > 0)
                CachedRigidbody.AddForce(Vector3.down * 80f, ForceMode.Force);
            //CachedRigidbody.velocity = Vector3.down * 80f * Time.fixedDeltaTime;

        }

        //on ground
        if(_grounded)
        {

            //CachedRigidbody.velocity = _moveSpeed * 10f * _moveDirection.normalized * Time.fixedDeltaTime;

            CachedRigidbody.AddForce(_moveSpeed * 10f * _moveDirection.normalized, ForceMode.Force);
        }

        //in air
        else if(!_grounded)
        {
            //CachedRigidbody.velocity = _airMultiplier * _moveSpeed * 10f * _moveDirection.normalized * Time.fixedDeltaTime;
            CachedRigidbody.AddForce(_airMultiplier * _moveSpeed * 10f * _moveDirection.normalized, ForceMode.Force);
        }

        //turn gravity off while on slope
        CachedRigidbody.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {

        //limit speed on slope
        if(OnSlope() && !_exitingSlope)
        {
            if(CachedRigidbody.velocity.magnitude > _moveSpeed)
                CachedRigidbody.velocity = CachedRigidbody.velocity.normalized * _moveSpeed;
        }

        //limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(CachedRigidbody.velocity.x, 0f, CachedRigidbody.velocity.z);

            //limit velocity if needed
            if(flatVel.magnitude > _moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * _moveSpeed;
                CachedRigidbody.velocity = new Vector3(limitedVel.x, CachedRigidbody.velocity.y, limitedVel.z);
            }
        }     
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, _playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < _maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(_moveDirection, slopeHit.normal).normalized;
    }
}

public enum MovementState
{
    Walking,
    Sprinting
}