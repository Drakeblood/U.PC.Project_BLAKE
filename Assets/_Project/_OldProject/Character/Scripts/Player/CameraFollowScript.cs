using UnityEngine;

[RequireComponent(typeof(PlayerInputController))]
public class CameraFollowScript : MonoBehaviour
{
    [SerializeField] private Transform playerPrefab;
    [SerializeField] private bool followPlayerWithMouseOffset = true;
    [SerializeField] private bool smoothCameraMovement = true;

    [Range(1,20)]
    [SerializeField] private float distanceBetweenMouseAndPlayer = 6;
    [Range(1,50)]
    [SerializeField] private float maxDistanceFromPlayer = 25;
    
    
    private Vector3 playerPos;
    private Vector2 _mousePosition;
    private Vector3 _mousePositionWorldPoint;
    private Camera _cam;

    private Vector3 velocity = Vector3.zero;
    
    private PlayerInputController _playerInputController;

    private void Awake()
    {
        _playerInputController = GetComponent<PlayerInputController>();
        _cam = Camera.main;
    }

    private void Start()
    {
        _playerInputController.mousePositionEvent += MousePositionHandler;
    }

    private void LateUpdate()
    {
        if (followPlayerWithMouseOffset)
        {
            FollowPlayerWithMouseOffset();
        }
        else
        {
            transform.position = playerPrefab.position;
        }
    }

    private void FollowPlayerWithMouseOffset()
    {
        playerPos = playerPrefab.position;
        GetMousePositionWorldPoint();

        float distance = Vector3.Distance(playerPos, _mousePositionWorldPoint);
        distance = Mathf.Clamp(distance, 0,maxDistanceFromPlayer);
        distance /= distanceBetweenMouseAndPlayer;
        
        Vector3 targetDirection = _mousePositionWorldPoint - playerPos;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 1f, 0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
        Vector3 targetPosition = transform.forward * distance + playerPos;


        if (smoothCameraMovement)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.05f);
        }
        else
        {
            transform.position = targetPosition;
        }
    }

    private void MousePositionHandler(Vector2 dir)
    {
        _mousePosition = dir;
    }

    private void GetMousePositionWorldPoint()
    {
        Plane playerPlane = new Plane(Vector3.up,playerPos);
        Ray ray = _cam.ScreenPointToRay(_mousePosition);

        if (playerPlane.Raycast(ray, out var hitDistance))
        {
            _mousePositionWorldPoint = ray.GetPoint(hitDistance);
        }
    }
}