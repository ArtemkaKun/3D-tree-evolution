using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CellTests
    {
        /*private GameObject _cellPrefab;
        private int _energy = 300;

        [UnityTest]
        public IEnumerator CellEnergyCalc()
        {
            _cellPrefab = Resources.Load("Prefabs/Cell") as GameObject;
            var new_cell = Object.Instantiate(_cellPrefab);
            new_cell.transform.position = new Vector3(0, 0, 0);
            new_cell.GetComponent<CellController>().CELL_USE_ENERGY = 13;
            new_cell.GetComponent<CellController>().SetEnergy(_energy);
            new_cell.GetComponent<CellController>().CellCalcEnergy();
            var next_energy = new_cell.GetComponent<CellController>().GetEnergy();
            Assert.AreEqual(305, next_energy);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator CellEnergyCalcWithOneAbove()
        {
            _cellPrefab = Resources.Load("Prefabs/Cell") as GameObject;
            var new_cell = Object.Instantiate(_cellPrefab);
            new_cell.transform.position = new Vector3(0, 0, 0);
            WorldController.AddCoords(new_cell.transform.position);
            
            _cellPrefab = Resources.Load("Prefabs/Cell") as GameObject;
            var new_cell_1 = Object.Instantiate(_cellPrefab);
            new_cell_1.transform.position = new Vector3(0, 1, 0);
            WorldController.AddCoords(new_cell_1.transform.position);
            
            new_cell.GetComponent<CellController>().CELL_USE_ENERGY = 13;
            new_cell.GetComponent<CellController>().SetEnergy(_energy);
            new_cell.GetComponent<CellController>().CellCalcEnergy();
            var next_energy = new_cell.GetComponent<CellController>().GetEnergy();
            Assert.AreEqual(299, next_energy);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator CellEnergyCalcWithNoSun()
        {
            _cellPrefab = Resources.Load("Prefabs/Cell") as GameObject;
            var new_cell = Object.Instantiate(_cellPrefab);
            new_cell.transform.position = new Vector3(0, 0, 0);
            WorldController.AddCoords(new_cell.transform.position);
            
            _cellPrefab = Resources.Load("Prefabs/Cell") as GameObject;
            var new_cell_1 = Object.Instantiate(_cellPrefab);
            new_cell_1.transform.position = new Vector3(0, 1, 0);
            WorldController.AddCoords(new_cell_1.transform.position);
            
            _cellPrefab = Resources.Load("Prefabs/Cell") as GameObject;
            var new_cell_2 = Object.Instantiate(_cellPrefab);
            new_cell_2.transform.position = new Vector3(0, 2, 0);
            WorldController.AddCoords(new_cell_2.transform.position);
            
            _cellPrefab = Resources.Load("Prefabs/Cell") as GameObject;
            var new_cell_3 = Object.Instantiate(_cellPrefab);
            new_cell_3.transform.position = new Vector3(0, 3, 0);
            WorldController.AddCoords(new_cell_3.transform.position);
            
            new_cell.GetComponent<CellController>().CELL_USE_ENERGY = 13;
            new_cell.GetComponent<CellController>().SetEnergy(_energy);
            new_cell.GetComponent<CellController>().CellCalcEnergy();
            var next_energy = new_cell.GetComponent<CellController>().GetEnergy();
            Assert.AreEqual(false, new_cell.GetComponent<CellController>()._isHaveSun);
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator CellGrowTest()
        {
            _cellPrefab = Resources.Load("Prefabs/Cell") as GameObject;
            var new_cell = Object.Instantiate(_cellPrefab);
            new_cell.transform.position = new Vector3(0, 0, 0);
            WorldController.AddCoords(new_cell.transform.position);
            
            _cellPrefab = Resources.Load("Prefabs/Cell") as GameObject;
            var new_cell_1 = Object.Instantiate(_cellPrefab);
            new_cell_1.transform.position = new Vector3(0, 1, 0);
            WorldController.AddCoords(new_cell_1.transform.position);

            new_cell.GetComponent<CellController>().CELL_USE_ENERGY = 13;
            new_cell.GetComponent<CellController>().SetEnergy(_energy);
            new_cell.GetComponent<CellController>().CellCalcEnergy();
            var next_energy = new_cell.GetComponent<CellController>().GetEnergy();
            Assert.AreEqual(false, new_cell.GetComponent<CellController>()._isHaveSun);
            yield return null;
        }*/
    }
}
