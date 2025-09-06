using System.Collections.Generic;
using UnityEngine;

public class InfiniteRoadGenerator : MonoBehaviour
{
    [Header("���Ĳ���")]
    [SerializeField] private Transform player; // ���λ��
    [SerializeField] private GameObject[] groundBlocks; // �����Ԥ����
    [SerializeField] private float blockLength = 10f; // ��������鳤��
    [SerializeField] private int initialBlocks = 5; // ��ʼ���ɿ���
    [SerializeField] private int visibleBlocks = 10; // �ɼ���Χ�ڿ���

    [Header("�ϰ�������")]
    [SerializeField] private GameObject[] obstacles; // �ϰ���Ԥ����
    [SerializeField] private int obstacleDensity = 3; // ÿN���������ϰ���
    [SerializeField] private int obstacleLayer; // �ϰ���ͼ��

    [Header("��������")]
    [SerializeField] private GameObject[] buildings; // ����Ԥ����
    [SerializeField] private float buildingOffset = 15f; // �������·ƫ����
    [SerializeField] private int buildingDensity = 5; // ÿN�������ɽ���
    [SerializeField] private int buildingLayer; // ����ͼ��

    [Header("ͼ������")]
    [SerializeField] private int groundLayer; // Groundͼ��

    private List<GameObject> activeBlocks = new List<GameObject>(); // ��Ծ���б�
    private List<GameObject> activeObstacles = new List<GameObject>(); // ��Ծ�ϰ����б�
    private List<GameObject> activeBuildings = new List<GameObject>(); // ��Ծ�����б�
    private float startZPosition; // ��·��ʼλ��
    private int currentBlockIndex = 0; // ��ǰ������

    void Start()
    {
        // ��ȡͼ��ID
        groundLayer = LayerMask.NameToLayer("Ground");
        obstacleLayer = LayerMask.NameToLayer("Obstacle");
        buildingLayer = LayerMask.NameToLayer("Building");

        startZPosition = transform.position.z;
        GenerateInitialBlocks();
    }

    void Update()
    {
        CheckAndGenerateBlocks();
    }

    // ���ɳ�ʼ�����
    void GenerateInitialBlocks()
    {
        for (int i = 0; i < initialBlocks; i++)
        {
            GenerateBlockAt(i);
        }
    }

    // ��鲢�����¿�
    void CheckAndGenerateBlocks()
    {
        float playerZ = player.position.z;
        float farthestBlockZ = startZPosition + currentBlockIndex * blockLength;

        // ����ҽӽ��ɼ���Χĩβʱ�����¿�
        if (playerZ > farthestBlockZ - (visibleBlocks * blockLength / 2))
        {
            GenerateNextBlock();
            DestroyOldestBlock();
        }
    }

    // ������һ�������
    void GenerateNextBlock()
    {
        GenerateBlockAt(currentBlockIndex);
    }

    // ��ָ���������ɿ鲢����ͼ�����ײ��
    void GenerateBlockAt(int index)
    {
        Vector3 position = new Vector3(
            transform.position.x,
            0f, // ����Y���꣨����Ϊ0�����ʣ�
            startZPosition + index * blockLength
        );

        // ���ѡ������Ԥ����
        GameObject block = Instantiate(
            groundBlocks[Random.Range(0, groundBlocks.Length)],
            position,
            Quaternion.identity,
            transform
        );

        // ���õ����ͼ��ΪGround���ؼ��޸ģ�
        SetLayerRecursively(block, groundLayer);

        // ��ӻ����Box Collider���ؼ��޸ģ�
        SetupGroundCollider(block);

        activeBlocks.Add(block);
        currentBlockIndex++;

        // �����ϰ���
        if (index % obstacleDensity == 0 && obstacles.Length > 0)
        {
            GenerateObstacle(position);
        }

        // ���ɽ���
        if (index % buildingDensity == 0 && buildings.Length > 0)
        {
            GenerateBuildings(position);
        }
    }

