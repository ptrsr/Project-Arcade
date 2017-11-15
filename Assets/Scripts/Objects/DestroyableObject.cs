using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour
{
    [SerializeField]
    private float
        _health,
        _explosionSize,
        _partForceMulti = 3;

    [SerializeField]
    private GameObject
        _destroyedPrefab;

    [SerializeField]
    private MonoBehaviour[] _removedBehaviours;

    [SerializeField]
    private bool
        _execludeFromScore = false,
        _noParts = false;

    private bool
        _exploded = false,
        _destroyableByImpact = false,
        _registered = false,
        _quitting = false;


    private float _maxHealth;

    private Rigidbody _rigidBody;

    private void Awake()
    {
        if (!_execludeFromScore)
        ObjectSafe.onGameStart += RegisterAsScore;

        _maxHealth = _health;
    }

    private void RegisterAsScore(ObjectSafe safe)
    {
        ServiceLocator.Locate<Score>().RegisterScorePoint();
        _registered = true;
    }

    void Update ()
    {
        if (!_exploded && _health < 0)
            Explode();

        _rigidBody = GetComponent<Rigidbody>();

        if (_rigidBody != null)
            _destroyableByImpact = true;
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (!_destroyableByImpact)
            return;

        float damage = collision.impulse.magnitude;
        Damage(damage);
    }

    public void Damage(float damage)
    {
        _health -= Mathf.Abs(damage);
    }

    public void Heal(float heal)
    {
        _health = Mathf.Min(_health + Mathf.Abs(heal), _maxHealth);
    }

    public void Explode()
    {
        GameObject destroyedObject;

        if (!_noParts)
        {
            if (_destroyedPrefab != null)
            {
                destroyedObject = Instantiate(_destroyedPrefab, transform.position, transform.rotation);
                destroyedObject.transform.localScale = transform.lossyScale;
            }
            else
                destroyedObject = gameObject;

            List<GameObject> parts = GetAllVisualParts(destroyedObject);


            foreach (GameObject part in parts)
            {
                Part PartComp = part.AddComponent<Part>();

                Vector3 force = (part.transform.position - transform.position).normalized * _partForceMulti;

                if (_rigidBody != null)
                    force += _rigidBody.velocity;

                PartComp.ExplodeForce = force;
            }
        }

        ExplodeAnimation();
        DamageSurroundings();

        if (_destroyedPrefab != null || _noParts)
            Destroy(gameObject);

        _exploded = true;

        Destroy(this);
    }

    private void DamageSurroundings()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionSize * 2);

        foreach (Collider collider in colliders)
        {
            DestroyableObject dObject = collider.GetComponent<DestroyableObject>();

            if (dObject == null)
                continue;

            float damage = Mathf.Clamp(_explosionSize - Vector3.Distance(transform.position, dObject.transform.position), 0, Mathf.Infinity);
            dObject.Damage(damage * 10);
        }
    }

    private void ExplodeAnimation()
    {
        GameObject explosion = Instantiate(ServiceLocator.Locate<Effects>().Explosion, transform.position, transform.rotation);
        explosion.GetComponent<ParticleSystem>().Play();
    }

    private List<GameObject> GetAllVisualParts(GameObject gameObject, bool onlyVisualObjects = true)
    {
        List<GameObject> objects = new List<GameObject>();
        objects.Add(gameObject);
        gameObject.transform.parent = null;

        for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
            objects.AddRange(GetAllVisualParts(gameObject.transform.GetChild(i).gameObject, false));

        if (!onlyVisualObjects)
            return objects;

        for (int i = objects.Count - 1; i >= 0; i--)
            if (objects[i].GetComponent<MeshRenderer>() == null)
            {
                GameObject obj = objects[i];
                objects.RemoveAt(i);

                Destroy(obj);
            }

        return objects;
    }

    public bool Registered
    {
        get { return _registered;  }
        set { _registered = value; }
    }

    public float Health
    {
        get { return _health; }
    }

    private void OnDestroy()
    {
        if (ServiceLocator.Locate<Menu>().GameState == GameState.Menu || _quitting)
            return;

        foreach (MonoBehaviour behaviour in _removedBehaviours)
            Destroy(behaviour);

        if (_registered)
            ServiceLocator.Locate<Score>().AddPoint();
    }

    private void OnApplicationQuit()
    {
        _quitting = true;
    }
}
