using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//������඼�ǳ�����ص���
using UnityEngine.UI;

public class MyFirstScript : MonoBehaviour
{
    private Transform monoTransform;
    [Range(1f,100f)]
    private float moveSpeed = 10f;
    //private float eulerX;

    private float sceneTime = 0;
    private float loadSceneDuration = 30f;//���س�����ʱ����

    [HideInInspector]//ϣ�������ǹ��еĲ���˽�е�
    public GameObject directionalLight;//����������
    [SerializeField]//ϣ��������˽�еĲ��ǹ��еģ�ͬ��ӵ����ק�Ĺ���
    private Transform lightTransform;
    void Awake()
    {
        //this.gameObject //����˭�����Ϸ��صľ���˭
        Debug.LogError("Awake:" + gameObject.name);
        monoTransform = gameObject.GetComponent<Transform>();//���Լ������
        //monoTransform = this.transform;

        directionalLight = GameObject.Find("Directional Light");//���������󷽷�,ֱ��������
        lightTransform = directionalLight.GetComponent<Transform>();//��������������

        //����ĳ������¹رյ�ǰ���(���)�ű�����,��ϣ���رն����������� this.gameObject.SetActive(false);
        directionalLight.GetComponent<Light>().enabled = true;//transform��û��enable���Ե�,�����������

        Object fLight = GameObject.FindObjectOfType(typeof(Light));//���ҳ����ﺬ��Light����Ķ���
        //�ҵ���������Ķ���
        Debug.LogWarning((fLight as Light).gameObject);
        //GameObject.SetActive(bool);�����Ƿ�����

        //eulerX = lightTransform.eulerAngles.x;
    }
    void OnEnable()
    {
        //this.gameObject //����˭�����Ϸ��صľ���˭
        Debug.LogError("OnEnable:" + gameObject.name);
    }
    // Start is called before the first frame update
    void Start()
    {
        //this.gameObject //����˭�����Ϸ��صľ���˭
        Debug.LogError("Start:" + gameObject.name);

        //transform.rotation = Quaternion.Euler(new Vector3(0,45,0));
        transform.eulerAngles = new Vector3(0,45,0);
        // Quaternion quaternion = transform.rotation;//��Ԫ�ؽǣ����Խ������������(ŷ������һЩ�޷�ת���ķ���)����ŷ����euler������⣬
        StartCoroutine(Load());//����Я�̵�api
        //GameObject.Instantiate("����");��¡����
        //Destroy(�ӳ�ɾ����5s);
    }

    //�첽���� �õ�Я��
    private IEnumerator Load() {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("1");
        GameObject player = GameObject.Find("Player");
        DontDestroyOnLoad(player);//�����³����������Բ�����ʧ,������ͬ��Ҳ������ʧ
        asyncOperation.allowSceneActivation = false;//��������������ת
        while (asyncOperation.progress < 0.9f) {
            Debug.Log("loading" + asyncOperation.progress);
            yield return null;//����ʱ�̻�ȡ���ȣ�nullָ����һ֡���ò�һ֡��ȡ����//���ܳ��ֵ�bug�������¼��ع���
        }
        asyncOperation.allowSceneActivation = true;

    }
    private void Update()
    {
       Debug.LogWarning("Update:" + gameObject.name);
        monoTransform.position = monoTransform.position + new Vector3(0, 0, moveSpeed * Time.deltaTime);//�����涨�������Ƕ�
        Debug.Log(Time.deltaTime);//������Ϸ֡��ʱ��
        //monoTransform.Translate(0,0, moveSpeed* Time.deltaTime);
        

        lightTransform.Rotate(new Vector3(10*Time.deltaTime, 0,0));


        //����ʱ����س���
        sceneTime += Time.deltaTime;
        Debug.LogError(sceneTime);
        if (sceneTime>= loadSceneDuration)
        {
            SceneManager.LoadScene("Scene2",LoadSceneMode.Additive);
            //            SceneManager.LoadScene("1"��LoadSceneMode.Additive);//����������1��2ͬʱ����
        }

    }
    void OnDisable()
    {
        //this.gameObject //����˭�����Ϸ��صľ���˭
        Debug.LogError("OnDisable:" + gameObject.name);
    }
}
//ctrl+j ����
//�ڽű������޸���Ϸ�����λ�ã�1.��ȡ���ϵ�Transform���
//Vector3 �ṹ�� ������xyz