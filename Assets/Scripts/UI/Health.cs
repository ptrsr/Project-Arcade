using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Health : MonoBehaviour
{
    private DestroyableObject _player;
    private float _maxHealth;
    private UnityEngine.UI.Slider _slider;

    [SerializeField]
    private float _lerpSpeed = 0.1f;

    void SetTarget()
    {
        _player = ServiceLocator.Locate<SpaceShip>().GetComponent<DestroyableObject>();
        _maxHealth = _player.Health;
    }

    private void Awake()
    {
        ObjectSafe.onSpawn += SetTarget;
        _slider = GetComponent<UnityEngine.UI.Slider>();
    }

	void Update () {

        float desired;

        if (_player != null)
            desired = _player.Health;
        else
            desired = 0.01f;

        _slider.value = Mathf.Lerp(_slider.value, desired / _maxHealth, _lerpSpeed * Time.deltaTime);
	}
}
