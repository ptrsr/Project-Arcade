using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    private enum CameraState
    {
        Menu,
        Game
    }

    [SerializeField]
    private CameraState _cameraState = CameraState.Menu;

    [SerializeField]
    private float
        _distance = 0.5f,
        _forwardMulti = 0.5f,
        _hoverHeight,
        _focusHeight;

    [SerializeField] [Range(0, 20)]
    private float
        _moveSpeed     = 0.9f,
        _rotateSpeed   = 0.9f;

    [SerializeField]
    private Vector2 _angle = new Vector2();

    [SerializeField]
    private Transform _menuTransform;

    private Vector3 _currentFocusPos;

    private Globe _globe;
    private MovingObject _target;

	void Start ()
    {
        _target = ServiceLocator.Locate<SpaceShip>();
        _globe = ServiceLocator.Locate<Globe>();

        if (_cameraState == CameraState.Game)
        {
            _currentFocusPos = _target.transform.position;
            SetCameraTransform(_target);
        }
        else
        {
            transform.position = _menuTransform.position;
            transform.rotation = _menuTransform.rotation;
        }
    }

    void FixedUpdate ()
    {
        if (_cameraState == CameraState.Game)
            Follow(_target, GetFocusPosition(_target));
    }

    private void OnValidate()
    {
        ServiceLocator.Provide(this);

        SpaceShip target = ServiceLocator.Locate<SpaceShip>();

        if (_cameraState == CameraState.Game && target != null)
            SetCameraTransform(target);
        else if (_menuTransform != null)
        {
            transform.position = _menuTransform.position;
            transform.rotation = _menuTransform.rotation;
        }

    }

    private void Follow(GlobeObject HoverTarget, Vector3 focusPosition)
    {
        transform.position = Vector3.Lerp(transform.position, HoverPosition(HoverTarget), Mathf.Min(_moveSpeed * Time.deltaTime, 1));
        transform.rotation = Quaternion.LookRotation((focusPosition - transform.position).normalized, HoverTarget.transform.up);
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
