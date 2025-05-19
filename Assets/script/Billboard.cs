using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Billboard : MonoBehaviour
{
    public Camera arCamera;  // optional override

    void Start()
    {
        if (arCamera == null)
            arCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (arCamera == null) return;

        Vector3 dir = transform.position - arCamera.transform.position;
        dir.y = 0;  // keep upright
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }
}
