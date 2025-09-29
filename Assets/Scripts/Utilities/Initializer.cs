using UnityEngine;

public static class Initializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]


    public static void Execute()
    {
        Debug.Log("Loaded by the CoreSystems from the Initializer script");
        Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("PERSISTENT_CORE")));
    }
}