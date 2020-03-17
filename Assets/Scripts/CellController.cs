using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UIElements;

public class CellController : MonoBehaviour
{
    [SerializeField] private int CELL_USE_ENERGY = 13;
    [SerializeField] private int GENES_COUNT = 6;
    
    [SerializeField] private float MAX_X = 29.5f;
    [SerializeField] private float MAX_Z = 29.5f;

    private int _energy;

    private int[] _gene;

    private bool _isHaveSun = true;
    private bool _isSeed;

    private GameObject _cellPrefab;
    private GameObject _treePrefab;

    private TreeController _treeController;

    private Material _woodMaterial;
    private Material _seedMaterial;

    private readonly List<int[]> _genes = new List<int[]>();

    private struct _GrowDirection
    {
        public int Up;
        public int Down;
        public int Forward;
        public int Back;
        public int Left;
        public int Right;
    }

    private void Awake()
    {
        _treeController = gameObject.GetComponentInParent<TreeController>();
        _woodMaterial = Resources.Load("Materials/Wood") as Material;
        _seedMaterial = Resources.Load("Materials/Seed") as Material;
        _cellPrefab = Resources.Load("Prefabs/Cell") as GameObject;
        _treePrefab = Resources.Load("Prefabs/Tree") as GameObject;

        gameObject.GetComponent<Renderer>().material = _seedMaterial;
    }

    public async Task CellMainLoop()
    {
        await CellCalcEnergy();

        if (_energy >= 18 && _isSeed)
        {
            _energy -= 18;

            await GrowNewCell();
        }
    }

    [BurstCompile]
    private struct JobCalcEnergy : IJob
    {
        public NativeArray<int> energy;

        public int cell_lvl;
        public int cell_y_coord;

        public void Execute()
        {
            energy[0] = (cell_lvl) * Mathf.RoundToInt(cell_y_coord + 6);
        }
    }

    private async Task CellCalcEnergy()
    {
        _energy -= CELL_USE_ENERGY;

        var cell_lvl = CheckSun();

        if (_isHaveSun)
        {
            var new_energy = new NativeArray<int>(1, Allocator.TempJob);
            var job = new JobCalcEnergy()
            {
                energy = new_energy,
                cell_lvl = cell_lvl,
                cell_y_coord = Mathf.RoundToInt(transform.position.y)
            };
            var handler = job.Schedule();
            handler.Complete();
            _energy += new_energy[0];
            new_energy.Dispose();
        }

        await Task.Delay(1);
    }

    //[BurstCompile]
    private struct JobCheckSun : IJobParallelForTransform
    {
        public NativeArray<int> have_sun;
        public NativeArray<int> cell_lvl;

        public void Execute(int index, TransformAccess transform)
        {
            var pos = transform.position;
            var lvl = 0;
            
            for (var i = 0; i < 3; i++)
            {
                pos += Vector3.up;
                if (!WorldController.CheckCoords(pos))
                {
                    ++lvl;
                }
            }
            
            if (lvl == 0)
            {
                have_sun[0] = 0;
            }
            else
            {
                have_sun[0] = 1;
            }

            cell_lvl[0] = lvl;
        }
    }

