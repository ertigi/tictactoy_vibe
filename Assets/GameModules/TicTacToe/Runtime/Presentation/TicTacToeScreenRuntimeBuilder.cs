using UnityEngine;
using UnityEngine.UI;

namespace GameModules.TicTacToe.Presentation
{
    /// <summary>
    /// Собирает runtime UI встроенной миниигры без modal-overlay поведения.
    /// </summary>
    public static class TicTacToeScreenRuntimeBuilder
    {
        public const int CurrentLayoutVersion = 3;

        private static readonly Color PanelColor = new(0.95f, 0.93f, 0.88f, 1f);
        private static readonly Color PanelBorderColor = new(0.84f, 0.33f, 0.18f, 0.28f);
        private static readonly Color TextColor = new(0.14f, 0.16f, 0.20f, 1f);
        private static readonly Color AccentColor = new(0.79f, 0.28f, 0.17f, 1f);
        private static readonly Color ButtonColor = new(0.15f, 0.17f, 0.22f, 1f);
        private static readonly Color ButtonTextColor = new(0.96f, 0.95f, 0.91f, 1f);
        private static readonly Color BoardBackgroundColor = new(0.12f, 0.14f, 0.18f, 0.08f);
        private static readonly Color CellColor = new(0.13f, 0.15f, 0.19f, 1f);
        private static readonly Color HighlightColor = new(0.94f, 0.73f, 0.20f, 0.24f);

        public static void Build(TicTacToeScreenView view)
        {
            var root = EnsureRoot(view);
            ClearChildren(root);
            EnsureCanvas(root.gameObject);

            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            var contentRoot = CreateRect("ContentRoot", root);
            Stretch(contentRoot, 20f);

            var panelSlot = CreateRect("PanelSlot", contentRoot);
            Stretch(panelSlot);

            var panel = CreateRect("Panel", panelSlot);
            panel.anchorMin = new Vector2(0.5f, 0.5f);
            panel.anchorMax = new Vector2(0.5f, 0.5f);
            panel.pivot = new Vector2(0.5f, 0.5f);
            panel.anchoredPosition = Vector2.zero;
            panel.sizeDelta = new Vector2(548f, 736f);
            var panelScaler = panel.gameObject.AddComponent<TicTacToeRectFitToParent>();
            panelScaler.Configure(new Vector2(548f, 736f), Vector2.zero, 1f);
            var panelImage = panel.gameObject.AddComponent<Image>();
            panelImage.type = Image.Type.Simple;
            panelImage.color = PanelColor;

            var panelOutline = panel.gameObject.AddComponent<Outline>();
            panelOutline.effectColor = PanelBorderColor;
            panelOutline.effectDistance = new Vector2(1f, -1f);

            var panelLayout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            panelLayout.padding = new RectOffset(28, 28, 28, 28);
            panelLayout.spacing = 16f;
            panelLayout.childAlignment = TextAnchor.UpperCenter;
            panelLayout.childControlWidth = true;
            panelLayout.childControlHeight = true;
            panelLayout.childForceExpandWidth = true;
            panelLayout.childForceExpandHeight = false;

            var titleLabel = CreateLayoutLabel(
                "Title",
                panel,
                font,
                34,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                AccentColor,
                48f,
                "Tic Tac Toe");
            titleLabel.horizontalOverflow = HorizontalWrapMode.Overflow;

            var statusLabel = CreateLayoutLabel(
                "Status",
                panel,
                font,
                24,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                TextColor,
                40f,
                string.Empty);

            var boardSection = CreateRect("BoardSection", panel);
            var boardSectionLayout = boardSection.gameObject.AddComponent<LayoutElement>();
            boardSectionLayout.flexibleHeight = 1f;
            boardSectionLayout.minHeight = 396f;

            var boardSurface = CreateRect("BoardSurface", boardSection);
            boardSurface.anchorMin = new Vector2(0.5f, 0.5f);
            boardSurface.anchorMax = new Vector2(0.5f, 0.5f);
            boardSurface.pivot = new Vector2(0.5f, 0.5f);
            boardSurface.anchoredPosition = Vector2.zero;
            boardSurface.sizeDelta = new Vector2(396f, 396f);

            var boardSurfaceImage = boardSurface.gameObject.AddComponent<Image>();
            boardSurfaceImage.type = Image.Type.Simple;
            boardSurfaceImage.color = BoardBackgroundColor;

            var boardGrid = boardSurface.gameObject.AddComponent<GridLayoutGroup>();
            boardGrid.padding = new RectOffset(0, 0, 0, 0);
            boardGrid.cellSize = new Vector2(124f, 124f);
            boardGrid.spacing = new Vector2(12f, 12f);
            boardGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            boardGrid.constraintCount = 3;
            boardGrid.childAlignment = TextAnchor.MiddleCenter;
            boardGrid.startAxis = GridLayoutGroup.Axis.Horizontal;
            boardGrid.startCorner = GridLayoutGroup.Corner.UpperLeft;

            var rewardLabel = CreateLayoutLabel(
                "Reward",
                panel,
                font,
                22,
                FontStyle.Normal,
                TextAnchor.MiddleCenter,
                TextColor,
                54f,
                string.Empty);

            var actionsRow = CreateRect("ActionsRow", panel);
            var actionsRowLayout = actionsRow.gameObject.AddComponent<LayoutElement>();
            actionsRowLayout.preferredHeight = 58f;
            actionsRowLayout.minHeight = 58f;

            var actionsLayout = actionsRow.gameObject.AddComponent<HorizontalLayoutGroup>();
            actionsLayout.spacing = 12f;
            actionsLayout.childAlignment = TextAnchor.MiddleCenter;
            actionsLayout.childControlWidth = true;
            actionsLayout.childControlHeight = true;
            actionsLayout.childForceExpandWidth = true;
            actionsLayout.childForceExpandHeight = true;

            var restartButton = CreateButton("RestartButton", actionsRow, font, "Restart");
            var closeButton = CreateButton("CloseButton", actionsRow, font, "Close");

            var cells = new TicTacToeCellView[9];

            for (var index = 0; index < cells.Length; index++)
            {
                var cellRect = CreateRect($"Cell_{index}", boardSurface);
                var cellImage = cellRect.gameObject.AddComponent<Image>();
                cellImage.type = Image.Type.Simple;
                cellImage.color = CellColor;

                var cellButton = cellRect.gameObject.AddComponent<Button>();
                cellButton.targetGraphic = cellImage;

                var highlightRect = CreateRect("Highlight", cellRect);
                Stretch(highlightRect);
                var highlightImage = highlightRect.gameObject.AddComponent<Image>();
                highlightImage.type = Image.Type.Simple;
                highlightImage.color = HighlightColor;
                highlightImage.raycastTarget = false;
                highlightImage.enabled = false;

                var label = CreateFillLabel(
                    "Label",
                    cellRect,
                    font,
                    56,
                    FontStyle.Bold,
                    TextAnchor.MiddleCenter,
                    ButtonTextColor,
                    string.Empty);

                var cellView = cellRect.gameObject.AddComponent<TicTacToeCellView>();
                cellView.BindRuntimeReferences(index, cellButton, label, highlightImage, cellImage);
                cells[index] = cellView;
            }

            view.BindRuntimeReferences(cells, closeButton, restartButton, statusLabel, rewardLabel);
            view.SetLayoutVersion(CurrentLayoutVersion);
        }

