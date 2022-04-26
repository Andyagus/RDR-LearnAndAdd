using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("SINGLETON AWAKE");
    }

    public void SayHi()
    {
        Debug.Log("HI I AM SINGLETON");
    }

    public static T instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindOrCreateInstance();
            }

            Debug.Log("FOUND INSTANCE");
            return _instance;
        }
    }

    private static T _instance;

    private static T FindOrCreateInstance()
    {

        var instance = GameObject.FindObjectOfType<T>();
        if (instance)
        {
            return instance;
        }

        var name = typeof(T).Name + "Singleton";
        var containerGameObject = new GameObject(name);

        var singletonComponent = containerGameObject.AddComponent<T>();

        return singletonComponent;
    }
}

  
