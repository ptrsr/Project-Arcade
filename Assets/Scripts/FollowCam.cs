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

    private Vector3 _currentFocusPos;

    private SpaceShip _ship;

	void Start ()
    {
        _ship = ServiceLocator.Locate<SpaceShip>();

        _currentFocusPos = _ship.transform.position;
        SetCameraTransform(_ship.transform);
	}
	
	void FixedUpdate ()
    {
        Follow(_ship.transform, GetFocusPosition(_ship));
	}

    private void OnValidate()
    {
        ServiceLocator.Provide(this);

        SpaceShip ship = ServiceLocator.Locate<SpaceShip>();

        if (ship != null)
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

    public void SetCameraTransform(Transform focusTarget)
    {
        transform.position = HoverPosition(focusTarget);
        transform.rotation = Quaternion.LookRotation((focusTarget.position - transform.position).normalized, focusTarget.up);
    }

    private Vector3 GetFocusPosition(SpaceShip ship)
    {
        Vector3 newFocusPos = ship.GlobePosition + new Vector3(0, ship.GlobeRadius, 0) +  (ship.GlobePosition - ship.MoveTarget.GlobePosition).normalized * _forwardMulti;
        _currentFocusPos = Vector3.Slerp(_currentFocusPos, Globe.GlobeToScenePosition(newFocusPos), _rotateSpeed);

        return _currentFocusPos;
    }
}
