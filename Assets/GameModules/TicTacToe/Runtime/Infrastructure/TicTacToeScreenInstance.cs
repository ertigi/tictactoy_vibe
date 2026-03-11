using GameModules.TicTacToe.Presentation;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace GameModules.TicTacToe.Infrastructure
{
    /// <summary>
    /// Обертка над созданным экраном и его addressable handle.
    /// </summary>
    public sealed class TicTacToeScreenInstance
    {
        private readonly AsyncOperationHandle<GameObject>? _assetHandle;
        private readonly bool _releaseAssetHandle;

        public TicTacToeScreenInstance(
            GameObject rootObject,
            TicTacToeScreenView view,
            AsyncOperationHandle<GameObject>? assetHandle,
            bool releaseAssetHandle)
        {
            RootObject = rootObject;
            View = view;
            _assetHandle = assetHandle;
            _releaseAssetHandle = releaseAssetHandle;
        }

        public GameObject RootObject { get; }

        public TicTacToeScreenView View { get; }

        public void Release()
        {
            if (RootObject != null)
            {
                Object.Destroy(RootObject);
            }

            if (_releaseAssetHandle && _assetHandle.HasValue && _assetHandle.Value.IsValid())
            {
                UnityEngine.AddressableAssets.Addressables.Release(_assetHandle.Value);
            }
        }
    }
}
