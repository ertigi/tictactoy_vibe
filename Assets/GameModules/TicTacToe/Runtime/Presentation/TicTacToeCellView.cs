using System;
using GameModules.TicTacToe.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace GameModules.TicTacToe.Presentation
{
    /// <summary>
    /// View одной клетки. Реальную визуальную часть можно заменить без смены API.
    /// </summary>
    public sealed class TicTacToeCellView : MonoBehaviour
    {
        private static readonly Color EmptyColor = new(0.13f, 0.15f, 0.19f, 1f);
        private static readonly Color PlayerColor = new(0.80f, 0.28f, 0.17f, 1f);
        private static readonly Color BotColor = new(0.16f, 0.48f, 0.42f, 1f);
        private static readonly Color DisabledColor = new(0.22f, 0.24f, 0.28f, 1f);
        private static readonly Color EmptyTextColor = new(0.95f, 0.93f, 0.88f, 1f);

        [SerializeField]
        private int index;

        [SerializeField]
        private Button button;

        [SerializeField]
        private Text label;

        [SerializeField]
        private Graphic highlightGraphic;

        [SerializeField]
        private Image backgroundImage;

        private BoardMark _mark;
        private bool _isInteractable = true;
        private bool _isHighlighted;
        private bool _isButtonSubscribed;

        public event Action<int> Clicked;

        public int Index => index;

        private void Awake()
        {
            SubscribeToButton();
        }

        private void OnDestroy()
        {
            UnsubscribeFromButton();
        }

        public void BindRuntimeReferences(
            int cellIndex,
            Button targetButton,
            Text targetLabel,
            Graphic targetHighlightGraphic,
            Image targetBackgroundImage)
        {
            UnsubscribeFromButton();

            index = cellIndex;
            button = targetButton;
            label = targetLabel;
            highlightGraphic = targetHighlightGraphic;
            backgroundImage = targetBackgroundImage;

            SubscribeToButton();
            RefreshVisualState();
        }

        public void SetMark(BoardMark mark)
        {
            _mark = mark;

            if (label == null)
            {
                return;
            }

            switch (mark)
            {
                case BoardMark.Player:
                    label.text = "X";
                    break;
                case BoardMark.Bot:
                    label.text = "O";
                    break;
                default:
                    label.text = string.Empty;
                    break;
            }

            RefreshVisualState();
        }

        public void SetInteractable(bool isInteractable)
        {
            _isInteractable = isInteractable;

            if (button != null)
            {
                button.interactable = isInteractable;
            }

            RefreshVisualState();
        }

        public void SetHighlight(bool isHighlighted)
        {
            _isHighlighted = isHighlighted;

            if (highlightGraphic != null)
            {
                highlightGraphic.enabled = isHighlighted;
            }

            RefreshVisualState();
        }

        private void HandleClick()
        {
            Clicked?.Invoke(index);
        }

        private void SubscribeToButton()
        {
            if (button == null || _isButtonSubscribed)
            {
                return;
            }

            button.onClick.AddListener(HandleClick);
            _isButtonSubscribed = true;
        }

        private void UnsubscribeFromButton()
        {
            if (button == null || !_isButtonSubscribed)
            {
                return;
            }

            button.onClick.RemoveListener(HandleClick);
            _isButtonSubscribed = false;
        }

        private void RefreshVisualState()
        {
            if (backgroundImage != null)
            {
                if (_mark == BoardMark.Player)
                {
                    backgroundImage.color = PlayerColor;
                }
                else if (_mark == BoardMark.Bot)
                {
                    backgroundImage.color = BotColor;
                }
                else
                {
                    backgroundImage.color = _isInteractable ? EmptyColor : DisabledColor;
                }
            }

            if (label != null)
            {
                label.color = _mark == BoardMark.None ? EmptyTextColor : Color.white;
            }

            if (highlightGraphic != null)
            {
                highlightGraphic.enabled = _isHighlighted;
            }
        }
    }
}