    private int CheckSun()
    {
        /*var new_sun = new NativeArray<int>(1, Allocator.TempJob);
        var cell_lvl = new NativeArray<int>(1, Allocator.TempJob);
        
        var job = new JobCheckSun()
        {
            have_sun = new_sun,
            cell_lvl = cell_lvl
        };
        
        var transforms = new Transform[]
        {
            transform
        };
        
        var transAccArr = new TransformAccessArray(transforms);
        
        var handler = job.Schedule(transAccArr);
        handler.Complete();

        if (new_sun[0] == 0)
        {
            _isHaveSun = false;
        }
        else
        {
            _isHaveSun = true;
        }

        var lvl = cell_lvl[0];
        transAccArr.Dispose();
        new_sun.Dispose();
        cell_lvl.Dispose();*/
        
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.up), Mathf.Infinity);
        if (hits.Length > 2)
        {
            _isHaveSun = false;
        }

        return 3 - hits.Length;
    }

    private async Task GrowNewCell()
    {
        var new_branch = new _GrowDirection()
        {
            Up = _gene[0],
            Down = _gene[1],
            Forward = _gene[2],
            Back = _gene[3],
            Left = _gene[4],
            Right = _gene[5]
        };

        if (new_branch.Up < GENES_COUNT)
        {
            GrowOneCell(Vector3.up, new_branch.Up);
        }

        if (new_branch.Down < GENES_COUNT)
        {
            GrowOneCell(Vector3.down, new_branch.Down);
        }

        if (new_branch.Forward < GENES_COUNT)
        {
            GrowOneCell(Vector3.forward, new_branch.Forward);
        }

        if (new_branch.Back < GENES_COUNT)
        {
            GrowOneCell(Vector3.back, new_branch.Back);
        }

        if (new_branch.Left < GENES_COUNT)
        {
            GrowOneCell(Vector3.left, new_branch.Left);
        }

        if (new_branch.Right < GENES_COUNT)
        {
            GrowOneCell(Vector3.right, new_branch.Right);
        }

        await Task.Delay(1);
    }

    private void GrowOneCell(Vector3 grow_direction, int one_gene)
    {
        var obj_transform = transform;
        var new_coord = obj_transform.position + grow_direction;

        if (!WorldController.CheckCoords(new_coord) && Math.Abs(new_coord.x) <= MAX_X && Math.Abs(new_coord.z) <= MAX_Z && new_coord.y >= 0)
        {
            var new_seed = Instantiate(_cellPrefab, obj_transform.parent);
            new_seed.name = "Cell" + WorldController.GetIndexer();
            WorldController.IncreaseIndexer();
            _treeController.AddNewCell(new_seed);

            WorldController.AddCoords(new_coord);
            new_seed.transform.position = new_coord;

            var cellController = new_seed.GetComponent<CellController>();
            cellController.SetSeed();
            cellController.SetGen(_treeController.GetGenes()[one_gene]);
            cellController.SetGenes(_treeController.GetGenes());

            _isSeed = false;
            gameObject.GetComponent<Renderer>().material = _woodMaterial;
        }
    }

    public int GetEnergy()
    {
        return _energy;
    }

    public void SetEnergy(int new_energy)
    {
        _energy = new_energy;
    }

    public void SetSeed()
    {
        _isSeed = true;
    }

    public void SetGen(int[] gen)
    {
        _gene = gen;
    }

    public void SetGenes(List<int[]> new_genes)
    {
        foreach (var one_gene in new_genes)
        {
            _genes.Add(one_gene);
        }
    }

    public bool CheckIsSeed()
    {
        return _isSeed;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Abyss")
        {
            Destroy(gameObject);
            return;
        }

        if (other.gameObject.name != "Ground")
        {
            if (transform.parent == null)
            {
                if (other.gameObject.transform.parent != null)
                {
                    Destroy(gameObject);
                }
                else if (transform.position.y > other.transform.position.y)
                {
                    Destroy(gameObject);
                }
                /*else if (Math.Abs(transform.position.y - other.transform.position.y) < 1)
                {
                    Destroy(other.gameObject);
                    Destroy(gameObject);
                }*/
            }
        }
        else
        {
            var new_tree = Instantiate(_treePrefab);
            new_tree.name = "Tree" + WorldController.GetIndexer();
            WorldController.IncreaseIndexer();
            WorldController.AddTree(new_tree);

            var pos = transform.position;
            new_tree.transform.position = new Vector3(pos.x, 0, pos.z);
            new_tree.GetComponent<TreeController>().SetGenes(_genes);
            Destroy(gameObject);
        }
    }
}