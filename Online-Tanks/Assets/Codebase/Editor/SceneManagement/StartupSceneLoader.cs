using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Codebase.Editor.SceneManagement
{
    [InitializeOnLoad]
    public class StartupSceneLoader
    {
        static StartupSceneLoader()
        {
            EditorApplication.playModeStateChanged += LoadStartupScene;
        }

        private static void LoadStartupScene(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.ExitingEditMode)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            }

            if (stateChange == PlayModeStateChange.EnteredPlayMode)
            {
                if (SceneManager.GetActiveScene().buildIndex != 0)
                    SceneManager.LoadScene(0);
            }
        }
    }
}