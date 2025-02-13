using UnityEngine;

public class CharacterInputController : SingletonBase<CharacterInputController>
{
    [SerializeField] private float maxDistanceHitCamera = 2f;

    private Player _player;
    private OnePersonCamera _onePersonCamera;
    
    private Vector3 _playerMoveDirection;
    private float _radiusCharacter;
    private float _heightCharacter;
    
    private bool _isRun;
    public bool isMove = true;
    public bool VisionEnabled { get; private set; }

    private void Awake()
    {
        Init();
    }
    public void Start()
    {
        VisionEnabled = false;
        _onePersonCamera = OnePersonCamera.Instance;
        _player = GetComponent<Player>();
        _radiusCharacter = _player.GetComponentInChildren<CapsuleCollider>().radius;
        _heightCharacter = _player.GetComponentInChildren<CapsuleCollider>().height;
    }

    private void FixedUpdate()
    {
        CharacterMove();
        CharacterRotate();
    }

    private void Update()
    {
        CameraUpdate();
        MainRay();
    }
    
    private const string Horizontal = "Horizontal";
    private const string Vertical = "Vertical";

    private void CharacterMove()
    {
        var dirZ = Input.GetAxis(Vertical);
        var dirX = Input.GetAxis(Horizontal);

        if (!isMove)
        {
            dirZ = 0;
            dirX = 0;
        }

        var ground = IsGrounded();

        _playerMoveDirection = new Vector3(dirX, 0, dirZ);

        if (IsWall() && !ground) return;

        if (!ground)
        {
            _player.Move(_playerMoveDirection, MoveType.Air);
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _player.Move(_playerMoveDirection, MoveType.Run);
            return;
        }

        _player.Move(_playerMoveDirection, MoveType.Walk);
    }


    private void CharacterRotate()
    {
        if (_onePersonCamera.IsLocked || _onePersonCamera.enabled) return;
        var rotation = new Quaternion(0, _onePersonCamera.transform.rotation.y, 0, _onePersonCamera.transform.rotation.w);
        _player.CharacterRotate(rotation);
    }

    private const string XAxis = "Mouse X";
    private const string YAxis = "Mouse Y";

    private void CameraUpdate()
    {
        var dirY = Input.GetAxis(YAxis);
        var dirX = Input.GetAxis(XAxis);

        _onePersonCamera.Move(dirX, dirY);
    }

    private bool IsGrounded()
    {
        var vectorDown = _player.transform.TransformDirection(Vector3.down);
        var maxDistance = _heightCharacter / 2 + 0.1f;

        if (Physics.Raycast(_player.transform.position - new Vector3(_radiusCharacter, 0, 0), vectorDown,
                out var hitLegs, maxDistance))
        {
            Debug.DrawRay(_player.transform.position - new Vector3(_radiusCharacter, 0, 0),
                vectorDown * hitLegs.distance, Color.red);
            return true;
        }

        if (Physics.Raycast(_player.transform.position - new Vector3(-_radiusCharacter, 0, 0), vectorDown, out hitLegs,
                maxDistance))
        {
            Debug.DrawRay(_player.transform.position - new Vector3(-_radiusCharacter, 0, 0),
                vectorDown * hitLegs.distance, Color.red);
            return true;
        }

        if (Physics.Raycast(_player.transform.position - new Vector3(0, 0, _radiusCharacter), vectorDown, out hitLegs,
                maxDistance))
        {
            Debug.DrawRay(_player.transform.position - new Vector3(0, 0, _radiusCharacter),
                vectorDown * hitLegs.distance, Color.red);
            return true;
        }

        if (Physics.Raycast(_player.transform.position - new Vector3(0, 0, -_radiusCharacter), vectorDown, out hitLegs,
                maxDistance))
        {
            Debug.DrawRay(_player.transform.position - new Vector3(0, 0, -_radiusCharacter),
                vectorDown * hitLegs.distance, Color.red);
            return true;
        }

        return false;
    }

    private bool IsWall()
    {
        var maxRadius = _radiusCharacter + 0.1f;

        if (Physics.Raycast(_player.transform.position, _player.transform.TransformDirection(Vector3.forward),
                out var hitLegs, maxRadius))
        {
            Debug.DrawRay(_player.transform.position,
                _player.transform.TransformDirection(Vector3.forward) * hitLegs.distance, Color.red);
            return true;
        }

        if (Physics.Raycast(_player.transform.position, _player.transform.TransformDirection(Vector3.back),
                out hitLegs, maxRadius))
        {
            Debug.DrawRay(_player.transform.position,
                _player.transform.TransformDirection(Vector3.back) * hitLegs.distance, Color.red);
            return true;
        }

        if (Physics.Raycast(_player.transform.position, _player.transform.TransformDirection(Vector3.right),
                out hitLegs, maxRadius))
        {
            Debug.DrawRay(_player.transform.position,
                _player.transform.TransformDirection(Vector3.right) * hitLegs.distance, Color.red);
            return true;
        }

        if (Physics.Raycast(_player.transform.position, _player.transform.TransformDirection(Vector3.left),
                out hitLegs, maxRadius))
        {
            Debug.DrawRay(_player.transform.position,
                _player.transform.TransformDirection(Vector3.left) * hitLegs.distance, Color.red);
            return true;
        }

        return false;

    }

    private void MainRay()
    {
        if (Physics.Raycast(_onePersonCamera.transform.position, _onePersonCamera.transform.forward, out var hitCamera,
                maxDistanceHitCamera, LayerMask.NameToLayer("Player")))
        {
            Debug.DrawLine(_onePersonCamera.transform.position, hitCamera.transform.position, Color.yellow);
            var hitInteractiveObject = hitCamera.collider.transform.parent?.GetComponent<InteractiveObject>();
            if (hitInteractiveObject)
            {
                hitInteractiveObject.Hit();
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse0)) hitInteractiveObject.Interact();
            }
        }

    }
}
    

    

