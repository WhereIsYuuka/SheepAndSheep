using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Type[] managers = new Type[]
    {
        typeof(AudioManager),

    };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            AddManagersIfMissing();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void AddManagersIfMissing()
    {
        foreach (var managerType in managers)
        {
            if(GetComponent(managerType) == null)
            {
                gameObject.AddComponent(managerType);
            }
        }
    }

}