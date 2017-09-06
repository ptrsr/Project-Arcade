using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField]
    private float
        _angle = 0.0f,
        _distance = 0.5f,
        _forwardMulti = 0.5f;

    [SerializeField] [Range(0, 20)]
    private float
        _moveSpeed     = 0.9f,
        _rotateSpeed   = 0.9f;

    private Vector3 _cameraTarget;
    private float _radius;

    private SpaceShip _ship;

	void Start ()
    {
        _ship = ServiceLocator.Locate<SpaceShip>();
        _radius = ServiceLocator.Locate<Globe>().Radius / 2;

        SetCameraTransform(_ship.transform);
	}
	
	void Update ()
    {
        Follow(_ship.transform, GetFocusPosition(_ship));
	}

    private void OnValidate()
    {
        ServiceLocator.Provide(this);

        SpaceShip ship = ServiceLocator.Locate<SpaceShip>();

        if (ship == null)
            return;

        SetCameraTransform(ship.transform);
    }

    private void Follow(Transform HoverTarget, Vector3 focusPosition)
    {
        transform.position = Vector3.Lerp(transform.position, HoverPosition(HoverTarget), Mathf.Min(_moveSpeed * Time.deltaTime, 1));
        transform.rotation = Quaternion.LookRotation((focusPosition - transform.position).normalized, HoverTarget.up);
    }

    private Vector3 HoverPosition(Transform focusTarget)
    {
        return focusTarget.TransformPoint(new Vector3(0, Mathf.Sin(_angle), Mathf.Cos(_angle)) * _distance);
    }

    private void SetCameraTransform(Transform focusTarget)
    {
        transform.position = HoverPosition(focusTarget);
        transform.LookAt(focusTarget.transform);
    }

    private Vector3 GetFocusPosition(SpaceShip ship)
    {
        Vector2 camPosition2D = ship.Position2D + ship.Move * _forwardMulti * Time.deltaTime;
        Vector3 camPosition3D = new Vector3(Mathf.Sin(camPosition2D.x), Mathf.Cos(camPosition2D.x), 0) * (_radius + camPosition2D.y);

        _cameraTarget = Vector3.Lerp(_cameraTarget, camPosition3D, _rotateSpeed);

        return _cameraTarget;
    }
}
