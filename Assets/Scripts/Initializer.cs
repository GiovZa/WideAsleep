using UnityEngine;
using UnityEngine.SceneManagement;

public static class Initializer
{
    private const string MAIN_MENU_SCENE   = "Main Menu";          // 你的主菜单场景名
    private const string PERSISTENT_PREFAB = "PERSISTENT_OBJECTS"; // Resources 下的 prefab 名

    private static GameObject s_persistentRoot;   // 运行时实例（根），便于销毁
    private static bool       s_bootstrapped;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] // 首个场景加载完成后触发
    private static void Bootstrap()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;                                   // 场景完成加载时回调（在 Awake/OnEnable 之后、Start 之前）
        s_bootstrapped = true;
        SyncWith(SceneManager.GetActiveScene());                                     // 根据首个场景决定是否生成
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) => SyncWith(scene);

    private static void SyncWith(Scene scene)
    {
        bool isMenu = scene.name == MAIN_MENU_SCENE;                                 // 判断是否主菜单

        if (isMenu)
        {
            // 进入主菜单：确保没有 Persistent（按你的要求“MainMenu 就不加载”）
            DespawnPersistent();
        }
        else
        {
            // 进入非主菜单：若未生成则加载并标记为跨场景保留
            if (s_persistentRoot == null)
            {
                var prefab = Resources.Load<GameObject>(PERSISTENT_PREFAB);
                if (prefab == null) { Debug.LogError($"[Initializer] Missing Resources/{PERSISTENT_PREFAB}"); return; }

                s_persistentRoot = Object.Instantiate(prefab);
                Object.DontDestroyOnLoad(s_persistentRoot);                          // 仅对根对象生效：其整个 Transform 子树都会被保留。 :contentReference[oaicite:0]{index=0}
            }
        }
    }

    private static void DespawnPersistent()
    {
        if (s_persistentRoot != null)
        {
            Object.Destroy(s_persistentRoot);
            s_persistentRoot = null;
        }
    }
}