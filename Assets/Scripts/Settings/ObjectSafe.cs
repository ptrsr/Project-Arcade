using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSafe
{
    public delegate void OnGameStart(ObjectSafe safe);
    public delegate void OnSafeAction();

    private static readonly ObjectSafe _instance = new ObjectSafe();

    public static event OnGameStart onGameStart;
    public static event OnSafeAction onSpawn;

    private List<GameObject> _currentObjects;
    private List<GameObject> _safedObjects;

    private ObjectSafe()
    {
        ServiceLocator.Provide(this);

        _currentObjects = new List<GameObject>();
        _safedObjects = new List<GameObject>();

        onGameStart = null;
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
        Debug.Log("Deleting objects: " + _currentObjects.Count);

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
        }
        onSpawn();
    }

    public void AddTemporaryObject(GameObject obj)
    {
        _currentObjects.Add(obj);
    }
}
