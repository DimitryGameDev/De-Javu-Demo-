using UnityEngine;
public enum TypeMoveCamera
{
    None=0,
    OnlyMove=1,
    WithRotation=2
}
public class OnePersonCamera : SingletonBase<OnePersonCamera>
{
    [Range(0.1f, 9f)][SerializeField] private float sensitivity = 2f;
    [Range(0f, 90f)][SerializeField] private float yRotationLimit = 88f;
    [Range(6f, 100f)][SerializeField] private float cameraSmoothMoveSpeed = 8f;
    [Range(1f, 50f)][SerializeField] private float cameraSmoothRotateSpeed = 5f;
    
    public TypeMoveCamera typeMove;
    private Transform _target;
    
    private Vector2 _rotation;
    private bool _isLocked;
    public bool IsLocked => _isLocked;
    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        transform.parent = null;
        enabled = false;
    }

    public void Move(float dirX, float dirY)
    {
        if (enabled)return;
        transform.position = _target.position;
        
        if (_isLocked) return;
        _rotation.x += dirX * sensitivity;
        _rotation.y += dirY * sensitivity;
        _rotation.y = Mathf.Clamp(_rotation.y, -yRotationLimit, yRotationLimit);
        var xQuat = Quaternion.AngleAxis(_rotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(_rotation.y, Vector3.left);
        transform.localRotation = xQuat * yQuat;
    }

    private void Update()
    {
        if (typeMove == TypeMoveCamera.None)
        {
            
        }

        if (typeMove == TypeMoveCamera.WithRotation)
        {
            if (transform.position != _target.transform.position || transform.rotation != _target.transform.rotation)
            {
                transform.position = Vector3.MoveTowards(transform.position, _target.position, Time.deltaTime * cameraSmoothMoveSpeed);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _target.rotation, cameraSmoothRotateSpeed);
            }
            else
            {
                _rotation.x = _target.rotation.eulerAngles.y;
                _rotation.y = _target.rotation.eulerAngles.x;
                typeMove = TypeMoveCamera.None;
                enabled = false;
            }
        }

        if (typeMove == TypeMoveCamera.OnlyMove)
        {
            if (transform.position != _target.transform.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, _target.position , Time.deltaTime * cameraSmoothMoveSpeed);
            }
            else
            {
                typeMove = TypeMoveCamera.None;
                enabled = false;
            }
        }
    }
    
    private void Set(Transform targetForCamera, TypeMoveCamera typeMoveCamera)
    {
        typeMove = typeMoveCamera;
        _target = targetForCamera;
        enabled = true;
    }
    
    public void SetTarget(Transform targetForCamera,TypeMoveCamera typeMoveCamera)
    {
        Set(targetForCamera, typeMoveCamera);
    }
    
    public void SetTarget(Transform targetForCamera,TypeMoveCamera typeMoveCamera,bool cameraLock)
    {
        _isLocked = cameraLock;
        Set(targetForCamera, typeMoveCamera);
    }
    
    public void SetTarget(Transform targetForCamera,TypeMoveCamera typeMoveCamera,bool cameraLock,bool cursorLock)
    {
        _isLocked = cameraLock;
        if (cursorLock)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            CharacterInputController.Instance.enabled = true;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            CharacterInputController.Instance.enabled = false;
        }
        Set(targetForCamera, typeMoveCamera);
    }

    public void CameraLock(bool value)
    {
        _isLocked = value;
        if (_isLocked)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

}
