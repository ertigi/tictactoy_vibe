#if UNITY_EDITOR
using System;
using System.IO;
using GameModules.TicTacToe.Config;
using GameModules.TicTacToe.Presentation;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.UI;

namespace GameModules.TicTacToe.Editor
{
    /// <summary>
    /// Создает базовый prefab и config для addressable-подключения модуля.
    /// </summary>
    public static class TicTacToeAddressablesSetupUtility
    {
        private const string RootFolderPath = "Assets/GameModules/TicTacToe";
        private const string PrefabsFolderPath = RootFolderPath + "/Content/Prefabs";
        private const string SettingsFolderPath = RootFolderPath + "/Content/ScriptableObjects";
        private const string ScreenPrefabPath = PrefabsFolderPath + "/TicTacToeScreen.prefab";
        private const string ModuleSettingsPath = SettingsFolderPath + "/TicTacToeModuleSettings.asset";
        private const string AddressablesGroupName = "GameModules.TicTacToe";
        private const string ScreenPrefabAddress = "GameModules/TicTacToe/Screen";

        [MenuItem("Tools/GameModules/TicTacToe/Setup Addressables Content")]
        public static void SetupAddressablesContent()
        {
            EnsureFolders();

            var prefab = EnsureScreenPrefab();
            var settings = EnsureModuleSettings();
            EnsureAddressableEntry(prefab);
            AssignScreenReference(settings, ScreenPrefabPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = settings;
            Debug.Log("TicTacToe addressable content is configured.");
        }

        private static void EnsureFolders()
        {
            Directory.CreateDirectory(PrefabsFolderPath);
            Directory.CreateDirectory(SettingsFolderPath);
        }

        private static GameObject EnsureScreenPrefab()
        {
            if (File.Exists(ScreenPrefabPath))
            {
                var root = PrefabUtility.LoadPrefabContents(ScreenPrefabPath);

                try
                {
                    EnsurePrefabContent(root);
                    PrefabUtility.SaveAsPrefabAsset(root, ScreenPrefabPath);
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }

                return AssetDatabase.LoadAssetAtPath<GameObject>(ScreenPrefabPath);
            }

            var prefabRoot = new GameObject("TicTacToeScreen", typeof(RectTransform));

            try
            {
                EnsurePrefabContent(prefabRoot);
                return PrefabUtility.SaveAsPrefabAsset(prefabRoot, ScreenPrefabPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(prefabRoot);
            }
        }

        private static void EnsurePrefabContent(GameObject root)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            root.name = "TicTacToeScreen";

            if (root.GetComponent<RectTransform>() == null)
            {
                root.AddComponent<RectTransform>();
            }

            var view = root.GetComponent<TicTacToeScreenView>();

            if (view == null)
            {
                view = root.AddComponent<TicTacToeScreenView>();
            }

            // Если prefab еще пустой, собираем базовую визуальную иерархию автоматически.
            TicTacToeScreenRuntimeBuilder.Build(view);

            PrepareRootForEmbedding(root);
        }

        private static TicTacToeModuleSettings EnsureModuleSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<TicTacToeModuleSettings>(ModuleSettingsPath);

            if (settings != null)
            {
                return settings;
            }

            settings = ScriptableObject.CreateInstance<TicTacToeModuleSettings>();
            AssetDatabase.CreateAsset(settings, ModuleSettingsPath);
            EditorUtility.SetDirty(settings);
            return settings;
        }

        private static void EnsureAddressableEntry(GameObject prefab)
        {
            if (prefab == null)
            {
                throw new InvalidOperationException("TicTacToe screen prefab was not created.");
            }

            var addressableSettings = AddressableAssetSettingsDefaultObject.GetSettings(true);

            if (addressableSettings == null)
            {
                throw new InvalidOperationException("AddressableAssetSettings could not be created.");
            }

            var group = addressableSettings.FindGroup(AddressablesGroupName)
                ?? addressableSettings.CreateGroup(
                    AddressablesGroupName,
                    false,
                    false,
                    false,
                    null,
                    typeof(ContentUpdateGroupSchema),
                    typeof(BundledAssetGroupSchema));

            var prefabGuid = AssetDatabase.AssetPathToGUID(ScreenPrefabPath);
            var entry = addressableSettings.CreateOrMoveEntry(prefabGuid, group, false, false);
            entry.SetAddress(ScreenPrefabAddress, false);

            EditorUtility.SetDirty(group);
            EditorUtility.SetDirty(addressableSettings);
        }

        private static void AssignScreenReference(
            TicTacToeModuleSettings settings,
            string prefabPath)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var prefabGuid = AssetDatabase.AssetPathToGUID(prefabPath);
            settings.SetScreenPrefabReference(prefabGuid);
            EditorUtility.SetDirty(settings);
        }

        private static void PrepareRootForEmbedding(GameObject root)
        {
            var rectTransform = root.GetComponent<RectTransform>();

            if (rectTransform == null)
            {
                rectTransform = root.AddComponent<RectTransform>();
            }

            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.identity;

            DestroyImmediateIfExists(root.GetComponent<Canvas>());
            DestroyImmediateIfExists(root.GetComponent<CanvasScaler>());
            DestroyImmediateIfExists(root.GetComponent<GraphicRaycaster>());
        }

        private static void DestroyImmediateIfExists(Component component)
        {
            if (component != null)
            {
                UnityEngine.Object.DestroyImmediate(component);
            }
        }
    }
}
#endif