        private static RectTransform EnsureRoot(TicTacToeScreenView view)
        {
            var rectTransform = view.GetComponent<RectTransform>();

            if (rectTransform == null)
            {
                rectTransform = view.gameObject.AddComponent<RectTransform>();
            }

            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;

            return rectTransform;
        }

        private static void EnsureCanvas(GameObject rootObject)
        {
            var parentCanvas = rootObject.transform.parent != null
                ? rootObject.GetComponentInParent<Canvas>()
                : null;

            if (parentCanvas != null)
            {
                return;
            }

            var canvas = rootObject.GetComponent<Canvas>();

            if (canvas == null)
            {
                canvas = rootObject.AddComponent<Canvas>();
            }

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;

            if (rootObject.GetComponent<CanvasScaler>() == null)
            {
                var scaler = rootObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080f, 1920f);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            }

            if (rootObject.GetComponent<GraphicRaycaster>() == null)
            {
                rootObject.AddComponent<GraphicRaycaster>();
            }
        }

        private static void ClearChildren(RectTransform root)
        {
            for (var i = root.childCount - 1; i >= 0; i--)
            {
                var child = root.GetChild(i).gameObject;

                if (UnityEngine.Application.isPlaying)
                {
                    Object.Destroy(child);
                }
                else
                {
                    Object.DestroyImmediate(child);
                }
            }
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.SetParent(parent, false);
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.identity;
            return rectTransform;
        }

