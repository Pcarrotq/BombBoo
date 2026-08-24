using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController01 : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;
    public Transform CameraPivot => cameraPivot;
    private Quaternion targetRotate;
    private float rotateSpeed = 360f;
    private bool isRotating;
    public bool IsRotating => isRotating;
    public int SideViewIndex => Mathf.RoundToInt(cameraPivot.eulerAngles.y / 90f) % 4;

    private bool isTopView;

    [SerializeField] private Player01 player01;
    private Vector3 followOffset;

    // Start is called before the first frame update
    void Awake()
    {
        if (cameraPivot == null)
        {
            Debug.LogError("Camera Pivot is not assigned.", this);
            enabled = false;
            return;
        }

        if (player01 == null)
        {
            player01 = FindFirstObjectByType<Player01>();
        }

        targetRotate = cameraPivot.rotation;
        if (player01 != null)
        {
            followOffset = cameraPivot.position - player01.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        KeyInput();
    }

    void LateUpdate()
    {
        if (player01 == null)
        {
            player01 = FindFirstObjectByType<Player01>();
            if (player01 == null) return;

            followOffset = cameraPivot.position - player01.transform.position;
        }

        cameraPivot.position = player01.transform.position + followOffset;
    }
    
    void KeyInput()
    {
        if (isRotating || player01 == null) return;

        bool shouldUseTopView = player01.playerType == PlayerType.boo;
        if (shouldUseTopView != isTopView)
        {
            isTopView = shouldUseTopView;
            targetRotate = cameraPivot.rotation * Quaternion.Euler(isTopView ? 90 : -90, 0, 0);
            StartCoroutine(RotateCamera());
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Z))
        {
            targetRotate = cameraPivot.rotation * Quaternion.Euler(0, 90, 0);
            StartCoroutine(RotateCamera());
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.C))
        {
            targetRotate = cameraPivot.rotation * Quaternion.Euler(0, -90, 0);
            StartCoroutine(RotateCamera());
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
