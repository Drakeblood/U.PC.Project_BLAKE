using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerInputController))]
public class PlayerMovement : MonoBehaviour
{
    private Camera cam;
    
    [SerializeField] private float playerSpeed;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float dashDuration;

    [SerializeField] private Transform gunHandlerTransform;

    private PlayerInputController _playerInputController;
    
    private Vector2 _movementAxis;
    private Vector2 _mousePosition;
    private float _angleRotationDifference;
    private Rigidbody _rigidbody;

    private float _dashCooldownCountdown;
    private float _dashDurationCountdown;
    private bool _dashPerformed;

    private Animator animator;

    private void Awake()
    {
        _playerInputController = GetComponent<PlayerInputController>();
        _rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        StartCoroutine(SetMainCamera());
    }

    private void Start()
    {
        _playerInputController.movementEvent += MovementHandler;
        _playerInputController.mousePositionEvent += MousePositionHandler;
        _playerInputController.dashEvent += Dash;
    }

    private void Update()
    {
        Rotation();
        DashCountdown();

        animator.SetFloat("Direction", CalculateDirection());
        animator.SetFloat("Speed", _rigidbody.velocity.magnitude);
    }


   void FixedUpdate()
    {
        MovePlayer();
        SpeedControl();
    }

    private void MovementHandler(Vector2 dir)
    {
        _movementAxis = dir;
    }
    private void MousePositionHandler(Vector2 dir)
    {
        _mousePosition = dir;
    }

    private void MovePlayer()
    {
        _movementAxis = _movementAxis.normalized;
        
        _rigidbody.AddForce(new Vector3(_movementAxis.x,0, _movementAxis.y) * (playerSpeed * 10f), ForceMode.Force);
    }
    
   private void Rotation()
   {
       Plane playerPlane = new Plane(Vector3.up, gunHandlerTransform.position);
       Ray ray = cam.ScreenPointToRay(_mousePosition);
       
       
       if (playerPlane.Raycast(ray, out var hitDistance))
       {
           Vector3 targetPoint = ray.GetPoint(hitDistance);
           Quaternion playerRotation = transform.rotation;
           Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position).normalized;
           targetRotation.x = 0;
           targetRotation.z = 0;

           if (!ShouldPlayerRotate(playerRotation, targetRotation)) return;
           _rigidbody.MoveRotation(Quaternion.Slerp(playerRotation, targetRotation, 20f * Time.deltaTime).normalized);
        }
   }

   private bool ShouldPlayerRotate(Quaternion playerRotation, Quaternion targetRotation)
   {
       //without this function, rigidbody calculating smth all the time
       var angle = Quaternion.Angle(playerRotation, targetRotation);
       if (_angleRotationDifference == angle) return false;
       
       _angleRotationDifference = angle;
       return true;

   }

   private void Dash()
   {
       if (_dashCooldownCountdown > 0) return;
       
       _rigidbody.AddForce(_rigidbody.velocity * dashForce, ForceMode.Impulse);

       _dashPerformed = true;
   }

   private void SpeedControl()
   {
       if (_dashDurationCountdown > 0) return;
       
       var rbVelocity = _rigidbody.velocity;
       
       Vector3 currentVelocity = new Vector3(rbVelocity.x, 0, rbVelocity.z);
       if (currentVelocity.magnitude > playerSpeed)
       {
           currentVelocity = currentVelocity.normalized * playerSpeed;
           _rigidbody.velocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
       }
   }

   private void DashCountdown()
   {
       if (_dashDurationCountdown > 0)
       {
           _dashDurationCountdown -= Time.deltaTime;
       }

       if (_dashCooldownCountdown > 0)
       {
           _dashCooldownCountdown -= Time.deltaTime;
       }

       if (_dashPerformed)
       {
           _dashDurationCountdown = dashDuration;
           _dashCooldownCountdown = dashCooldown;
           _dashPerformed = false;
       }
   }

   private IEnumerator SetMainCamera()
   {
       while (Camera.main == null)
       {
           yield return new WaitForSeconds(0.1f);
       }

       cam = Camera.main;
   }

   float CalculateDirection()
    {
	    if (_rigidbody.velocity != Vector3.zero)
	    {
            Vector3 NormalizedVel = _rigidbody.velocity.normalized;

            float ForwardCosAngle = Vector3.Dot(transform.forward, NormalizedVel);
            float ForwardDeltaDegree = Mathf.Acos(ForwardCosAngle) * Mathf.Rad2Deg;

            float RightCosAngle = Vector3.Dot(transform.right, NormalizedVel);
		    if (RightCosAngle < 0)
		    {
			    ForwardDeltaDegree *= -1;
		    }

            return ForwardDeltaDegree;
	    }

	    return 0f;
    }
}