        private static Text CreateLayoutLabel(
            string name,
            Transform parent,
            Font font,
            int fontSize,
            FontStyle fontStyle,
            TextAnchor alignment,
            Color color,
            float preferredHeight,
            string value)
        {
            var rectTransform = CreateRect(name, parent);
            var label = rectTransform.gameObject.AddComponent<Text>();
            label.font = font;
            label.fontSize = fontSize;
            label.fontStyle = fontStyle;
            label.alignment = alignment;
            label.color = color;
            label.supportRichText = false;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.text = value;

            var layoutElement = rectTransform.gameObject.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = preferredHeight;
            layoutElement.minHeight = preferredHeight;

            return label;
        }

        private static Text CreateFillLabel(
            string name,
            Transform parent,
            Font font,
            int fontSize,
            FontStyle fontStyle,
            TextAnchor alignment,
            Color color,
            string value)
        {
            var rectTransform = CreateRect(name, parent);
            Stretch(rectTransform);

            var label = rectTransform.gameObject.AddComponent<Text>();
            label.font = font;
            label.fontSize = fontSize;
            label.fontStyle = fontStyle;
            label.alignment = alignment;
            label.color = color;
            label.supportRichText = false;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.text = value;
            label.raycastTarget = false;

            return label;
        }

        private static Button CreateButton(
            string name,
            Transform parent,
            Font font,
            string text)
        {
            var rectTransform = CreateRect(name, parent);
            var image = rectTransform.gameObject.AddComponent<Image>();
            image.type = Image.Type.Simple;
            image.color = ButtonColor;

            var button = rectTransform.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            var colors = button.colors;
            colors.normalColor = ButtonColor;
            colors.highlightedColor = new Color(0.22f, 0.25f, 0.31f, 1f);
            colors.pressedColor = new Color(0.10f, 0.12f, 0.16f, 1f);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = new Color(0.27f, 0.27f, 0.27f, 0.55f);
            button.colors = colors;

            var layoutElement = rectTransform.gameObject.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 58f;
            layoutElement.flexibleWidth = 1f;
            layoutElement.minWidth = 120f;

            CreateFillLabel(
                "Label",
                rectTransform,
                font,
                21,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                ButtonTextColor,
                text);

            return button;
        }

        private static void Stretch(RectTransform rectTransform, float inset = 0f)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = new Vector2(inset, inset);
            rectTransform.offsetMax = new Vector2(-inset, -inset);
        }
    }

    /// <summary>
    /// Равномерно вписывает карточку миниигры в родительский контейнер без обрезания.
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class TicTacToeRectFitToParent : MonoBehaviour
    {
        [SerializeField]
        private Vector2 designSize = new(548f, 736f);

        [SerializeField]
        private Vector2 padding = Vector2.zero;

        [SerializeField]
        private float maxScale = 1f;

        private RectTransform _rectTransform;

        public void Configure(Vector2 targetDesignSize, Vector2 targetPadding, float targetMaxScale)
        {
            designSize = targetDesignSize;
            padding = targetPadding;
            maxScale = Mathf.Max(0.01f, targetMaxScale);
            Apply();
        }

        private void Awake()
        {
            Apply();
        }

        private void OnEnable()
        {
            Apply();
        }

        private void Update()
        {
            Apply();
        }

        private void OnRectTransformDimensionsChange()
        {
            Apply();
        }

        private void OnTransformParentChanged()
        {
            Apply();
        }

        private void Apply()
        {
            if (designSize.x <= 0f || designSize.y <= 0f)
            {
                return;
            }

            if (_rectTransform == null)
            {
                _rectTransform = transform as RectTransform;
            }

            if (_rectTransform == null)
            {
                return;
            }

            var parentRectTransform = _rectTransform.parent as RectTransform;

            if (parentRectTransform == null)
            {
                return;
            }

            var parentRect = parentRectTransform.rect;
            var availableWidth = Mathf.Max(1f, parentRect.width - (padding.x * 2f));
            var availableHeight = Mathf.Max(1f, parentRect.height - (padding.y * 2f));
            var scaleByWidth = availableWidth / designSize.x;
            var scaleByHeight = availableHeight / designSize.y;
            var scale = Mathf.Min(scaleByWidth, scaleByHeight, maxScale);
            scale = Mathf.Max(0.05f, scale);

            _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            _rectTransform.pivot = new Vector2(0.5f, 0.5f);
            _rectTransform.anchoredPosition = Vector2.zero;
            _rectTransform.sizeDelta = designSize;
            _rectTransform.localScale = new Vector3(scale, scale, 1f);
            _rectTransform.localRotation = Quaternion.identity;
        }
    }
}
