using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public class control : MonoBehaviour
{

    public Rigidbody Player;
    public Animator animator;
    // �ٶȲ���
    public float baseSpeed = 0.1f;      // �����ٶ�

    [SerializeField]
    private float maxSpeed = 10f;      // ����ٶ�
    [SerializeField]
    private float normalAcceleration = 4f;   // ��ͨ���ٶȣ�����W��
    [SerializeField]
    private float boostAcceleration = 8f;    // ��̼��ٶȣ�W+Shift��
    [SerializeField]
    private float slowdownRate = 4f;         // �������ʣ���S��
    [SerializeField]
    //private float resetDelay = 0.5f;   // ��ײ�������ٶȵ��ӳ�ʱ��
    // Start is called before the first frame update

    // ״̬����    
    private float currentSpeed; // ��ǰ�ٶ�
    //private bool isWalking = false;
    //private bool isRunning = false;
    private bool isBoosting = false;  // �Ƿ����ڳ�̣�W+Shift��
    private bool isSlowing = false;   // �Ƿ����ڼ��٣���S��
    private bool isSpeeding = false;  // �Ƿ����ڼ���
    //private bool canCollide = true;   // �Ƿ������ײ�������޵�ʱ�䣩


    void Start()
    {
        animator = GetComponent<Animator>();
        Player = GetComponent<Rigidbody>();
        currentSpeed = baseSpeed;//��ʼ�ٶȻ���ֵ
        Player.freezeRotation = true;// ����Rigidbody��������ֹ���ƶ���ͻ
        Player.useGravity = false; // �������Ҫ����
    }


    void Update()
    {
        Debug.Log(currentSpeed);
        bool isWKeyDown = Input.GetKey(KeyCode.W);
        bool isSKeyDown = Input.GetKey(KeyCode.S);
        bool isShiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // ����״̬
        isBoosting = isWKeyDown && isShiftDown;
        isSlowing = isSKeyDown;
        isSpeeding = isWKeyDown;

        // �����ٶ�
        CalculateSpeed();
        // ���¶���״̬
        UpdateAnimation();

    }
    void FixedUpdate()
    {
        // ʹ��Rigidbody���������ƶ������transform.Translate��
        Vector3 movement = transform.forward * currentSpeed * Time.fixedDeltaTime;
        Player.MovePosition(Player.position + movement);
    }
    void CalculateSpeed()
    {
        if (isBoosting)
        {
            // W+Shift�����ټ���
            currentSpeed = Mathf.Min(currentSpeed + boostAcceleration * Time.deltaTime, maxSpeed);
        }
        else if (isSpeeding)
        {
            // ��W����������
            currentSpeed = Mathf.Min(currentSpeed + normalAcceleration * Time.deltaTime, maxSpeed);
        }
        else if (isSlowing)
        {
            // S�����ٵ������ٶ�
            currentSpeed = Mathf.Max(currentSpeed - slowdownRate * Time.deltaTime, baseSpeed);
        }
        else
        {
            // �����룺ƽ�����ٵ������ٶ�
            currentSpeed = Mathf.Lerp(currentSpeed, baseSpeed, Time.deltaTime * 0.1f);
        }
    }
    void UpdateAnimation()
    {
        // ���ݵ�ǰ�ٶ����ö���״̬
        if (currentSpeed <= 0.1f)
        {
            // ��ֹ
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);

        }
        else if (currentSpeed <= 3f)
        {
            // ��·
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", true);
            animator.SetBool("Run", false);
        }
        else
        {
            // �ܲ�
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