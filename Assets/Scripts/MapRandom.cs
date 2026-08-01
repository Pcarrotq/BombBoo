using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRandom : MonoBehaviour
{
    [SerializeField] private MapType mapType;
    private static List<Vector3> mapPositions = new List<Vector3>();
    private bool isClose;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 randomPos;

        if (mapType == MapType.footrest)
        {
            do
            {
                randomPos = new Vector3(
                    Random.Range(-4f, 5f),
                    Random.Range(-4f, 0f),
                    Random.Range(-4f, 0f));
                
                isClose = false;

                foreach (Vector3 mapPos in mapPositions)
                {
                    if (Vector3.Distance(randomPos, mapPos) < 2f)
                    {
                        isClose = true;
                        break;
                    }
                }
            } while (isClose);
            
            transform.position = randomPos;
            mapPositions.Add(randomPos);
        }
    }
}
