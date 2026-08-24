using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    private const float ExitRange = 0.75f;
    private Player01 player;

    public static void Show(Vector3 position, Sprite sprite)
    {
        GameObject exitObject = new GameObject("Exit");
        exitObject.transform.position = position;
        SpriteRenderer renderer = exitObject.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = Color.cyan;
        renderer.sortingOrder = 10;
        exitObject.AddComponent<BoxCollider>().isTrigger = true;
        exitObject.AddComponent<Exit>();
    }

    void Update()
    {
        if (player == null) player = FindFirstObjectByType<Player01>();
        if (player == null || Vector3.Distance(player.transform.position, transform.position) > ExitRange) return;

        Debug.Log("Game Clear!");
        BossHeartThorn01.ResetProgress();
        SceneManager.LoadScene("StartScene");
    }
}
