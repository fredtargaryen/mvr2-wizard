#if MVR_HDRP
using MiddleVR.Unity.HDRP;
#endif
#if MVR_URP
using MiddleVR.Unity.URP;
#endif
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class MiddleVRSetupWizard : EditorWindow
{
    private static ListRequest packageListRequest;
    private static bool waitingForList;
    private static bool foundMVR;
    private static bool foundHDRP;
    private static bool foundMVRHDRP;
    private static bool foundURP;
    private static bool foundMVRURP;
    private static bool shouldRepaint;
    private static GUIStyle wrapStyle;

    [MenuItem("MiddleVR/Setup Wizard")]
    static void Init()
    {
        MiddleVRSetupWizard helperWindow = GetWindow<MiddleVRSetupWizard>();
        helperWindow.Show();
        shouldRepaint = false;
        Refresh();
    }

    static void Refresh() 
    {
        waitingForList = true;
        shouldRepaint = true;
        // Get a list of all packages installed in the project and update the window when received
        packageListRequest = Client.List(true, false);
        EditorApplication.update += OnPackageListReceived;
    }

    private void OnGUI()
    {
        // Enables text wrapping for LabelFields when used
        if (wrapStyle == null)
        {
            wrapStyle = EditorStyles.label;
            wrapStyle.wordWrap = true;
        }
        foundMVR = false;
        foundHDRP = false;
        foundMVRHDRP = false;
        foundURP = false;
        foundMVRURP = false;
        if (packageListRequest != null && packageListRequest.IsCompleted)
        {
            if (packageListRequest.Status == StatusCode.Success)
            {
                // Check the names in the package list to see what's been installed
                foreach (var package in packageListRequest.Result)
                {
                    if (package.name.Equals("com.middlevr")) foundMVR = true;
                    else if (package.name.Equals("com.unity.render-pipelines.high-definition")) foundHDRP = true;
                    else if (package.name.Equals("com.middlevr.hdrp")) foundMVRHDRP = true;
                    else if (package.name.Equals("com.unity.render-pipelines.universal")) foundURP = true;
                    else if (package.name.Equals("com.middlevr.urp")) foundMVRURP = true;
                }
                bool installedInProject = (foundHDRP && foundMVR && foundMVRHDRP) ||
                        (foundURP && foundMVR && foundMVRURP) ||
                        (!foundHDRP && !foundURP && foundMVR);
                if (installedInProject)
                {
                    EditorGUILayout.HelpBox("MiddleVR has been successfully installed in your project.", MessageType.Info);

                    // Installing MiddleVR in the Scene
                    bool installedInScene = false;
#if MVR
                    // Ensure there is one MVRManager in the Scene
                    MVRManagerScript[] managers = Object.FindObjectsOfType<MVRManagerScript>(true);
                    if(managers.Length > 1)
                    {
                        EditorGUILayout.HelpBox("There are too many MiddleVR Managers in your Scene.", MessageType.Error);
                        if(GUILayout.Button("Click to Fix"))
                        {
                            for(int i = 1; i < managers.Length; i++)
                            {
                                Destroy(managers[i].gameObject);
                            }
                        }
                    }
                    else if(managers.Length == 0)
                    {
                        EditorGUILayout.HelpBox("A MiddleVR Manager is needed in your Scene for MiddleVR to work.", MessageType.Error);
                        if(GUILayout.Button("Click to Fix"))
                        {
                            PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.middlevr/MVRManager.prefab"));
                        }
                    }
                    else
                    {
                        MVRManagerScript manager = managers[0];
                        if(foundMVRHDRP)
                        {
#if MVR_HDRP
                            // Ensure there is one MVR_HDRP prefab in the Scene
                            HDRPSettings[] settingsList = Object.FindObjectsOfType<HDRPSettings>(true);
                            if(settingsList.Length > 1)
                            {
                                EditorGUILayout.HelpBox("There are too many MiddleVR HDRP prefabs in your Scene.", MessageType.Error);
                                if(GUILayout.Button("Click to Fix"))
                                {
                                    for(int i = 1; i < settingsList.Length; i++)
                                    {
                                        Destroy(settingsList[i].gameObject);
                                    }
                                }
                            }
                            else if(settingsList.Length == 0)
                            {
                                EditorGUILayout.HelpBox("A MiddleVR HDRP prefab is needed in your Scene for MiddleVR to work in HDRP.", MessageType.Error);
                                if(GUILayout.Button("Click to Fix"))
                                {
                                    PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.middlevr.hdrp/MVR_HDRP.prefab"));
                                }
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("MiddleVR has been successfully installed in your Scene.", MessageType.Info);
                                installedInScene = true;
                            }
#endif
                        }
                        else if(foundMVRURP)
                        {
#if MVR_URP
                            // Ensure there is one MVR_URP prefab in the Scene
                            URPSettings[] settingsList = Object.FindObjectsOfType<URPSettings>(true);
                            if (settingsList.Length > 1)
                            {
                                EditorGUILayout.HelpBox("There are too many MiddleVR URP prefabs in your Scene.", MessageType.Error);
                                if (GUILayout.Button("Click to Fix"))
                                {
                                    for (int i = 1; i < settingsList.Length; i++)
                                    {
                                        Destroy(settingsList[i].gameObject);
                                    }
                                }
                            }
                            else if (settingsList.Length == 0)
                            {
                                EditorGUILayout.HelpBox("A MiddleVR URP prefab is needed in your Scene for MiddleVR to work in URP.", MessageType.Error);
                                if (GUILayout.Button("Click to Fix"))
                                {
                                    PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.middlevr.urp/MVR_URP.prefab"));
                                }
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("MiddleVR has been successfully installed in your Scene.", MessageType.Info);
                                installedInScene = true;
                            }
#endif
                        }
                        // If you have your own custom SRP you may need to implement your own checking for extra GameObjects needed for MiddleVR
                        else
                        {
                            //Built-in pipeline only needs the MVRManager
                            EditorGUILayout.HelpBox("MiddleVR has been successfully installed in your Scene.", MessageType.Info);
                            installedInScene = true;
                        }
                    }
#endif
                    if (installedInScene) this.DrawCustomWizardOptions();
                }
                else
                {
                    EditorGUILayout.HelpBox("Please follow these steps to install MiddleVR in your project.", MessageType.Info);
                    if (foundMVR)
                    {
                        if (foundHDRP)
                        {
                            EditorGUILayout.HelpBox(BuildInstallRequestString("HDRP", "com.middlevr.hdrp.tgz"), MessageType.Warning);
                        }
                        if (foundURP)
                        {
                            EditorGUILayout.HelpBox(BuildInstallRequestString("URP", "com.middlevr.urp.tgz"), MessageType.Warning);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox(BuildInstallRequestString("standard", "com.middlevr.tgz"), MessageType.Warning);
                    }
                }
            }
            else if (packageListRequest.Status >= StatusCode.Failure)
            {
                Debug.Log(packageListRequest.Error.message);
            }
            if (shouldRepaint)
            {
                Repaint();
                shouldRepaint = false;
            }
        }
        else
        {
            EditorGUILayout.LabelField("Waiting for package list... (click Refresh if you have waited longer than a few seconds)", wrapStyle);
        }
        
        if (!waitingForList && GUILayout.Button("Refresh")) {
            Refresh();
        }

        if (GUILayout.Button("Remove Wizard From Project"))
        {
            EditorGUILayout.LabelField("Bye!");
            this.Close();
            AssetDatabase.MoveAssetToTrash("Assets/MiddleVR Setup Wizard/");
        }
    }

    private void DrawCustomWizardOptions()
    {
        // Edit this function to provide further options related to your specific MiddleVR configuration.
        // These options will only display when MiddleVR 2 is correctly installed in your project and the current Scene.
    }

    private static void OnPackageListReceived()
    {
        waitingForList = false;
        EditorApplication.update -= OnPackageListReceived;
    }

    private static string BuildInstallRequestString(string packageType, string tgzName)
    {
        return $"The MiddleVR {packageType} package is required. Please install it by going to Window > Package Manager > + > Add package from tarball... and selecting your {tgzName} file.";
    }
}