    // �ݹ�����ͼ�㣨���������壩
    void SetLayerRecursively(GameObject obj, int layerIndex)
    {
        obj.layer = layerIndex;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layerIndex);
        }
    }

    // ���õ�����Box Collider
    // �޸Ĵ˴���������� Mesh Collider
    void SetupGroundCollider(GameObject groundBlock)
    {
        // �Ƴ�����ײ��
        Collider[] oldColliders = groundBlock.GetComponents<Collider>();
        foreach (Collider collider in oldColliders)
        {
            Destroy(collider);
        }

        // ��� Mesh Collider��������ã�
        MeshCollider meshCollider = groundBlock.AddComponent<MeshCollider>();
        meshCollider.convex = false; // ��͹�棬���ϸ���ģ��
    }

    // �ϰ��ﱣ�� Box Collider��ԭ�߼����䣩
    void GenerateObstacle(Vector3 blockPosition)
    {
        GameObject obstacle = Instantiate(
            obstacles[Random.Range(0, obstacles.Length)],
            blockPosition + new Vector3(
                Random.Range(-blockLength / 4, blockLength / 4),
                0,
                blockLength / 2
            ),
            Quaternion.identity,
            transform
        );

        SetLayerRecursively(obstacle, obstacleLayer);

        Rigidbody rb = obstacle.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = obstacle.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        // �ϰ���ǿ���� Box Collider
        BoxCollider boxCollider = obstacle.GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = obstacle.AddComponent<BoxCollider>();
        }

        activeObstacles.Add(obstacle);
    }

    // ������ɵĵ���鼰�������
    void DestroyOldestBlock()
    {
        if (activeBlocks.Count > visibleBlocks)
        {
            // ���ٵ����
            Destroy(activeBlocks[0]);
            activeBlocks.RemoveAt(0);

            // ��������ϰ���
            DestroyOldestObstacles();

            // ������ؽ���
            DestroyOldestBuildings();
        }
    }

    // ������ɵ��ϰ���
    void DestroyOldestObstacles()
    {
        // ���ٳ�����Χ���ϰ���
        float destroyDistance = startZPosition + (currentBlockIndex - visibleBlocks) * blockLength;

        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            if (activeObstacles[i] != null && activeObstacles[i].transform.position.z < destroyDistance)
            {
                Destroy(activeObstacles[i]);
                activeObstacles.RemoveAt(i);
            }
        }
    }

    // ���ɽ���
    void GenerateBuildings(Vector3 blockPosition)
    {
        // ��ཨ��
        GameObject leftBuilding = Instantiate(
            buildings[Random.Range(0, buildings.Length)],
            blockPosition + new Vector3(-buildingOffset, 0, blockLength / 2),
            Quaternion.identity,
            transform
        );

        // ���ý���ͼ��
        SetLayerRecursively(leftBuilding, buildingLayer);

        // ȷ����������ײ��
        AddBuildingCollider(leftBuilding);

        activeBuildings.Add(leftBuilding);

        // �Ҳཨ��
        GameObject rightBuilding = Instantiate(
            buildings[Random.Range(0, buildings.Length)],
            blockPosition + new Vector3(buildingOffset, 0, blockLength / 2),
            Quaternion.Euler(0, 180, 0),
            transform
        );

        // ���ý���ͼ��
        SetLayerRecursively(rightBuilding, buildingLayer);

        // ȷ����������ײ��
        AddBuildingCollider(rightBuilding);

        activeBuildings.Add(rightBuilding);
    }

    // Ϊ���������ײ��
    void AddBuildingCollider(GameObject building)
    {
        // ����Ƿ�������ײ��
        Collider collider = building.GetComponent<Collider>();
        if (collider == null)
        {
            // ���������ײ�壨�ʺϸ���ģ�ͣ�
            MeshCollider meshCollider = building.AddComponent<MeshCollider>();
            meshCollider.convex = false; // ��͹�棬�ʺϸ�����״
        }

        // ��Ӹ���ʹ���ܲ�����ײ
        Rigidbody rb = building.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = building.AddComponent<Rigidbody>();
            rb.isKinematic = true; // ��Ϊ�˶�ѧ����
            rb.useGravity = false; // ��������Ӱ��
        }
    }

    // ������ɵĽ���
    void DestroyOldestBuildings()
    {
        // ���ٳ�����Χ�Ľ���
        float destroyDistance = startZPosition + (currentBlockIndex - visibleBlocks) * blockLength;

        for (int i = activeBuildings.Count - 1; i >= 0; i--)
        {
            if (activeBuildings[i] != null && activeBuildings[i].transform.position.z < destroyDistance)
            {
                Destroy(activeBuildings[i]);
                activeBuildings.RemoveAt(i);
            }
        }
    }
}