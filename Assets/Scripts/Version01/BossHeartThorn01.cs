using UnityEngine;
using UnityEngine.SceneManagement;

public class BossHeartThorn01 : MonoBehaviour
{
    private enum ThornState { WaitingForFirstBeat, CanGrab, Grabbed, CanPull, Finished }

    private const int RequiredPulls = 4;
    public static int SuccessfulPulls { get; private set; }
    public static bool ChallengeStarted { get; private set; }
    public static int OuterPondsToRestore { get; private set; }
    public static bool IsComplete => SuccessfulPulls >= RequiredPulls;

    [SerializeField] private Transform heart;
    [SerializeField] private Transform thornVisual;
    [SerializeField] private Sprite thornSprite;
    private LayerMask wallLayers = ~0;
    private float minBeatInterval = 5f;
    private float maxBeatInterval = 10f;
    private float interactionRange = 1f;
    private float emergedLength = 1.5f;
    private float wallLength = 1f;
    private float thornMoveDuration = 0.25f;
    private float beatInputWindow = 0.75f;
    private float resultDelay = 0.75f;

    private ThornState state;
    private Player01 player;
    private Vector3 heartBaseScale;
    private Vector3 thornBaseScale;
    private float nextBeatTime;
    private float resultTime;
    private float targetWallLength;
    private float inputDeadline;
    private Coroutine thornMove;

    public static void BeginChallenge()
    {
        ChallengeStarted = true;
    }

    public static void ResetProgress()
    {
        SuccessfulPulls = 0;
        ChallengeStarted = false;
        OuterPondsToRestore = 0;
    }

    public static void ClearOuterRestore()
    {
        OuterPondsToRestore = 0;
    }

    void Awake()
    {
        heart ??= transform;
        Rigidbody body = heart.GetComponent<Rigidbody>();
        if (body != null)
        {
            body.isKinematic = true;
            body.useGravity = false;
            body.constraints = RigidbodyConstraints.FreezeAll;
        }

        player = FindFirstObjectByType<Player01>();
        heartBaseScale = heart.localScale;
        EnsureThornVisual();
        thornBaseScale = thornVisual.localScale;
        thornVisual.localScale = new Vector3(0f, thornBaseScale.y, thornBaseScale.z);
    }

    void Start()
    {
        if (IsComplete)
        {
            SceneManager.LoadScene("GameScene");
            return;
        }

        state = ThornState.WaitingForFirstBeat;
        ScheduleNextBeat();
    }

    void Update()
    {
        if (state == ThornState.Finished)
        {
            if (Time.time >= resultTime) SceneManager.LoadScene("GameScene");
            return;
        }

        if (Time.time >= nextBeatTime) Beat();
        if (state == ThornState.CanPull && Time.time > inputDeadline)
        {
            Fail();
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && IsPlayerClose()) HandleInteraction();
    }

    private void Beat()
    {
        ScheduleNextBeat();
        StartCoroutine(PulseHeart());

        switch (state)
        {
            case ThornState.WaitingForFirstBeat:
                ChooseThornDirection();
                MoveThornTo(emergedLength);
                state = ThornState.CanGrab;
                inputDeadline = Time.time + beatInputWindow;
                break;
            case ThornState.CanGrab:
                MoveThornTo(0f);
                state = ThornState.WaitingForFirstBeat;
                break;
            case ThornState.Grabbed:
                state = ThornState.CanPull;
                inputDeadline = Time.time + beatInputWindow;
                break;
            case ThornState.CanPull:
                Fail();
                break;
        }
    }

    private void HandleInteraction()
    {
        if (state == ThornState.CanGrab && Time.time <= inputDeadline)
        {
            state = ThornState.Grabbed;
            Debug.Log("Heart thorn grabbed. Pull it after the next beat.", this);
        }
        else if (state == ThornState.CanPull)
        {
            SuccessfulPulls++;
            OuterPondsToRestore = 0;
            MoveThornTo(0f);
            Finish();
            Debug.Log($"Heart thorn removed: {SuccessfulPulls}/{RequiredPulls}", this);
        }
    }

    private void Fail()
    {
        OuterPondsToRestore = 1;
        MoveThornTo(targetWallLength);
        Finish();
        Debug.Log("Heart thorn reached the wall. One outer thorn was restored.", this);
    }

    private void Finish()
    {
        state = ThornState.Finished;
        resultTime = Time.time + resultDelay;
    }

    private bool IsPlayerClose()
    {
        if (player == null) player = FindFirstObjectByType<Player01>();
        if (player == null) return false;

        Collider heartCollider = heart.GetComponent<Collider>();
        Vector3 nearestPoint = heartCollider != null
            ? heartCollider.ClosestPoint(player.transform.position)
            : heart.position;
        return Vector3.Distance(player.transform.position, nearestPoint) <= interactionRange;
    }

    private void EnsureThornVisual()
    {
        if (thornVisual == null)
        {
            GameObject visual = new GameObject("Heart Thorn Visual");
            visual.transform.SetParent(heart, false);
            thornVisual = visual.transform;
        }

        SpriteRenderer renderer = thornVisual.GetComponent<SpriteRenderer>() ??
            thornVisual.gameObject.AddComponent<SpriteRenderer>();
        SpriteRenderer heartRenderer = heart.GetComponent<SpriteRenderer>();
        if (renderer.sprite == null) renderer.sprite = thornSprite != null ? thornSprite : heartRenderer?.sprite;
        renderer.color = Color.white;
        renderer.sortingOrder = heartRenderer != null ? heartRenderer.sortingOrder + 1 : 1;
    }

    private void SetThornLength(float length)
    {
        thornVisual.localScale = new Vector3(length, thornBaseScale.y * 0.25f, thornBaseScale.z);
        thornVisual.localPosition = Vector3.right * length * 0.5f;
    }

    private void MoveThornTo(float targetLength)
    {
        if (thornMove != null) StopCoroutine(thornMove);
        thornMove = StartCoroutine(AnimateThornLength(targetLength));
    }

    private System.Collections.IEnumerator AnimateThornLength(float targetLength)
    {
        float startLength = thornVisual.localScale.x;
        float duration = Mathf.Max(0.01f, thornMoveDuration);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetThornLength(Mathf.Lerp(startLength, targetLength, elapsed / duration));
            yield return null;
        }

        SetThornLength(targetLength);
        thornMove = null;
    }

    private void ScheduleNextBeat()
    {
        float minimum = Mathf.Max(0.1f, minBeatInterval);
        nextBeatTime = Time.time + Random.Range(minimum, Mathf.Max(minimum, maxBeatInterval));
    }

    private void ChooseThornDirection()
    {
        float angle = Random.Range(0f, 360f);
        thornVisual.localRotation = Quaternion.Euler(0f, 0f, angle);
        Vector3 worldDirection = heart.TransformDirection(thornVisual.localRotation * Vector3.right);
        targetWallLength = wallLength;

        foreach (RaycastHit hit in Physics.RaycastAll(heart.position, worldDirection, wallLength,
                     wallLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.IsChildOf(heart) || hit.transform.GetComponentInParent<Player01>() != null) continue;
            targetWallLength = Mathf.Min(targetWallLength, hit.distance);
        }
    }

    private System.Collections.IEnumerator PulseHeart()
    {
        heart.localScale = heartBaseScale * 1.1f;
        yield return new WaitForSeconds(0.15f);
        if (heart != null) heart.localScale = heartBaseScale;
    }
}
