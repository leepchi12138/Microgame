using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ����ǽ��������ͳһ���������������ǽ���赲�߼�
/// ���ܣ�������ƶ�ʱ����Ƿ����ײ������ǽ������ײ���������ǽ����������ֱ���赲
/// </summary>
public class LimitZoneManagerversion1 : MonoBehaviour
{
    // ��ҵ�Transform��������ڻ�ȡ���������λ��
    public Transform player;
    // ����ǽ�Ĳ㼶���룬�������߼��ʱֻ���ָ���㼶������
    public LayerMask AirWall;
    // ���߼��ľ��룬�����ж�����Ƿ񼴽���ײ������ǽ
    public float checkDistance = 0.5f;
    // ��ҵĶ�����������ڻ�ȡ��ҵ��ƶ�����;���
    private Animator _anim;

    /// <summary>
    /// ��ʼ���������ڽű�����ʱ����
    /// ���ܣ���ȡ��ҵĶ������
    /// </summary>
    void Start()
    {
        // ���������ô��ڣ����ȡ��Animator���
        if (player != null)
            _anim = player.GetComponent<Animator>();
    }

    /// <summary>
    /// ֡���º�����ÿ֡����һ��
    /// ���ܣ��������ƶ������������ǽ����ײ�߼�
    /// </summary>
    void Update()
    {
        // �����һ򶯻���������ڣ����˳���������������ô���
        if (player == null || _anim == null)
            return;

        // �Ӷ��������ȡ��ұ�֡���ƶ��������ڶ�����λ�ƣ�
        Vector3 move = _anim.deltaPosition;

        // ����ƶ�����С�������������������账����ײ��ֱ���˳�
        if (move.magnitude < 0.001f)
            return;

        // ������Ҳ�������ײʱ��������λ��
        Vector3 newPos = player.position + move;

        // ���߼�⣺����ҵ�ǰλ�����ƶ����������ߣ�����Ƿ����ײ������ǽ
        // ���������(���λ��)������(�ƶ������һ��)����ײ��Ϣ�������롢���㼶
        RaycastHit hit;
        if (Physics.Raycast(player.position, move.normalized, out hit, checkDistance, AirWall))
        {
            // �����⵽����ǽ���������ǽ�����ķ���
            // ԭ�����ƶ�����ͶӰ������ǽ���棨��ֱ��ǽ�ķ��߷���
            Vector3 slideDir = Vector3.ProjectOnPlane(move, hit.normal);

            // ���㻬�������λ��
            newPos = player.position + slideDir;

            // ��΢�ط��߷���ƫ��һ�㣬������ҿ���ǽ��
            newPos += hit.normal * 0.05f;
        }

        // Ӧ�ü�������λ�õ����
        player.position = newPos;
    }
}