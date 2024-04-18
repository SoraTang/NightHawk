using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GlidingState : PlayerState
{
    private MovementSM _sm;
    private float glideForce = -2.7f; // 自定义的滑翔力大小

    private Transform leftController;
    private Transform rightController;

    public float threshold = 0.1f; // 差异阈值
    public float forceMagnitude = 0.25f; // 施加的力大小

    public GlidingState(MovementSM stateMachine) : base("GlidingState", stateMachine)
    {
        _sm = stateMachine;
    }

    public override void EnterState()
    {
        base.EnterState();
        _sm.player.GetComponent<ActionBasedContinuousTurnProvider>().enabled = false;
        _sm.player.GetComponent<ActionBasedContinuousMoveProvider>().enabled = false;
        leftController = _sm.leftController;
        rightController = _sm.rightController;
        Rigidbody rigidbody = _sm.player.GetComponent<Rigidbody>();
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0.2f, rigidbody.velocity.z);
        // 在进入 GlidingState 状态时进行初始化

    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        InputEvent.Instance.onLeftTriggerUp += TriggerUp;
        InputEvent.Instance.onRightTriggerUp += TriggerUp;
        InputEvent.Instance.onLeftTriggerDown -= TriggerUp;
        InputEvent.Instance.onRightTriggerDown -= TriggerUp;
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        // 给物体施加持续的浮力
        _sm.player.GetComponent<Rigidbody>().AddForce(Vector3.up * glideForce, ForceMode.Acceleration);

        AddForce();
    }

    public void HandleCollisionEnter(Collision collision)
    {
        // 处理碰撞事件
        if (collision.gameObject.CompareTag("Ground"))
        {
            _sm.player.GetComponent<ActionBasedContinuousTurnProvider>().enabled = true;
            _sm.player.GetComponent<ActionBasedContinuousMoveProvider>().enabled = true;
            playerStateMachine.ChangeState(_sm.idleState);
        }
    }
    public void HandleCollisionExit(Collision collision)
    {
        // 处理碰撞退出事件
    }


    public void TriggerUp()
    {
        _sm.player.GetComponent<ActionBasedContinuousTurnProvider>().enabled = true;
        _sm.player.GetComponent<ActionBasedContinuousMoveProvider>().enabled = true;
        playerStateMachine.ChangeState(_sm.flappingState);
    }

    public void AddForce()
    {
        Rigidbody rigidbody = _sm.player.GetComponent<Rigidbody>();
        Vector3 leftPos = leftController.position;
        Vector3 rightPos = rightController.position;

        // 计算两者的垂直差异
        float verticalDifference = rightPos.y - leftPos.y;

        // 如果差异超过阈值
        if (verticalDifference > threshold)
        {
            Vector3 forceDirection = Vector3.Cross(rigidbody.velocity.normalized, Vector3.up);

            rigidbody.AddForce(forceDirection * forceMagnitude, ForceMode.Force);
        }
        if(verticalDifference < -threshold) 
        {
            Vector3 forceDirection = Vector3.Cross(rigidbody.velocity.normalized, Vector3.up);
            rigidbody.AddForce(-forceDirection * forceMagnitude, ForceMode.Force);
        }
    }
}
