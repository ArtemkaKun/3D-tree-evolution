using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    [SerializeField] private int MAX_TREE_AGE = 100;
    [SerializeField] private int GENES_COUNT = 6;
    [SerializeField] private int START_SEED_ENERGY = 300;

    private int _treeAge;
    private int _treeEnergy;

    private GameObject _cellPrefab;

    private List<int[]> _treeGenes = new List<int[]>();
    private readonly List<GameObject> _treeCells = new List<GameObject>();

    private void Awake()
    {
        _cellPrefab = Resources.Load("Prefabs/Cell") as GameObject;
    }

    public void Start()
    {
        if (_treeGenes.Count == 0)
        {
            GenomeInit();
        }

        RootFormer();
    }

    private void GenomeInit()
    {
        var time = DateTime.Now;
        var random =
            new System.Random(time.Hour + time.Minute + time.Second + time.Millisecond + name[name.Length - 1]);

        var max_random_edge = GENES_COUNT * 2 - 1;

        for (var gene = 0; gene < GENES_COUNT; ++gene)
        {
            var rand_gene = new[]
            {
                random.Next(0, max_random_edge), random.Next(0, max_random_edge), random.Next(0, max_random_edge),
                random.Next(0, max_random_edge), random.Next(0, max_random_edge), random.Next(0, max_random_edge)
            };

            _treeGenes.Add(rand_gene);
        }
    }

    private void RootFormer()
    {
        var root = Instantiate(_cellPrefab, transform);
        root.name = "Cell" + WorldController.GetIndexer();
        WorldController.IncreaseIndexer();
        _treeCells.Add(root);

        var new_coord = transform.position;
        WorldController.AddCoords(new_coord);
        root.transform.position = new_coord;

        var cell_controller = root.GetComponent<CellController>();
        cell_controller.SetSeed();
        cell_controller.SetEnergy(START_SEED_ENERGY);
        cell_controller.SetGen(_treeGenes[0]);
        cell_controller.SetGenes(_treeGenes);
    }

    public async Task TreeMainLoop()
    {
        ++_treeAge;

        TreeEnergyCalc();

        if (_treeAge >= MAX_TREE_AGE || _treeEnergy < 0)
        {
            WorldController.RemoveTree(gameObject);
            DestroyTree();
            Destroy(gameObject);
            await Task.Delay(1);
        }
        else
        {
            var buffer_tree = new List<GameObject>();

            foreach (var one_cell in _treeCells)
            {
                buffer_tree.Add(one_cell);
            }

            if (buffer_tree.Count > 0)
            {
                await Task.WhenAll(buffer_tree.Select(x => x.GetComponent<CellController>().CellMainLoop())
                    .ToArray());
            }
        }
    }

    private void DestroyTree()
    {
        var buffer_tree = new List<GameObject>();

        foreach (var one_cell in _treeCells)
        {
            buffer_tree.Add(one_cell);
        }

        foreach (var one_cell in buffer_tree)
        {
            if (!one_cell.GetComponent<CellController>().CheckIsSeed())
            {
                _treeCells.Remove(one_cell);
                WorldController.RemoveCoords(one_cell.transform.position);
                Destroy(one_cell);
            }
            else
            {
                _treeCells.Remove(one_cell);
                Mutate();
                NewTree(one_cell);
            }
        }

        _treeCells.Clear();
    }

    private struct JobMutate : IJob
    {
        public int last_name_symbol;
        public int GENES_COUNT;
        public NativeArray<int> new_gene;
        public NativeArray<int> mutable_gen;
        public NativeArray<int> mutable_dir;

        public void Execute()
        {
            var time = DateTime.Now;
            var gene_mutate = new System.Random(time.Hour + time.Minute + time.Second + last_name_symbol);

            if (gene_mutate.Next(0, 100) <= 5)
            {
                var max_random_enge = GENES_COUNT * 2 - 1;
                mutable_gen[0] = gene_mutate.Next(0, GENES_COUNT - 1);
                mutable_dir[0] = gene_mutate.Next(0, 5);
                new_gene[0] = gene_mutate.Next(0, max_random_enge);
                //_treeGenes[mutable_gen][mutable_dir] = gene_mutate.Next(0, max_random_enge);
                WorldController.IncreaseGeneration();
            }
        }
    }

    private void Mutate()
    {
        var new_gene = new NativeArray<int>(1, Allocator.TempJob);
        var mutable_gen = new NativeArray<int>(1, Allocator.TempJob);
        var mutable_dir = new NativeArray<int>(1, Allocator.TempJob);

        var job = new JobMutate()
        {
            last_name_symbol = name[name.Length - 1],
            GENES_COUNT = GENES_COUNT,
            new_gene = new_gene,
            mutable_gen = mutable_gen,
            mutable_dir = mutable_dir
        };

        var handler = job.Schedule();
        handler.Complete();
        _treeGenes[mutable_gen[0]][mutable_dir[0]] = new_gene[0];

        mutable_gen.Dispose();
        mutable_dir.Dispose();
        new_gene.Dispose();

        /*var time = DateTime.Now;
        var gene_mutate = new System.Random(time.Hour + time.Minute + time.Second + name[name.Length - 1]);

        if (gene_mutate.Next(0, 100) <= 15)
        {
            var max_random_enge = GENES_COUNT * 2 - 1;
            var mutable_gen = gene_mutate.Next(0, GENES_COUNT - 1);
            var mutable_dir = gene_mutate.Next(0, 5);
            _treeGenes[mutable_gen][mutable_dir] = gene_mutate.Next(0, max_random_enge);
            WorldController.IncreaseGeneration();
        }*/
    }

    private void NewTree(GameObject one_cell)
    {
        one_cell.GetComponent<CellController>().SetGenes(_treeGenes);
        one_cell.transform.SetParent(null);
        WorldController.RemoveCoords(one_cell.transform.position);
        one_cell.AddComponent<Rigidbody>();
        one_cell.GetComponent<Rigidbody>().constraints =
            RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotation;
    }

    private void TreeEnergyCalc()
    {
        _treeEnergy = 0;

        foreach (var one_tree in _treeCells)
        {
            _treeEnergy += one_tree.GetComponent<CellController>().GetEnergy();
        }
    }

    public void AddNewCell(GameObject cell)
    {
        _treeCells.Add(cell);
    }

    public List<int[]> GetGenes()
    {
        return _treeGenes;
    }

    public void SetGenes(List<int[]> new_genes)
    {
        _treeGenes = new_genes;
    }
}