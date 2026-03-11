using System;
using GameModules.TicTacToe.Api;
using GameModules.TicTacToe.Application;
using GameModules.TicTacToe.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace GameModules.TicTacToe.Presentation
{
    /// <summary>
    /// Корневой экран миниигры.
    /// </summary>
    public sealed class TicTacToeScreenView : MonoBehaviour
    {
        [SerializeField]
        private TicTacToeCellView[] cellViews = Array.Empty<TicTacToeCellView>();

        [SerializeField]
        private Button closeButton;

        [SerializeField]
        private Button restartButton;

        [SerializeField]
        private Text statusLabel;

        [SerializeField]
        private Text rewardLabel;

        [SerializeField]
        private int layoutVersion;

        public event Action<int> CellClicked;

        public event Action CloseRequested;

        public event Action RestartRequested;

        private void Awake()
        {
            PrepareForEmbedding();
            EnsureViewHierarchy();
            for (var i = 0; i < cellViews.Length; i++)
            {
                if (cellViews[i] != null)
                {
                    cellViews[i].Clicked += HandleCellClicked;
                }
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(HandleCloseClicked);
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(HandleRestartClicked);
            }
        }

        private void OnDestroy()
        {
            for (var i = 0; i < cellViews.Length; i++)
            {
                if (cellViews[i] != null)
                {
                    cellViews[i].Clicked -= HandleCellClicked;
                }
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(HandleCloseClicked);
            }

            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(HandleRestartClicked);
            }
        }

        public void Render(TicTacToeSessionSnapshot snapshot)
        {
            for (var i = 0; i < cellViews.Length; i++)
            {
                if (cellViews[i] == null || i >= snapshot.Cells.Count)
                {
                    continue;
                }

                cellViews[i].SetMark(snapshot.Cells[i]);
                cellViews[i].SetHighlight(Contains(snapshot.WinningCells, BoardPosition.FromIndex(i)));
                cellViews[i].SetInteractable(!snapshot.IsInputLocked && !snapshot.IsFinished && snapshot.Cells[i] == BoardMark.None);
            }

            if (statusLabel != null)
            {
                statusLabel.text = BuildStatusText(snapshot);
            }
        }

        public void ConfigureForSession(bool allowRestart)
        {
            SetVisible(true);

            if (rewardLabel != null)
            {
                rewardLabel.text = string.Empty;
            }

            if (restartButton != null)
            {
                restartButton.gameObject.SetActive(false);
                restartButton.interactable = allowRestart;
            }

            if (closeButton != null)
            {
                closeButton.gameObject.SetActive(true);
            }
        }

        public void RenderResult(TicTacToeResult result, bool canRestart)
        {
            if (statusLabel != null)
            {
                statusLabel.text = BuildResultText(result.Outcome);
            }

            if (rewardLabel != null)
            {
                rewardLabel.text = BuildRewardText(result.Reward);
            }

            if (restartButton != null)
            {
                restartButton.gameObject.SetActive(canRestart);
                restartButton.interactable = canRestart;
            }

            if (closeButton != null)
            {
                closeButton.gameObject.SetActive(true);
                closeButton.interactable = true;
            }
        }

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        internal void PrepareForEmbedding()
        {
            NormalizeRootRectTransform();
            RemoveStandaloneCanvasIfEmbedded();
        }

        private void HandleCellClicked(int index)
        {
            CellClicked?.Invoke(index);
        }

        private void HandleCloseClicked()
        {
            CloseRequested?.Invoke();
        }

        private void HandleRestartClicked()
        {
            RestartRequested?.Invoke();
        }

        internal void BindRuntimeReferences(
            TicTacToeCellView[] generatedCellViews,
            Button generatedCloseButton,
            Button generatedRestartButton,
            Text generatedStatusLabel,
            Text generatedRewardLabel)
        {
            cellViews = generatedCellViews ?? Array.Empty<TicTacToeCellView>();
            closeButton = generatedCloseButton;
            restartButton = generatedRestartButton;
            statusLabel = generatedStatusLabel;
            rewardLabel = generatedRewardLabel;
        }

        internal void SetLayoutVersion(int version)
        {
            layoutVersion = version;
        }

        private static bool Contains(
            System.Collections.Generic.IReadOnlyList<BoardPosition> positions,
            BoardPosition target)
        {
            for (var i = 0; i < positions.Count; i++)
            {
                if (positions[i].Equals(target))
                {
                    return true;
                }
            }

            return false;
        }

        private static string BuildStatusText(TicTacToeSessionSnapshot snapshot)
        {
            if (snapshot.IsFinished)
            {
                switch (snapshot.Outcome)
                {
                    case TicTacToeRoundOutcome.PlayerWin:
                        return "Victory";
                    case TicTacToeRoundOutcome.BotWin:
                        return "Defeat";
                    case TicTacToeRoundOutcome.Draw:
                        return "Draw";
                }
            }

            if (!snapshot.IsFinished && snapshot.CurrentTurn == BoardMark.None)
            {
                return string.Empty;
            }

            return snapshot.CurrentTurn == BoardMark.Player ? "Your turn" : "Bot turn";
        }

        private static string BuildResultText(TicTacToeOutcome outcome)
        {
            switch (outcome)
            {
                case TicTacToeOutcome.PlayerWin:
                    return "Victory";
                case TicTacToeOutcome.BotWin:
                    return "Defeat";
                case TicTacToeOutcome.Draw:
                    return "Draw";
                case TicTacToeOutcome.Cancelled:
                    return "Cancelled";
                default:
                    return string.Empty;
            }
        }

        private static string BuildRewardText(TicTacToeRewardData reward)
        {
            if (reward == null || !reward.HasAnyReward)
            {
                return string.Empty;
            }

            if (reward.HasCurrency && reward.HasDrop)
            {
                return $"+{reward.TotalAmount} {reward.CurrencyId}, {reward.Drop.DisplayName} ({reward.Drop.Rarity})";
            }

            if (reward.HasCurrency)
            {
                return $"+{reward.TotalAmount} {reward.CurrencyId}";
            }

            return $"{reward.Drop.DisplayName} ({reward.Drop.Rarity})";
        }

        private void EnsureViewHierarchy()
        {
            if (HasBoundReferences() && layoutVersion == TicTacToeScreenRuntimeBuilder.CurrentLayoutVersion)
            {
                return;
            }

            TicTacToeScreenRuntimeBuilder.Build(this);
        }

        private bool HasBoundReferences()
        {
            return cellViews != null
                && cellViews.Length == 9
                && AreCellsBound()
                && closeButton != null
                && restartButton != null
                && statusLabel != null
                && rewardLabel != null;
        }

        private bool AreCellsBound()
        {
            for (var i = 0; i < cellViews.Length; i++)
            {
                if (cellViews[i] == null)
                {
                    return false;
                }
            }

            return true;
        }

        private void NormalizeRootRectTransform()
        {
            var rectTransform = GetComponent<RectTransform>();

            if (rectTransform == null)
            {
                rectTransform = gameObject.AddComponent<RectTransform>();
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

        private void RemoveStandaloneCanvasIfEmbedded()
        {
            var parentCanvas = transform.parent != null
                ? transform.parent.GetComponentInParent<Canvas>()
                : null;

            if (parentCanvas == null)
            {
                return;
            }

            // Убираем собственный canvas у вложенного экрана, чтобы он жил внутри хост-контейнера.
            DestroyComponentIfExists(GetComponent<Canvas>());
            DestroyComponentIfExists(GetComponent<CanvasScaler>());
            DestroyComponentIfExists(GetComponent<GraphicRaycaster>());
        }

        private void DestroyComponentIfExists(Component component)
        {
            if (component == null)
            {
                return;
            }

            if (UnityEngine.Application.isPlaying)
            {
                Destroy(component);
            }
            else
            {
                DestroyImmediate(component);
            }
        }
    }
}
