using UnityEngine;

public abstract class SingletonObject : MonoBehaviour {
  public virtual void OnSingletonDestroyed() {

  }
}

public abstract class SingletonObject<T> : SingletonObject where T : SingletonObject {
  static T _instance;
  static protected string _resourcePath;

  protected void Awake() {
    // destroy old singleton
    DestroySingleton();

    // assign this as instance
    _instance = (T)(object)this;
  }

  public static T Instance {
    get {
      InstantiateSingleton();
      return _instance;
    }
  }

  public static void DestroySingleton() {
    if (_instance) {
      _instance.OnSingletonDestroyed();
      Destroy(_instance.gameObject);
    }

    _instance = null;
  }

  public static void InstantiateSingleton() {
    if (!_instance) {
      Object obj = FindObjectOfType(typeof(T));

      if (obj) {
        _instance = (T)obj;
      }
      else {
        var resource = _resourcePath == null ? typeof(T).Name : _resourcePath;
        var resourceObj = Resources.Load(resource);

        if (resourceObj) {
          obj = Instantiate(resourceObj);

          if (obj) {
            // make sure this survives scene loading
            DontDestroyOnLoad(obj);

            // grab component
            _instance = ((GameObject)obj).GetComponent<T>();
          }
          else {
            Debug.LogError(string.Format("could not load auto instance of {0}", typeof(T)));
          }
        }
      }
    }
  }
}
