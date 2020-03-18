using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldController : MonoBehaviour
{
    private static int _indexer;
    private static int _worldAge;
    private static int _generation;

    private static GameObject _treePrefab;

    private static readonly List<GameObject> _trees = new List<GameObject>();
    private static readonly List<Vector3> _coordMap = new List<Vector3>();

    private static int _groundLength;
    private static int _groundWidth;
    private static int _maxTreeAge;
    private static int _startEnergy;
    private static int _genesCount;
    private static int _cellUsage;
    private static int _startForestSize;

    private void Awake()
    {
        _treePrefab = Resources.Load("Prefabs/Tree") as GameObject;
    }

    public async Task StartWorld()
    {
        transform.localScale = new Vector3(_groundLength, 1, _groundWidth);

        for (var i = 0; i < _startForestSize; i++)
        {
            TreeSpawner();
        }

        await MainLoop();
    }

    private static void TreeSpawner()
    {
        var rand_pos = Random.insideUnitSphere * _startForestSize / 2;
        rand_pos = new Vector3(Mathf.Round(rand_pos.x), 0, Mathf.Round(rand_pos.z));

        var new_tree = Instantiate(_treePrefab);
        new_tree.name = "Tree" + _indexer;
        ++_indexer;
        _trees.Add(new_tree);

        new_tree.transform.localPosition = rand_pos;
    }

    private async Task MainLoop()
    {
        while (_worldAge < 1000)
        {
            ++_worldAge;

            var buffer_forest = new List<GameObject>();

            foreach (var one_tree in _trees)
            {
                buffer_forest.Add(one_tree);
            }

            await Task.WhenAll(buffer_forest.Select(x => x.GetComponent<TreeController>().TreeMainLoop())
                .ToArray());

            await Task.Delay(1);
        }
    }

    public static void RemoveTree(GameObject tree)
    {
        _trees.Remove(tree);
    }

    public static void AddTree(GameObject tree)
    {
        _trees.Add(tree);
    }

    public static void IncreaseGeneration()
    {
        ++_generation;
    }

    public static void IncreaseIndexer()
    {
        ++_indexer;
    }

    public static int GetIndexer()
    {
        return _indexer;
    }

    public static int GetYear()
    {
        return _worldAge;
    }

    public static int GetGeneration()
    {
        return _generation;
    }

    public static int GetForestSize()
    {
        return _trees.Count;
    }

    public static void AddCoords(Vector3 new_coord)
    {
        _coordMap.Add(new_coord);
    }

    public static void RemoveCoords(Vector3 new_coord)
    {
        _coordMap.Remove(new_coord);
    }

    public static bool CheckCoords(Vector3 need_coord)
    {
        return _coordMap.Contains(need_coord);
    }

    public static void SetGroundLength(int new_data)
    {
        _groundLength = new_data;
    }

    public static void SetGroundWidth(int new_data)
    {
        _groundWidth = new_data;
    }

    public static void SetMaxAge(int new_data)
    {
        _maxTreeAge = new_data;
    }

    public static void SetStartEnergy(int new_data)
    {
        _startEnergy = new_data;
    }

    public static void SetGenesCount(int new_data)
    {
        _genesCount = new_data;
    }

    public static void SetCellUsage(int new_data)
    {
        _cellUsage = new_data;
    }

    public static void SetStartForestSize(int new_data)
    {
        _startForestSize = new_data;
    }

    public static float GetMaxX()
    {
        return _groundLength / 2 - 0.5f;
    }
    
    public static float GetMaxZ()
    {
        return _groundWidth / 2 - 0.5f;
    }

    public static int GetMaxAge()
    {
        return _maxTreeAge;
    }
    
    public static int GetStartEnergy()
    {
        return _startEnergy;
    }
    
    public static int GetGenesCount()
    {
        return _genesCount;
    }
    
    public static int GetCellUsage()
    {
        return _cellUsage;
    }
}