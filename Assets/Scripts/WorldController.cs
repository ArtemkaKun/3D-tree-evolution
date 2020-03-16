using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldController : MonoBehaviour
{
    [SerializeField] private int START_FOREST_SIZE = 12;

    private static int _indexer;
    private static int _worldAge;
    private static int _generation;

    private GameObject _treePrefab;

    private static readonly List<GameObject> _trees = new List<GameObject>();
    private static readonly List<Vector3> _coordMap = new List<Vector3>();

    private void Awake()
    {
        _treePrefab = Resources.Load("Prefabs/Tree") as GameObject;
    }

    private async Task Start()
    {
        for (var i = 0; i < START_FOREST_SIZE; i++)
        {
            TreeSpawner();
        }

        await MainLoop();
    }

    private void TreeSpawner()
    {
        var rand_pos = Random.insideUnitSphere * 10;
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

            try
            {
                await Task.WhenAll(buffer_forest.Select(x => x.GetComponent<TreeController>().TreeMainLoop())
                    .ToArray());
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }

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
}