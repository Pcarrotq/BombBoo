using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBossInside : MonoBehaviour
{
    private static readonly Color MonitorGreen = new Color(0.1f, 1f, 0.35f, 1f);
    private static readonly Color DimGreen = new Color(0.05f, 0.3f, 0.12f, 1f);

    [SerializeField] private Sprite vitalSprite;

    private BossHeartThorn01 thorn;
    private RectTransform track;
    private RectTransform signal;
    private Image target;
    private Image targetRight;
    private TMP_Text prompt;

    void Awake()
    {
        thorn = FindFirstObjectByType<BossHeartThorn01>();
        BuildMonitor();
    }

    void Update()
    {
        if (thorn == null) thorn = FindFirstObjectByType<BossHeartThorn01>();
        if (thorn == null || thorn.CurrentBeatInterval <= 0f) return;

        float progress = 1f - thorn.TimeUntilNextBeat / thorn.CurrentBeatInterval;
        bool inputNow = thorn.IsInputWindowOpen;
        float right = track.rect.width * 0.5f - 10f;
        signal.anchoredPosition = new Vector2(inputNow
            ? Mathf.Lerp(12f, -12f, thorn.InputWindowProgress)
            : Mathf.Lerp(right, 12f, progress), 0f);

        target.color = inputNow ? Color.yellow : DimGreen;
        targetRight.color = target.color;
        signal.GetComponent<Image>().color = Color.white;
        prompt.text = inputNow ? (thorn.IsPullWindow ? "E  PULL" : "E  GRAB") : "HEART BEAT";
    }

    private void BuildMonitor()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        Transform uiParent = canvas != null ? canvas.transform : transform;
        RectTransform panel = CreateImage("Heart Monitor", uiParent, new Color(0f, 0.08f, 0.03f, 0.9f));
        panel.anchorMin = panel.anchorMax = panel.pivot = Vector2.zero;
        panel.anchoredPosition = new Vector2(24f, 24f);
        panel.sizeDelta = new Vector2(420f, 110f);

        track = CreateImage("Beat Track", panel, DimGreen);
        track.anchorMin = new Vector2(0f, 0.5f);
        track.anchorMax = new Vector2(1f, 0.5f);
        track.sizeDelta = new Vector2(-36f, 3f);
        track.anchoredPosition = new Vector2(10f, -10f);

        RectTransform targetRect = CreateImage("E Timing", panel, DimGreen);
        targetRect.anchorMin = targetRect.anchorMax = new Vector2(0.5f, 0.5f);
        targetRect.anchoredPosition = new Vector2(-60f, -10f);
        targetRect.sizeDelta = new Vector2(6f, 52f);
        target = targetRect.GetComponent<Image>();

        RectTransform targetRightRect = CreateImage("E Timing Right", panel, DimGreen);
        targetRightRect.anchorMin = targetRightRect.anchorMax = new Vector2(0.5f, 0.5f);
        targetRightRect.anchoredPosition = new Vector2(60f, -10f);
        targetRightRect.sizeDelta = new Vector2(6f, 52f);
        targetRight = targetRightRect.GetComponent<Image>();

        signal = CreateImage("Beat Signal", track, MonitorGreen);
        signal.anchorMin = signal.anchorMax = new Vector2(0.5f, 0.5f);
        signal.sizeDelta = new Vector2(90f, 30f);
        Image signalImage = signal.GetComponent<Image>();
        signalImage.sprite = vitalSprite;
        signalImage.preserveAspect = true;

        GameObject textObject = new GameObject("Beat Prompt", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(panel, false);
        RectTransform textRect = (RectTransform)textObject.transform;
        textRect.anchorMin = new Vector2(0f, 1f);
        textRect.anchorMax = new Vector2(1f, 1f);
        textRect.pivot = new Vector2(0.5f, 1f);
        textRect.anchoredPosition = new Vector2(0f, -8f);
        textRect.sizeDelta = new Vector2(-24f, 32f);
        prompt = textObject.GetComponent<TextMeshProUGUI>();
        prompt.fontSize = 22f;
        prompt.color = MonitorGreen;
        prompt.alignment = TextAlignmentOptions.Left;
        prompt.text = "HEART BEAT";
    }

    private static RectTransform CreateImage(string name, Transform parent, Color color)
    {
        GameObject item = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        item.transform.SetParent(parent, false);
        item.GetComponent<Image>().color = color;
        return (RectTransform)item.transform;
    }
}
