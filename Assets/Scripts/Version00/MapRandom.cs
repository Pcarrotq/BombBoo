using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapRandom : MonoBehaviour
{
    [SerializeField] private MapType mapType;
    private static List<Vector3> mapPositions = new List<Vector3>();
    private static int positionSceneHandle = -1;
    private bool isClose;

    private const float MinSpacing = 2f;
    private const float ReachSafety = 0.8f;
    private static readonly Vector3 GroundStart = new Vector3(0f, -4f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        int currentSceneHandle = SceneManager.GetActiveScene().handle;
        if (positionSceneHandle != currentSceneHandle)
        {
            mapPositions.Clear();
            mapPositions.Add(GroundStart);
            positionSceneHandle = currentSceneHandle;
        }

        Vector3 randomPos;

        if (mapType == MapType.footrest)
        {
            const int maxAttempts = 100;
            int attempts = 0;
            Player player = FindFirstObjectByType<Player>();
            Rigidbody playerBody = player != null ? player.GetComponent<Rigidbody>() : null;
            do
            {
                Vector3 anchor = mapPositions[Random.Range(0, mapPositions.Count)];
                randomPos = new Vector3(
                    Random.Range(-4f, 5f),
                    Random.Range(-4f, 0f),
                    Random.Range(-4f, 0f));
                
                isClose = !CanJumpBetween(anchor, randomPos, player, playerBody);

                if (!isClose)
                {
                    foreach (Vector3 mapPos in mapPositions)
                    {
                        if (Vector3.Distance(randomPos, mapPos) < MinSpacing)
                        {
                            isClose = true;
                            break;
                        }
                    }
                }
                attempts++;
            } while (isClose && attempts < maxAttempts);

            if (isClose)
            {
                Debug.LogWarning($"{name}: 점프로 도달 가능한 발판 위치를 찾지 못해 비활성화합니다.");
                gameObject.SetActive(false);
                return;
            }

            Debug.Assert(mapPositions.Exists(position => CanJumpBetween(position, randomPos, player, playerBody)),
                $"{name}: 도달할 수 없는 발판이 생성되었습니다.");
            
            transform.position = randomPos;
            mapPositions.Add(randomPos);
        }
    }

    private static bool CanJumpBetween(Vector3 from, Vector3 to, Player player, Rigidbody playerBody)
    {
        if (player == null || playerBody == null) return false;

        float jumpSpeed = player.JumpForce / playerBody.mass;
        float gravity = Mathf.Abs(Physics.gravity.y);
        if (gravity <= 0f) return false;
        float height = to.y - from.y;
        float discriminant = jumpSpeed * jumpSpeed - 2f * gravity * height;
        if (discriminant < 0f) return false;

        float landingTime = (jumpSpeed + Mathf.Sqrt(discriminant)) / gravity;
        float horizontalDistance = Vector2.Distance(
            new Vector2(from.x, from.z),
            new Vector2(to.x, to.z));
        return horizontalDistance <= player.MoveSpeed * landingTime * ReachSafety;
    }
}
