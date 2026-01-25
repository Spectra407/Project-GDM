using UnityEngine;


// Use this for things that should reset every fight (ex: CombatManager, HandView)
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            // If the instance is null, we try to find it in the scene
            if (_instance == null)
            {
                _instance = Object.FindAnyObjectByType<T>();
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        // If an instance already exists and it's not THIS object, destroy this one
        if (_instance != null && _instance != this as T)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;
    }

    protected virtual void OnApplicationQuit()
    {
        _instance = null;
    }
}


// Use this for things that stay with Alice forever (ex: DeckManager)
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        // Run the base Singleton check first
        base.Awake();

        // If this object survived the Awake check, make it immortal
        if (Instance == this as T)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}