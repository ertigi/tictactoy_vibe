using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameModules.TicTacToe.Config
{
    /// <summary>
    /// Корневой конфиг модуля для Zenject installer и runtime-сервисов.
    /// </summary>
    [CreateAssetMenu(
        fileName = "TicTacToeModuleSettings",
        menuName = "GameModules/TicTacToe/Module Settings")]
    public sealed class TicTacToeModuleSettings : ScriptableObject
    {
        [SerializeField]
        private AssetReferenceGameObject screenPrefabReference;

        [SerializeField]
        [Min(0f)]
        private float defaultBotTurnDelaySeconds = 0.35f;

        [SerializeField]
        [Min(0f)]
        private float resultAutoCloseDelaySeconds = 1.5f;

        [SerializeField]
        private TicTacToeRewardSettings rewardSettings = new();

        public AssetReferenceGameObject ScreenPrefabReference => screenPrefabReference;

        public float DefaultBotTurnDelaySeconds => defaultBotTurnDelaySeconds;

        public float ResultAutoCloseDelaySeconds => resultAutoCloseDelaySeconds;

        public TicTacToeRewardSettings RewardSettings => rewardSettings;

#if UNITY_EDITOR
        public void SetScreenPrefabReference(string assetGuid)
        {
            screenPrefabReference = string.IsNullOrWhiteSpace(assetGuid)
                ? null
                : new AssetReferenceGameObject(assetGuid);
        }
#endif
    }
}
