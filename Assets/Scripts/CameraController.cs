using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;
    public Transform CameraPivot => cameraPivot;
    private Quaternion targetRotate;
    private float rotateSpeed = 360f;
    private bool isRotating;

    private bool isTapPress = false;

    [SerializeField] private Player player;

    // Start is called before the first frame update
    void Awake()
    {
        if (cameraPivot == null)
        {
            Debug.LogError("Camera Pivot is not assigned.", this);
            enabled = false;
            return;
        }

        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }

        targetRotate = cameraPivot.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        KeyInput();
    }
    
    void KeyInput()
    {
        if (!isRotating)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                targetRotate = cameraPivot.rotation * Quaternion.Euler(0, 90, 0);
                StartCoroutine(RotateCamera());
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                targetRotate = cameraPivot.rotation * Quaternion.Euler(0, -90, 0);
                StartCoroutine(RotateCamera());
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                isTapPress = !isTapPress;
                
                if (isTapPress)
                {
                    targetRotate = cameraPivot.rotation * Quaternion.Euler(90, 0, 0);
                }
                else
                {
                    targetRotate = cameraPivot.rotation * Quaternion.Euler(-90, 0, 0);
                }

                StartCoroutine(RotateCamera());
            }

            if (isTapPress && player != null && player.playerType == PlayerType.bomb)
            {
                isTapPress = false;
                targetRotate = cameraPivot.rotation * Quaternion.Euler(-90, 0, 0);
                StartCoroutine(RotateCamera());
            }
        }
    }

    IEnumerator RotateCamera()
    {
        isRotating = true;

        while (Quaternion.Angle(cameraPivot.rotation, targetRotate) > 0.1f)
        {
            cameraPivot.rotation = Quaternion.RotateTowards(cameraPivot.rotation, targetRotate, rotateSpeed * Time.deltaTime);
            yield return null;
        }

        cameraPivot.rotation = targetRotate;

        isRotating = false;
    }
}
