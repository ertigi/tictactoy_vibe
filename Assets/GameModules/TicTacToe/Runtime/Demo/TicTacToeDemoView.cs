using System;
using GameModules.TicTacToe.Api;
using UnityEngine;
using UnityEngine.UI;

namespace GameModules.TicTacToe.Demo
{
    /// <summary>
    /// Внешний demo-экран, который показывает запуск модуля и полученный наружу результат.
    /// </summary>
    public sealed class TicTacToeDemoView : MonoBehaviour
    {
        [SerializeField]
        private Button launchButton;

        [SerializeField]
        private Button replayButton;

        [SerializeField]
        private Text stateLabel;

        [SerializeField]
        private Text outcomeLabel;

        [SerializeField]
        private Text rewardLabel;

        [SerializeField]
        private Text sessionLabel;

        [SerializeField]
        private Text hostPlaceholderLabel;

        [SerializeField]
        private RectTransform moduleHost;

        [SerializeField]
        private int layoutVersion;

        public event Action LaunchRequested;

        public event Action ReplayRequested;

        public RectTransform ModuleHost => moduleHost;

        private void Awake()
        {
            EnsureViewHierarchy();

            if (launchButton != null)
            {
                launchButton.onClick.AddListener(HandleLaunchClicked);
            }

            if (replayButton != null)
            {
                replayButton.onClick.AddListener(HandleReplayClicked);
            }
        }

        private void OnDestroy()
        {
            if (launchButton != null)
            {
                launchButton.onClick.RemoveListener(HandleLaunchClicked);
            }

            if (replayButton != null)
            {
                replayButton.onClick.RemoveListener(HandleReplayClicked);
            }
        }

        public void ShowWarmupState()
        {
            SetStateText("Preparing embedded module...");
            SetOutcomeText("Outcome: waiting for launch");
            SetRewardText("Reward: waiting for result");
            SetSessionText("Session: not started");
            SetControls(canLaunch: false, canReplay: false);
            SetHostPlaceholder(true, "Addressable UI is preloading...");
        }

        public void ShowReadyState()
        {
            SetStateText("Demo host is ready. Press Launch Mini-Game.");
            SetOutcomeText("Outcome: waiting for launch");
            SetRewardText("Reward: waiting for result");
            SetSessionText("Session: not started");
            SetControls(canLaunch: true, canReplay: false);
            SetHostPlaceholder(true, "Mini-game will be embedded here.");
        }

        public void ShowWarmupFailed(string message)
        {
            SetStateText($"Warmup failed. Launch will load on demand.\n{message}");
            SetOutcomeText("Outcome: waiting for launch");
            SetRewardText("Reward: waiting for result");
            SetSessionText("Session: not started");
            SetControls(canLaunch: true, canReplay: false);
            SetHostPlaceholder(true, "Mini-game will be loaded on first launch.");
        }

        public void ShowLaunchState()
        {
            SetStateText("External caller launched the mini-game.");
            SetOutcomeText("Outcome: match in progress");
            SetRewardText("Reward: pending");
            SetSessionText("Session: running");
            SetControls(canLaunch: false, canReplay: false);
            SetHostPlaceholder(false, string.Empty);
        }

        public void ShowResult(TicTacToeResult result)
        {
            if (result == null)
            {
                ShowError("Result is null.");
                return;
            }

            SetStateText("External caller received the match result.");
            SetOutcomeText(BuildOutcomeText(result));
            SetRewardText(BuildRewardText(result.Reward));
            SetSessionText($"Session: {result.SessionId} | Turns: {result.TurnsCount}");
            SetControls(canLaunch: true, canReplay: true);
            SetHostPlaceholder(true, "Mini-game closed. Press Play Again to launch a new session.");
        }

        public void ShowError(string message)
        {
            SetStateText($"Launch failed.\n{message}");
            SetControls(canLaunch: true, canReplay: false);
            SetHostPlaceholder(true, "Mini-game host is idle.");
        }

