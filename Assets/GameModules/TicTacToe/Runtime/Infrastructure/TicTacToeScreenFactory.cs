using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameModules.TicTacToe.Config;
using GameModules.TicTacToe.Presentation;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using Object = UnityEngine.Object;

namespace GameModules.TicTacToe.Infrastructure
{
    /// <summary>
    /// Загружает экран через Addressables и инжектит зависимости в созданный объект.
    /// </summary>
    public sealed class TicTacToeScreenFactory : ITicTacToeScreenFactory
    {
        private readonly IInstantiator _instantiator;
        private readonly TicTacToeModuleSettings _moduleSettings;

        private AsyncOperationHandle<GameObject>? _preloadedHandle;

        public TicTacToeScreenFactory(
            IInstantiator instantiator,
            TicTacToeModuleSettings moduleSettings)
        {
            _instantiator = instantiator;
            _moduleSettings = moduleSettings;
        }

        public async UniTask PreloadAsync(CancellationToken cancellationToken = default)
        {
            if (!CanUseAddressableScreen())
            {
                return;
            }

            if (_preloadedHandle.HasValue && _preloadedHandle.Value.IsValid())
            {
                return;
            }

            var assetReference = GetRequiredScreenReference();
            var handle = assetReference.LoadAssetAsync<GameObject>();
            _preloadedHandle = handle;

            try
            {
                await handle.ToUniTask(cancellationToken: cancellationToken);
            }
            catch
            {
                if (_preloadedHandle.HasValue && _preloadedHandle.Value.IsValid())
                {
                    Addressables.Release(_preloadedHandle.Value);
                }

                _preloadedHandle = null;
                throw;
            }
        }

        public async UniTask<TicTacToeScreenInstance> CreateAsync(
            Transform parent,
            CancellationToken cancellationToken = default)
        {
            if (!CanUseAddressableScreen())
            {
                return CreateRuntimeScreen(parent);
            }

            var usePreloadedHandle = _preloadedHandle.HasValue && _preloadedHandle.Value.IsValid();
            var assetHandle = usePreloadedHandle
                ? _preloadedHandle.Value
                : await GetOrLoadHandleAsync(cancellationToken);

            if (!assetHandle.IsDone)
            {
                await assetHandle.ToUniTask(cancellationToken: cancellationToken);
            }

            var prefab = assetHandle.Result;

            if (prefab == null)
            {
                throw new InvalidOperationException("TicTacToe screen prefab is not loaded.");
            }

            var releaseAssetHandle = !usePreloadedHandle;
            var localHandle = releaseAssetHandle ? assetHandle : (AsyncOperationHandle<GameObject>?)null;

            try
            {
                var instance = parent != null
                    ? _instantiator.InstantiatePrefab(prefab, parent)
                    : _instantiator.InstantiatePrefab(prefab);
                var view = instance.GetComponent<TicTacToeScreenView>();

                if (view == null)
                {
                    Object.Destroy(instance);
                    throw new InvalidOperationException("TicTacToeScreenView was not found on screen prefab.");
                }

                NormalizeInstanceRect(instance, parent);
                view.PrepareForEmbedding();

                return new TicTacToeScreenInstance(instance, view, localHandle, releaseAssetHandle);
            }
            catch
            {
                if (releaseAssetHandle && localHandle.HasValue && localHandle.Value.IsValid())
                {
                    Addressables.Release(localHandle.Value);
                }

                throw;
            }
        }

        public void ReleasePreloaded()
        {
            if (_preloadedHandle.HasValue && _preloadedHandle.Value.IsValid())
            {
                Addressables.Release(_preloadedHandle.Value);
                _preloadedHandle = null;
            }
        }

        private async UniTask<AsyncOperationHandle<GameObject>> GetOrLoadHandleAsync(
            CancellationToken cancellationToken)
        {
            if (_preloadedHandle.HasValue && _preloadedHandle.Value.IsValid())
            {
                return _preloadedHandle.Value;
            }

            var assetReference = GetRequiredScreenReference();
            var handle = assetReference.LoadAssetAsync<GameObject>();
            await handle.ToUniTask(cancellationToken: cancellationToken);
            return handle;
        }

        private TicTacToeScreenInstance CreateRuntimeScreen(Transform parent)
        {
            var rootObject = new GameObject("TicTacToeScreen", typeof(RectTransform));
            rootObject.SetActive(false);

            if (parent != null)
            {
                rootObject.transform.SetParent(parent, false);
            }

            _instantiator.InstantiateComponent<TicTacToeScreenView>(rootObject);
            rootObject.SetActive(true);

            var view = rootObject.GetComponent<TicTacToeScreenView>();
            NormalizeInstanceRect(rootObject, parent);
            view.PrepareForEmbedding();
            return new TicTacToeScreenInstance(rootObject, view, null, false);
        }

        private bool CanUseAddressableScreen()
        {
            return _moduleSettings != null
                && _moduleSettings.ScreenPrefabReference != null
                && _moduleSettings.ScreenPrefabReference.RuntimeKeyIsValid();
        }

        private AssetReferenceGameObject GetRequiredScreenReference()
        {
            if (!CanUseAddressableScreen())
            {
                throw new InvalidOperationException("TicTacToe screen prefab reference is not configured.");
            }

            return _moduleSettings.ScreenPrefabReference;
        }

        private static void NormalizeInstanceRect(GameObject instance, Transform parent)
        {
            if (instance == null)
            {
                return;
            }

            var rectTransform = instance.transform as RectTransform;

            if (rectTransform == null)
            {
                return;
            }

            if (parent != null)
            {
                rectTransform.SetParent(parent, false);
            }

            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.identity;
        }
    }
}
