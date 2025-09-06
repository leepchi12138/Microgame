using System.Collections.Generic;
using UnityEngine;

public class InfiniteRoadGenerator : MonoBehaviour
{
    [Header("核心参数")]
    [SerializeField] private Transform player; // 玩家位置
    [SerializeField] private GameObject[] groundBlocks; // 地面块预制体
    [SerializeField] private float blockLength = 10f; // 单个地面块长度
    [SerializeField] private int initialBlocks = 5; // 初始生成块数
    [SerializeField] private int visibleBlocks = 10; // 可见范围内块数

    [Header("障碍物设置")]
    [SerializeField] private GameObject[] obstacles; // 障碍物预制体
    [SerializeField] private int obstacleDensity = 3; // 每N个块生成障碍物
    [SerializeField] private int obstacleLayer; // 障碍物图层

    [Header("建筑设置")]
    [SerializeField] private GameObject[] buildings; // 建筑预制体
    [SerializeField] private float buildingOffset = 15f; // 建筑与道路偏移量
    [SerializeField] private int buildingDensity = 5; // 每N个块生成建筑
    [SerializeField] private int buildingLayer; // 建筑图层

    [Header("图层设置")]
    [SerializeField] private int groundLayer; // Ground图层

    private List<GameObject> activeBlocks = new List<GameObject>(); // 活跃块列表
    private List<GameObject> activeObstacles = new List<GameObject>(); // 活跃障碍物列表
    private List<GameObject> activeBuildings = new List<GameObject>(); // 活跃建筑列表
    private float startZPosition; // 道路起始位置
    private int currentBlockIndex = 0; // 当前块索引

    void Start()
    {
        // 获取图层ID
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

    // 生成初始地面块
    void GenerateInitialBlocks()
    {
        for (int i = 0; i < initialBlocks; i++)
        {
            GenerateBlockAt(i);
        }
    }

    // 检查并生成新块
    void CheckAndGenerateBlocks()
    {
        float playerZ = player.position.z;
        float farthestBlockZ = startZPosition + currentBlockIndex * blockLength;

        // 当玩家接近可见范围末尾时生成新块
        if (playerZ > farthestBlockZ - (visibleBlocks * blockLength / 2))
        {
            GenerateNextBlock();
            DestroyOldestBlock();
        }
    }

    // 生成下一个地面块
    void GenerateNextBlock()
    {
        GenerateBlockAt(currentBlockIndex);
    }

    // 在指定索引生成块并设置图层和碰撞体
    void GenerateBlockAt(int index)
    {
        Vector3 position = new Vector3(
            transform.position.x,
            0f, // 地面Y坐标（调整为0更合适）
            startZPosition + index * blockLength
        );

        // 随机选择地面块预制体
        GameObject block = Instantiate(
            groundBlocks[Random.Range(0, groundBlocks.Length)],
            position,
            Quaternion.identity,
            transform
        );

        // 设置地面块图层为Ground（关键修改）
        SetLayerRecursively(block, groundLayer);

        // 添加或更新Box Collider（关键修改）
        SetupGroundCollider(block);

        activeBlocks.Add(block);
        currentBlockIndex++;

        // 生成障碍物
        if (index % obstacleDensity == 0 && obstacles.Length > 0)
        {
            GenerateObstacle(position);
        }

        // 生成建筑
        if (index % buildingDensity == 0 && buildings.Length > 0)
        {
            GenerateBuildings(position);
        }
    }

    // 递归设置图层（处理子物体）
    void SetLayerRecursively(GameObject obj, int layerIndex)
    {
        obj.layer = layerIndex;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layerIndex);
        }
    }

    // 设置地面块的Box Collider
    // 修改此处：地面块用 Mesh Collider
    void SetupGroundCollider(GameObject groundBlock)
    {
        // 移除旧碰撞体
        Collider[] oldColliders = groundBlock.GetComponents<Collider>();
        foreach (Collider collider in oldColliders)
        {
            Destroy(collider);
        }

        // 添加 Mesh Collider（地面块用）
        MeshCollider meshCollider = groundBlock.AddComponent<MeshCollider>();
        meshCollider.convex = false; // 非凸面，贴合复杂模型
    }

    // 障碍物保持 Box Collider（原逻辑不变）
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

        // 障碍物强制用 Box Collider
        BoxCollider boxCollider = obstacle.GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = obstacle.AddComponent<BoxCollider>();
        }

        activeObstacles.Add(obstacle);
    }

    // 销毁最旧的地面块及相关物体
    void DestroyOldestBlock()
    {
        if (activeBlocks.Count > visibleBlocks)
        {
            // 销毁地面块
            Destroy(activeBlocks[0]);
            activeBlocks.RemoveAt(0);

            // 销毁相关障碍物
            DestroyOldestObstacles();

            // 销毁相关建筑
            DestroyOldestBuildings();
        }
    }

    // 销毁最旧的障碍物
    void DestroyOldestObstacles()
    {
        // 销毁超出范围的障碍物
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

    // 生成建筑
    void GenerateBuildings(Vector3 blockPosition)
    {
        // 左侧建筑
        GameObject leftBuilding = Instantiate(
            buildings[Random.Range(0, buildings.Length)],
            blockPosition + new Vector3(-buildingOffset, 0, blockLength / 2),
            Quaternion.identity,
            transform
        );

        // 设置建筑图层
        SetLayerRecursively(leftBuilding, buildingLayer);

        // 确保建筑有碰撞体
        AddBuildingCollider(leftBuilding);

        activeBuildings.Add(leftBuilding);

        // 右侧建筑
        GameObject rightBuilding = Instantiate(
            buildings[Random.Range(0, buildings.Length)],
            blockPosition + new Vector3(buildingOffset, 0, blockLength / 2),
            Quaternion.Euler(0, 180, 0),
            transform
        );

        // 设置建筑图层
        SetLayerRecursively(rightBuilding, buildingLayer);

        // 确保建筑有碰撞体
        AddBuildingCollider(rightBuilding);

        activeBuildings.Add(rightBuilding);
    }

    // 为建筑添加碰撞体
    void AddBuildingCollider(GameObject building)
    {
        // 检查是否已有碰撞体
        Collider collider = building.GetComponent<Collider>();
        if (collider == null)
        {
            // 添加网格碰撞体（适合复杂模型）
            MeshCollider meshCollider = building.AddComponent<MeshCollider>();
            meshCollider.convex = false; // 非凸面，适合复杂形状
        }

        // 添加刚体使其能参与碰撞
        Rigidbody rb = building.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = building.AddComponent<Rigidbody>();
            rb.isKinematic = true; // 设为运动学刚体
            rb.useGravity = false; // 不受重力影响
        }
    }

    // 销毁最旧的建筑
    void DestroyOldestBuildings()
    {
        // 销毁超出范围的建筑
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