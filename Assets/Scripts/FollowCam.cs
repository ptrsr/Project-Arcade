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
        _moveSpeed = 0.9f,
        _extraSpeed = 0.9f;

    private SpaceShip _ship;

	void Start ()
    {
        _ship = ServiceLocator.Locate<SpaceShip>();

        SetCameraTransform(_ship.transform);
	}
	
	void Update ()
    {
        Follow(_ship.transform, _ship.CameraTarget);
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
}
