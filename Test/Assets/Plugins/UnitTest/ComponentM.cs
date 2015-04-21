using UnityEngine;
using System.Collections;
using System;

public class ComponentM : MonoBehaviour
{
    public Action action;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (action != null)
        {
            action();
            action = null;
        }
    }

    public static ComponentM AddComponentM(GameObject obj)
    {
        return obj.AddComponent<ComponentM>();
    }
}
