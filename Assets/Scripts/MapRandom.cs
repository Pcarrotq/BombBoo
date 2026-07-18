using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRandom : MonoBehaviour
{
    [SerializeField] private MapType mapType;

    // Start is called before the first frame update
    void Start()
    {
        if (mapType == MapType.footrest)
        {
            transform.position = new Vector3(
                Random.Range(-10f, 10f),
                Random.Range(-10f, 10f),
                Random.Range(-10f, 10f));
        }
    }
}
