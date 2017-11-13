using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField]
    private float
        _distance = 0.5f,
        _forwardMulti = 0.5f,
        _hoverHeight,
        _focusHeight,
        _returnSpeed;

    [SerializeField] [Range(0, 20)]
    private float
        _moveSpeed     = 0.9f,
        _rotateSpeed   = 0.9f;

    [SerializeField]
    private Vector2 _angle = new Vector2();

    private float 
        _delta,
        _linear;

    private Vector3 _currentFocusPos;

    private Globe        _globe;
    private Menu         _menu;
    private MovingObject _target;
    private PostFX       _postFX;

    private void Awake()
    {
        ObjectSafe.onSpawn += SetTarget;
    }

    private void SetTarget()
    {
        _target = ServiceLocator.Locate<SpaceShip>();
    }

    void Start ()
    {
        _target = ServiceLocator.Locate<SpaceShip>();
        _globe = ServiceLocator.Locate<Globe>();
        _menu = ServiceLocator.Locate<Menu>();
        _postFX = ServiceLocator.Locate<PostFX>();

        if (_menu.GameState == GameState.Game)
        {
            _currentFocusPos = _target.transform.position;
            SetCameraTransform(_target);
        }
        else
        {
            transform.position = _menu.transform.position;
            transform.rotation = _menu.transform.rotation;
        }

        Vector3 desiredPosition = HoverPosition(_target);
        _delta = Vector3.Distance(desiredPosition, _menu.transform.position);
    }

    void FixedUpdate ()
    {
        _linear = 1 - Vector3.Distance(transform.position, _menu.transform.position) / _delta;
        _postFX.Grayout = _linear;

        if (_menu.GameState == GameState.Game)
            Follow(_target, GetFocusPosition(_target));

        if (_menu.GameState == GameState.Menu)
            FlyToMenu();
    }

    private void FlyToMenu()
    {
        transform.position = Vector3.Lerp(transform.position, _menu.transform.position, _returnSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, _menu.transform.rotation, _returnSpeed);
    }

    private void Follow(GlobeObject HoverTarget, Vector3 focusPosition)
    {
        Vector3 desiredPosition = HoverPosition(HoverTarget);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Mathf.Min(_moveSpeed * Time.deltaTime, 1));

        Quaternion desiredRotation = Quaternion.LookRotation((focusPosition - transform.position).normalized, HoverTarget.transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation,  (1 -_linear) * _returnSpeed);
    }

    private Vector3 HoverPosition(GlobeObject focusTarget)
    {
        Vector3 focusPos = focusTarget.GlobePosition;
        focusPos.y = _hoverHeight * Radius;

        focusPos = Globe.GlobeToScenePosition(focusPos);

        Quaternion cameraRotation = Quaternion.LookRotation(Vector3.forward, focusTarget.GlobeUp);
        return focusPos + cameraRotation * (new Vector3(Mathf.Sin(_angle.x), Mathf.Sin(_angle.y), Mathf.Cos(_angle.x)) * -_distance);
    }

    public void SetCameraTransform(GlobeObject focusTarget)
    {
        transform.position = HoverPosition(focusTarget);
        transform.rotation = Quaternion.LookRotation((focusTarget.ScenePosition - transform.position).normalized, focusTarget.transform.up);
    }

    private Vector3 GetFocusPosition(MovingObject target)
    {
        float focusHeight = _focusHeight * Radius;

        Vector3 focusGlobePos = target.GlobePosition;
        focusGlobePos.y = target.GlobeRadius + focusHeight;

        Vector3 focusTargetPos = target.LastMove + target.GlobePosition;
        focusGlobePos.y = target.GlobeRadius + focusHeight;

        Vector3 newFocusPos = focusGlobePos +  (focusGlobePos - focusTargetPos).normalized * _forwardMulti;
        _currentFocusPos = Vector3.Slerp(_currentFocusPos, Globe.GlobeToScenePosition(newFocusPos), _rotateSpeed);

        return _currentFocusPos;
    }

    private float Radius
    {
        get
        {
            if (_globe == null)
                _globe = ServiceLocator.Locate<Globe>();

            return _globe.Radius;
        }
    }
}
