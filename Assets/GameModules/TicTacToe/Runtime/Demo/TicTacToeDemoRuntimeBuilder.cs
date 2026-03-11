using UnityEngine;
using UnityEngine.UI;

namespace GameModules.TicTacToe.Demo
{
    /// <summary>
    /// Собирает demo-экран с явным разделением на host UI и embedded mini-game.
    /// </summary>
    public static class TicTacToeDemoRuntimeBuilder
    {
        public const int CurrentLayoutVersion = 3;

        private static readonly Color BackgroundColor = new(0.09f, 0.10f, 0.12f, 1f);
        private static readonly Color FrameColor = new(0.08f, 0.10f, 0.11f, 1f);
        private static readonly Color PanelColor = new(0.94f, 0.92f, 0.86f, 1f);
        private static readonly Color AccentColor = new(0.84f, 0.33f, 0.18f, 1f);
        private static readonly Color TextColor = new(0.14f, 0.16f, 0.20f, 1f);
        private static readonly Color MutedTextColor = new(0.27f, 0.30f, 0.34f, 1f);
        private static readonly Color ButtonColor = new(0.15f, 0.17f, 0.22f, 1f);
        private static readonly Color ButtonTextColor = new(0.96f, 0.95f, 0.91f, 1f);
        private static readonly Color HostFrameColor = new(0.13f, 0.15f, 0.19f, 1f);
        private static readonly Color HostPlaceholderColor = new(0.86f, 0.84f, 0.79f, 1f);
        private static readonly Color InfoBlockColor = new(0.89f, 0.87f, 0.82f, 0.9f);

        public static void Build(TicTacToeDemoView view)
        {
            var root = EnsureRoot(view);
            ClearChildren(root);
            EnsureCanvas(root.gameObject);

            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            var background = CreateRect("Background", root);
            Stretch(background);
            var backgroundImage = background.gameObject.AddComponent<Image>();
            backgroundImage.type = Image.Type.Simple;
            backgroundImage.color = BackgroundColor;

            var shell = CreateRect("Shell", background);
            Stretch(shell, 36f);
            var shellImage = shell.gameObject.AddComponent<Image>();
            shellImage.type = Image.Type.Simple;
            shellImage.color = FrameColor;

            var shellLayout = shell.gameObject.AddComponent<HorizontalLayoutGroup>();
            shellLayout.padding = new RectOffset(28, 28, 28, 28);
            shellLayout.spacing = 20f;
            shellLayout.childAlignment = TextAnchor.UpperLeft;
            shellLayout.childControlWidth = true;
            shellLayout.childControlHeight = true;
            shellLayout.childForceExpandWidth = true;
            shellLayout.childForceExpandHeight = true;

            var sidebar = CreatePanel("Sidebar", shell, 420f);
            var hostPanel = CreatePanel("HostPanel", shell, -1f);

            var sidebarLayoutElement = sidebar.gameObject.GetComponent<LayoutElement>();
            sidebarLayoutElement.flexibleWidth = 0f;
            sidebarLayoutElement.flexibleHeight = 1f;

            var hostPanelLayoutElement = hostPanel.gameObject.GetComponent<LayoutElement>();
            hostPanelLayoutElement.flexibleWidth = 1f;
            hostPanelLayoutElement.flexibleHeight = 1f;
            hostPanelLayoutElement.minWidth = 720f;

            CreateLayoutLabel(
                "Title",
                sidebar,
                font,
                34,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                AccentColor,
                50f,
                "Embedded Mini-Game Demo");

            var sidebarSubtitle = CreateLayoutLabel(
                "Subtitle",
                sidebar,
                font,
                18,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                MutedTextColor,
                72f,
                "The host screen launches TicTacToe as a reusable module and receives the result back.");
            sidebarSubtitle.verticalOverflow = VerticalWrapMode.Overflow;

            var launchButton = CreateButton("LaunchButton", sidebar, font, "Launch Mini-Game");
            var replayButton = CreateButton("ReplayButton", sidebar, font, "Play Again");

            var infoPanel = CreateRect("InfoPanel", sidebar);
            var infoPanelLayoutElement = infoPanel.gameObject.AddComponent<LayoutElement>();
            infoPanelLayoutElement.flexibleHeight = 1f;
            infoPanelLayoutElement.minHeight = 220f;

            var infoPanelImage = infoPanel.gameObject.AddComponent<Image>();
            infoPanelImage.type = Image.Type.Simple;
            infoPanelImage.color = InfoBlockColor;

            var infoLayout = infoPanel.gameObject.AddComponent<VerticalLayoutGroup>();
            infoLayout.padding = new RectOffset(16, 16, 16, 16);
            infoLayout.spacing = 6f;
            infoLayout.childAlignment = TextAnchor.UpperLeft;
            infoLayout.childControlWidth = true;
            infoLayout.childControlHeight = true;
            infoLayout.childForceExpandWidth = true;
            infoLayout.childForceExpandHeight = false;

            var stateLabel = CreateInfoLabel("State", infoPanel, font, "State: idle");
            var outcomeLabel = CreateInfoLabel("Outcome", infoPanel, font, "Outcome: waiting for launch");
            var rewardLabel = CreateInfoLabel("Reward", infoPanel, font, "Reward: waiting for result");
            var sessionLabel = CreateInfoLabel("Session", infoPanel, font, "Session: not started");

            CreateLayoutLabel(
                "HostTitle",
                hostPanel,
                font,
                32,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                AccentColor,
                46f,
                "Module Host");

            var hostSubtitle = CreateLayoutLabel(
                "HostSubtitle",
                hostPanel,
                font,
                18,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                MutedTextColor,
                56f,
                "The mini-game is mounted here as a child object instead of taking over the whole scene.");
            hostSubtitle.verticalOverflow = VerticalWrapMode.Overflow;

            var moduleHostFrame = CreateRect("ModuleHostFrame", hostPanel);
            var moduleHostFrameLayout = moduleHostFrame.gameObject.AddComponent<LayoutElement>();
            moduleHostFrameLayout.flexibleHeight = 1f;
            moduleHostFrameLayout.flexibleWidth = 1f;
            moduleHostFrameLayout.minHeight = 620f;
            moduleHostFrameLayout.preferredHeight = 620f;

            var moduleHostFrameImage = moduleHostFrame.gameObject.AddComponent<Image>();
            moduleHostFrameImage.type = Image.Type.Simple;
            moduleHostFrameImage.color = HostFrameColor;

            var moduleHostViewport = CreateRect("ModuleHostViewport", moduleHostFrame);
            Stretch(moduleHostViewport, 18f);
            moduleHostViewport.gameObject.AddComponent<RectMask2D>();

            var moduleHost = CreateRect("ModuleHost", moduleHostViewport);
            Stretch(moduleHost);

            var hostPlaceholderLabel = CreateFillLabel(
                "HostPlaceholder",
                moduleHost,
                font,
                26,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                HostPlaceholderColor,
                "Mini-game will appear here.");

            view.BindRuntimeReferences(
                launchButton,
                replayButton,
                stateLabel,
                outcomeLabel,
                rewardLabel,
                sessionLabel,
                hostPlaceholderLabel,
                moduleHost);
            view.SetLayoutVersion(CurrentLayoutVersion);
        }

