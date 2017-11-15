using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSafe
{
    private static readonly ObjectSafe _instance = new ObjectSafe();

    public delegate void OnGameStart(ObjectSafe safe);

    public static event OnGameStart onGameStart;
    public static event OnAction onSpawn;

    private List<GameObject> _currentObjects;
    private List<GameObject> _safedObjects;

    private ObjectSafe()
    {
        ServiceLocator.Provide(this);

        _currentObjects = new List<GameObject>();
        _safedObjects = new List<GameObject>();

        onGameStart = null;

        Menu.OnPlay += Spawn;
        Menu.OnStop += Delete;
    }

	public void Start ()
    {
        foreach (OnGameStart start in onGameStart.GetInvocationList())
        {
            try { start(this); }
            catch { onGameStart -= start; }
        }
    }

    public void Safe(GameObject original)
    {
        _safedObjects.Add(original);
        original.SetActive(false);
    }

    public void Delete()
    {
        for (int i = _currentObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = _currentObjects[i];
            _currentObjects.RemoveAt(i);

            if (obj != null)
                Object.Destroy(obj);
        }
    }

    public void Spawn()
    {
        foreach (GameObject original in _safedObjects)
        {
            GameObject copy = Object.Instantiate(original);
            _currentObjects.Add(copy);
            copy.SetActive(true);

            var scoreComponent = original.GetComponent<DestroyableObject>();

            if (scoreComponent != null && scoreComponent.Registered)
                copy.GetComponent<DestroyableObject>().Registered = true;
        }
        onSpawn();
    }

    public void AddTemporaryObject(GameObject obj)
    {
        _currentObjects.Add(obj);
    }
}
