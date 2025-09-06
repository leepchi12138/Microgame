using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public class control : MonoBehaviour
{

    public Rigidbody Player;
    public Animator animator;
    // 速度参数
    public float baseSpeed = 0.1f;      // 基础速度

    [SerializeField]
    private float maxSpeed = 10f;      // 最大速度
    [SerializeField]
    private float normalAcceleration = 4f;   // 普通加速度（仅按W）
    [SerializeField]
    private float boostAcceleration = 8f;    // 冲刺加速度（W+Shift）
    [SerializeField]
    private float slowdownRate = 4f;         // 减速速率（按S）
    [SerializeField]
    //private float resetDelay = 0.5f;   // 碰撞后重置速度的延迟时间
    // Start is called before the first frame update

    // 状态变量    
    private float currentSpeed; // 当前速度
    //private bool isWalking = false;
    //private bool isRunning = false;
    private bool isBoosting = false;  // 是否正在冲刺（W+Shift）
    private bool isSlowing = false;   // 是否正在减速（按S）
    private bool isSpeeding = false;  // 是否正在加速
    //private bool canCollide = true;   // 是否可以碰撞（用于无敌时间）


    void Start()
    {
        animator = GetComponent<Animator>();
        Player = GetComponent<Rigidbody>();
        currentSpeed = baseSpeed;//初始速度基础值
        Player.freezeRotation = true;// 设置Rigidbody参数，防止与移动冲突
        Player.useGravity = false; // 如果不需要重力
    }


    void Update()
    {
        Debug.Log(currentSpeed);
        bool isWKeyDown = Input.GetKey(KeyCode.W);
        bool isSKeyDown = Input.GetKey(KeyCode.S);
        bool isShiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // 更新状态
        isBoosting = isWKeyDown && isShiftDown;
        isSlowing = isSKeyDown;
        isSpeeding = isWKeyDown;

        // 计算速度
        CalculateSpeed();
        // 更新动画状态
        UpdateAnimation();

    }
    void FixedUpdate()
    {
        // 使用Rigidbody进行物理移动（替代transform.Translate）
        Vector3 movement = transform.forward * currentSpeed * Time.fixedDeltaTime;
        Player.MovePosition(Player.position + movement);
    }
    void CalculateSpeed()
    {
        if (isBoosting)
        {
            // W+Shift：快速加速
            currentSpeed = Mathf.Min(currentSpeed + boostAcceleration * Time.deltaTime, maxSpeed);
        }
        else if (isSpeeding)
        {
            // 仅W：正常加速
            currentSpeed = Mathf.Min(currentSpeed + normalAcceleration * Time.deltaTime, maxSpeed);
        }
        else if (isSlowing)
        {
            // S：减速到基础速度
            currentSpeed = Mathf.Max(currentSpeed - slowdownRate * Time.deltaTime, baseSpeed);
        }
        else
        {
            // 无输入：平滑减速到基础速度
            currentSpeed = Mathf.Lerp(currentSpeed, baseSpeed, Time.deltaTime * 0.1f);
        }
    }
    void UpdateAnimation()
    {
        // 根据当前速度设置动画状态
        if (currentSpeed <= 0.1f)
        {
            // 静止
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);

        }
        else if (currentSpeed <= 3f)
        {
            // 走路
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", true);
            animator.SetBool("Run", false);
        }
        else
        {
            // 跑步
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", false);
            animator.SetBool("Run", true);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            animator.SetTrigger("Jump");
        }
    }

}