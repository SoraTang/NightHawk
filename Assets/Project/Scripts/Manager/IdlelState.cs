using UnityEngine;
using UnityEngine.InputSystem;

public class IdleState : PlayerState
{
    private MovementSM _sm;
    private float flapForce = 8.5f;
    private Vector3 flapForceDirection = new Vector3(0f, 3f, 1f).normalized;
    private float flapUpwardForce = 0.5f;

    private Transform leftController;
    private Transform rightController;
    private Transform player;

    private Vector3 previousPosition;
    private bool leftSwing = false;
    private bool rightSwing = false;
    private float swingDelay = 0.6f;
    private float lastSwingTime = 0f;
    private Quaternion lastLeftRotation;
    private Quaternion lastRightRotation;
    private float xRotationThreshold = 15f; // 挥动检测阈值

    public IdleState(MovementSM stateMachine) : base("IdleState", stateMachine)
    {
        _sm = stateMachine;
    }

    public override void EnterState()
    {
        base.EnterState();
        leftController = _sm.leftController;
        rightController = _sm.rightController;
        lastLeftRotation = leftController.rotation;
        lastRightRotation = rightController.rotation;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        flapForceDirection = new Vector3(_sm.player.forward.x, 3f, _sm.player.forward.z).normalized;
        _sm.player.GetComponent<Rigidbody>().AddForce(Vector3.up * flapUpwardForce, ForceMode.Acceleration);
        // 检测挥动触发 Flap
        DetectSwing();
        
    }


    private void Flap()
    {
        // 使用指定的力方向和大小给物体施加力量
        _sm.player.GetComponent<Rigidbody>().AddForce(flapForceDirection * flapForce, ForceMode.Impulse);
    }


    // 挥动检测功能
    private void DetectSwing()
    {
        // 计算左右手柄在x轴上的旋转变化
        float leftRotationDelta = Quaternion.Angle(lastLeftRotation, leftController.rotation);
        float rightRotationDelta = Quaternion.Angle(lastRightRotation, rightController.rotation);

        // 如果旋转变化超过阈值，则认为是挥动
        leftSwing = leftRotationDelta > xRotationThreshold;
        rightSwing = rightRotationDelta > xRotationThreshold;

        // 更新上一帧的旋转信息
        lastLeftRotation = leftController.rotation;
        lastRightRotation = rightController.rotation;

        // 如果左右手柄都向下挥动，并且间隔超过0.6秒，则触发flap函数
        if (leftSwing && rightSwing && Time.time - lastSwingTime >= swingDelay)
        {
            Flap();
            lastSwingTime = Time.time;
            playerStateMachine.ChangeState(_sm.flappingState);
        }
    }
}
