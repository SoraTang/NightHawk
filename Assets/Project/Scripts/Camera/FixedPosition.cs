using UnityEngine;

public class FixPosition : MonoBehaviour
{
    public Vector3 fixedPosition;

    void Start()
    {
        // 在游戏开始时将物体的位置设置为固定位置
        transform.position = fixedPosition;
    }
}
