using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CellController : MonoBehaviour
{
    [SerializeField] private int CELL_USE_ENERGY = 13;
    [SerializeField] private int GENES_COUNT = 6;

    private int _energy;

    private int[] _gene;

    private bool _isHaveSun = true;
    private bool _isSeed;

    private GameObject _cellPrefab;
    private GameObject _treePrefab;

    private TreeController _treeController;

    private Material _woodMaterial;
    private Material _seedMaterial;

    private List<int[]> _genes = new List<int[]>();

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
            /*var hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.up), 1f);
            if (hits.Length == 0)
            {
                var new_seed = Instantiate(_cellPrefab, gameObject.transform.parent);
                new_seed.name = "Cell" + WorldController.GetIndexer();
                WorldController.IncreaseIndexer();
                _treeController.AddNewCell(new_seed);

                var pos = transform.position;
                new_seed.transform.position = new Vector3(pos.x, pos.y + 1, pos.z);

                var cellController = new_seed.GetComponent<CellController>();
                cellController.SetSeed();
                cellController.SetGen(_treeController.GetGenes()[new_branch.Up]);
                cellController.SetGenes(_treeController.GetGenes());

                _isSeed = false;
                gameObject.GetComponent<Renderer>().material = _woodMaterial;
            }*/
        }

        if (new_branch.Down < GENES_COUNT)
        {
            GrowOneCell(Vector3.down, new_branch.Down);
            /*var hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.down), 1f);
            if (hits.Length == 0)
            {
                var new_seed = Instantiate(_cellPrefab, gameObject.transform.parent);
                new_seed.name = "Cell" + WorldController.GetIndexer();
                WorldController.IncreaseIndexer();
                _treeController.AddNewCell(new_seed);

                var pos = transform.position;
                new_seed.transform.position = new Vector3(pos.x, pos.y - 1, pos.z);

                var cellController = new_seed.GetComponent<CellController>();
                cellController.SetSeed();
                cellController.SetGen(_treeController.GetGenes()[new_branch.Down]);
                cellController.SetGenes(_treeController.GetGenes());

                _isSeed = false;
                gameObject.GetComponent<Renderer>().material = _woodMaterial;
            }*/
        }

        if (new_branch.Forward < GENES_COUNT)
        {
            GrowOneCell(Vector3.forward, new_branch.Forward);
            /*var hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), 1f);
            if (hits.Length == 0)
            {
                var new_seed = Instantiate(_cellPrefab, gameObject.transform.parent);
                new_seed.name = "Cell" + WorldController.GetIndexer();
                WorldController.IncreaseIndexer();
                _treeController.AddNewCell(new_seed);

                var pos = transform.position;
                new_seed.transform.position = new Vector3(pos.x, pos.y, pos.z + 1);

                var cellController = new_seed.GetComponent<CellController>();
                cellController.SetSeed();
                cellController.SetGen(_treeController.GetGenes()[new_branch.Forward]);
                cellController.SetGenes(_treeController.GetGenes());

                _isSeed = false;
                gameObject.GetComponent<Renderer>().material = _woodMaterial;
            }*/
        }

        if (new_branch.Back < GENES_COUNT)
        {
            GrowOneCell(Vector3.back, new_branch.Back);
            /*var hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.back), 1f);
            if (hits.Length == 0)
            {
                var new_seed = Instantiate(_cellPrefab, gameObject.transform.parent);
                new_seed.name = "Cell" + WorldController.GetIndexer();
                WorldController.IncreaseIndexer();
                _treeController.AddNewCell(new_seed);

                var pos = transform.position;
                new_seed.transform.position = new Vector3(pos.x, pos.y, pos.z - 1);

                var cellController = new_seed.GetComponent<CellController>();
                cellController.SetSeed();
                cellController.SetGen(_treeController.GetGenes()[new_branch.Back]);
                cellController.SetGenes(_treeController.GetGenes());

                _isSeed = false;
                gameObject.GetComponent<Renderer>().material = _woodMaterial;
            }*/
        }

        if (new_branch.Left < GENES_COUNT)
        {
            GrowOneCell(Vector3.left, new_branch.Left);
            /*var hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.left), 1f);
            if (hits.Length == 0)
            {
                var new_seed = Instantiate(_cellPrefab, gameObject.transform.parent);
                new_seed.name = "Cell" + WorldController.GetIndexer();
                WorldController.IncreaseIndexer();
                _treeController.AddNewCell(new_seed);

                var pos = transform.position;
                new_seed.transform.position = new Vector3(pos.x - 1, pos.y, pos.z);

                var cellController = new_seed.GetComponent<CellController>();
                cellController.SetSeed();
                cellController.SetGen(_treeController.GetGenes()[new_branch.Left]);
                cellController.SetGenes(_treeController.GetGenes());

                _isSeed = false;
                gameObject.GetComponent<Renderer>().material = _woodMaterial;
            }*/
        }

        if (new_branch.Right < GENES_COUNT)
        {
            GrowOneCell(Vector3.right, new_branch.Right);
            /*var hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.right), 1f);
            if (hits.Length == 0)
            {
                var new_seed = Instantiate(_cellPrefab, gameObject.transform.parent);
                new_seed.name = "Cell" + WorldController.GetIndexer();
                WorldController.IncreaseIndexer();
                _treeController.AddNewCell(new_seed);

                var pos = transform.position;
                new_seed.transform.position = new Vector3(pos.x + 1, pos.y, pos.z);

                var cellController = new_seed.GetComponent<CellController>();
                cellController.SetSeed();
                cellController.SetGen(_treeController.GetGenes()[new_branch.Right]);
                cellController.SetGenes(_treeController.GetGenes());

                _isSeed = false;
                gameObject.GetComponent<Renderer>().material = _woodMaterial;
            }*/
        }

        await Task.Delay(100);
    }

    private void GrowOneCell(Vector3 grow_direction, int one_gene)
    {
        var obj_transform = transform;
        
        var hits = Physics.RaycastAll(obj_transform.position, obj_transform.TransformDirection(grow_direction), 1f);
        if (hits.Length == 0)
        {
            var new_seed = Instantiate(_cellPrefab, obj_transform.parent);
            new_seed.name = "Cell" + WorldController.GetIndexer();
            WorldController.IncreaseIndexer();
            _treeController.AddNewCell(new_seed);
            
            new_seed.transform.position = obj_transform.position + grow_direction;

            var cellController = new_seed.GetComponent<CellController>();
            cellController.SetSeed();
            cellController.SetGen(_treeController.GetGenes()[one_gene]);
            cellController.SetGenes(_treeController.GetGenes());

            _isSeed = false;
            gameObject.GetComponent<Renderer>().material = _woodMaterial;
        }
    }
    
    private async Task CellCalcEnergy()
    {
        _energy -= CELL_USE_ENERGY;

        if (_isHaveSun)
        {
            var cell_lvl = CheckSun();

            if (_isHaveSun)
            {
                _energy = (3 - cell_lvl) * Mathf.RoundToInt(transform.position.y + 6);
            }
        }

        await Task.Delay(1);
    }

    private int CheckSun()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.up), 5f);

        if (hits.Length > 2)
        {
            _isHaveSun = false;
        }

        return hits.Length;
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
                else if (Math.Abs(transform.position.y - other.transform.position.y) < 1)
                {
                    Destroy(other.gameObject);
                    Destroy(gameObject);
                }
            }

            /*if (!(transform.position.y > other.transform.position.y)) return;
            if (transform.parent == null)
            {
                Destroy(gameObject);
            }*/
        }
        else
        {
            var new_tree = Instantiate(_treePrefab);
            new_tree.name = "Tree" + WorldController.GetIndexer();
            WorldController.IncreaseIndexer();
            WorldController.AddTree(new_tree);

            new_tree.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            new_tree.GetComponent<TreeController>().SetGenes(_genes);
            Destroy(gameObject);
        }
    }
}