        internal void BindRuntimeReferences(
            Button generatedLaunchButton,
            Button generatedReplayButton,
            Text generatedStateLabel,
            Text generatedOutcomeLabel,
            Text generatedRewardLabel,
            Text generatedSessionLabel,
            Text generatedHostPlaceholderLabel,
            RectTransform generatedModuleHost)
        {
            launchButton = generatedLaunchButton;
            replayButton = generatedReplayButton;
            stateLabel = generatedStateLabel;
            outcomeLabel = generatedOutcomeLabel;
            rewardLabel = generatedRewardLabel;
            sessionLabel = generatedSessionLabel;
            hostPlaceholderLabel = generatedHostPlaceholderLabel;
            moduleHost = generatedModuleHost;
        }

        internal void SetLayoutVersion(int version)
        {
            layoutVersion = version;
        }

        private void HandleLaunchClicked()
        {
            LaunchRequested?.Invoke();
        }

        private void HandleReplayClicked()
        {
            ReplayRequested?.Invoke();
        }

        private void EnsureViewHierarchy()
        {
            if (HasBoundReferences() && layoutVersion == TicTacToeDemoRuntimeBuilder.CurrentLayoutVersion)
            {
                return;
            }

            TicTacToeDemoRuntimeBuilder.Build(this);
        }

        private bool HasBoundReferences()
        {
            return launchButton != null
                && replayButton != null
                && stateLabel != null
                && outcomeLabel != null
                && rewardLabel != null
                && sessionLabel != null
                && hostPlaceholderLabel != null
                && moduleHost != null;
        }

        private void SetControls(bool canLaunch, bool canReplay)
        {
            if (launchButton != null)
            {
                launchButton.interactable = canLaunch;
            }

            if (replayButton != null)
            {
                replayButton.interactable = canReplay;
            }
        }

        private void SetStateText(string value)
        {
            if (stateLabel != null)
            {
                stateLabel.text = value ?? string.Empty;
            }
        }

        private void SetOutcomeText(string value)
        {
            if (outcomeLabel != null)
            {
                outcomeLabel.text = value ?? string.Empty;
            }
        }

        private void SetRewardText(string value)
        {
            if (rewardLabel != null)
            {
                rewardLabel.text = value ?? string.Empty;
            }
        }

        private void SetSessionText(string value)
        {
            if (sessionLabel != null)
            {
                sessionLabel.text = value ?? string.Empty;
            }
        }

        private void SetHostPlaceholder(bool isVisible, string value)
        {
            if (hostPlaceholderLabel == null)
            {
                return;
            }

            hostPlaceholderLabel.gameObject.SetActive(isVisible);
            hostPlaceholderLabel.text = value ?? string.Empty;
        }

        private static string BuildOutcomeText(TicTacToeResult result)
        {
            switch (result.Outcome)
            {
                case TicTacToeOutcome.PlayerWin:
                    return "Outcome: Victory";
                case TicTacToeOutcome.BotWin:
                    return "Outcome: Defeat";
                case TicTacToeOutcome.Draw:
                    return "Outcome: Draw";
                case TicTacToeOutcome.Cancelled:
                    return "Outcome: Cancelled";
                default:
                    return "Outcome: None";
            }
        }

        private static string BuildRewardText(TicTacToeRewardData reward)
        {
            if (reward == null || !reward.HasAnyReward)
            {
                return "Reward: none";
            }

            if (reward.HasCurrency && reward.HasDrop)
            {
                return $"Reward: +{reward.TotalAmount} {reward.CurrencyId}, {reward.Drop.DisplayName} ({reward.Drop.Rarity})";
            }

            if (reward.HasCurrency)
            {
                return $"Reward: +{reward.TotalAmount} {reward.CurrencyId}";
            }

            return $"Reward: {reward.Drop.DisplayName} ({reward.Drop.Rarity})";
        }
    }
}
