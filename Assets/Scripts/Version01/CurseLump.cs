using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider))]
public class CurseLump : MonoBehaviour
{
    private const int RequiredLumps = 3;
    private const float ExitDelay = 5f;
    private const float InteractionRange = 1f;
    private static readonly List<CurseLump> lumps = new List<CurseLump>();
    private static Transform heart;
    private static Sprite fallbackSprite;
    private static bool exitShown;

    private Player01 player;
    private SpriteRenderer spriteRenderer;
    private bool isPressed;
    private float pressedAt;

    void Awake()
    {
        if (!lumps.Contains(this)) lumps.Add(this);
        EnsureComponents();
    }

    void Update()
    {
        if (!isPressed && Input.GetKeyDown(KeyCode.E) && IsPlayerClose()) Press();
        TryShowExit();
    }

    void OnDestroy()
    {
        lumps.Remove(this);
    }

    public static void BeginFinalPhase(Transform heartTransform)
    {
        heart = heartTransform;
        fallbackSprite = heart.GetComponent<SpriteRenderer>()?.sprite;
        exitShown = false;
        lumps.Clear();

        CurseLump[] placedLumps = FindObjectsByType<CurseLump>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (placedLumps.Length == 0)
        {
            Vector3 height = Vector3.up;
            Vector3[] offsets =
            {
                Vector3.left * 2.5f + height,
                Vector3.forward * 2.5f + height,
                Vector3.right * 2.5f + height
            };
            for (int i = 0; i < RequiredLumps; i++)
            {
                GameObject lumpObject = new GameObject($"Curse Lump {i + 1}");
                lumpObject.transform.position = heart.position + offsets[i];
                lumpObject.AddComponent<SpriteRenderer>();
                lumpObject.AddComponent<BoxCollider>();
                placedLumps = Add(placedLumps, lumpObject.AddComponent<CurseLump>());
            }
        }

        Debug.Assert(placedLumps.Length == RequiredLumps,
            $"Final phase expects exactly {RequiredLumps} CurseLumps, but found {placedLumps.Length}.");
        foreach (CurseLump lump in placedLumps)
        {
            lump.gameObject.SetActive(true);
            lump.Initialize();
        }
    }

    private void Initialize()
    {
        SpriteRenderer renderer = EnsureComponents();
        if (!lumps.Contains(this)) lumps.Add(this);
        player = FindFirstObjectByType<Player01>();
        isPressed = false;
        if (renderer.sprite == null) renderer.sprite = fallbackSprite;
        renderer.color = Color.magenta;
        renderer.sortingOrder = heart.GetComponent<SpriteRenderer>()?.sortingOrder + 2 ?? 2;
    }

    private SpriteRenderer EnsureComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        if (GetComponent<Collider>() == null) gameObject.AddComponent<BoxCollider>();
        return spriteRenderer;
    }

    private void Press()
    {
        isPressed = true;
        pressedAt = Time.time;
        EnsureComponents().color = new Color(0.35f, 0f, 0.35f, 1f);
        Debug.Log($"Curse lump pressed: {PressedCount()}/{RequiredLumps}", this);
    }

    private bool IsPlayerClose()
    {
        if (player == null) player = FindFirstObjectByType<Player01>();
        if (player == null) return false;

        Collider collider = GetComponent<Collider>();
        return Vector3.Distance(player.transform.position,
            collider.ClosestPoint(player.transform.position)) <= InteractionRange;
    }

    private static void TryShowExit()
    {
        if (exitShown || lumps.Count != RequiredLumps) return;

        float lastPressedAt = 0f;
        foreach (CurseLump lump in lumps)
        {
            if (lump == null || !lump.isPressed) return;
            lastPressedAt = Mathf.Max(lastPressedAt, lump.pressedAt);
        }
        if (Time.time < lastPressedAt + ExitDelay) return;

        exitShown = true;
        Player01 player = FindFirstObjectByType<Player01>();
        Vector3 exitPosition = heart.position + Vector3.right * 4f;
        if (player != null) exitPosition.y = player.transform.position.y;
        Exit.Show(exitPosition, fallbackSprite);
    }

    private static int PressedCount()
    {
        int count = 0;
        foreach (CurseLump lump in lumps) if (lump != null && lump.isPressed) count++;
        return count;
    }

    private static CurseLump[] Add(CurseLump[] source, CurseLump item)
    {
        CurseLump[] result = new CurseLump[source.Length + 1];
        source.CopyTo(result, 0);
        result[source.Length] = item;
        return result;
    }
}