        private static RectTransform EnsureRoot(TicTacToeDemoView view)
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

            if (rootObject.GetComponent<CanvasScaler>() == null)
            {
                var scaler = rootObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            }

            if (rootObject.GetComponent<GraphicRaycaster>() == null)
            {
                rootObject.AddComponent<GraphicRaycaster>();
            }
        }

        private static RectTransform CreatePanel(
            string name,
            Transform parent,
            float preferredWidth)
        {
            var rect = CreateRect(name, parent);
            var image = rect.gameObject.AddComponent<Image>();
            image.type = Image.Type.Simple;
            image.color = PanelColor;

            var layout = rect.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(24, 24, 24, 24);
            layout.spacing = 16f;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            var layoutElement = rect.gameObject.AddComponent<LayoutElement>();
            if (preferredWidth > 0f)
            {
                layoutElement.preferredWidth = preferredWidth;
                layoutElement.minWidth = preferredWidth;
            }
            else
            {
                layoutElement.flexibleWidth = 1f;
            }

            layoutElement.flexibleHeight = 1f;
            return rect;
        }

        private static Text CreateInfoLabel(
            string name,
            Transform parent,
            Font font,
            string value)
        {
            var label = CreateLayoutLabel(
                name,
                parent,
                font,
                18,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                TextColor,
                34f,
                value);
            label.verticalOverflow = VerticalWrapMode.Overflow;
            return label;
        }

        private static Button CreateButton(
            string name,
            Transform parent,
            Font font,
            string text)
        {
            var rect = CreateRect(name, parent);
            var image = rect.gameObject.AddComponent<Image>();
            image.type = Image.Type.Simple;
            image.color = ButtonColor;

            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            var colors = button.colors;
            colors.normalColor = ButtonColor;
            colors.highlightedColor = new Color(0.23f, 0.26f, 0.32f, 1f);
            colors.pressedColor = new Color(0.10f, 0.12f, 0.16f, 1f);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = new Color(0.30f, 0.31f, 0.34f, 0.65f);
            button.colors = colors;

            var layoutElement = rect.gameObject.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 64f;
            layoutElement.minHeight = 64f;

            CreateFillLabel(
                "Label",
                rect,
                font,
                22,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                ButtonTextColor,
                text);

            return button;
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
            var rect = CreateRect(name, parent);
            var label = rect.gameObject.AddComponent<Text>();
            label.font = font;
            label.fontSize = fontSize;
            label.fontStyle = fontStyle;
            label.alignment = alignment;
            label.color = color;
            label.supportRichText = false;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.text = value;

            var layoutElement = rect.gameObject.AddComponent<LayoutElement>();
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
            var rect = CreateRect(name, parent);
            Stretch(rect);

            var label = rect.gameObject.AddComponent<Text>();
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

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.SetParent(parent, false);
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.identity;
            return rectTransform;
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

        private static void Stretch(RectTransform rectTransform, float inset = 0f)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = new Vector2(inset, inset);
            rectTransform.offsetMax = new Vector2(-inset, -inset);
        }
    }
}
