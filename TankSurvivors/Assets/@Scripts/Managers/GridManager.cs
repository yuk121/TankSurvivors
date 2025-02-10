using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Cell
{
    public HashSet<DropItemController> DropObject { get; } = new HashSet<DropItemController>();
}

public class GridManager : MonoBehaviour
{
    #region SimpleSingleTon
    public static GridManager Instance;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    #endregion
    Grid _grid;
    Dictionary<Vector3Int, Cell> _cellDic = new Dictionary<Vector3Int, Cell>();

    public void Init()
    {
        _grid = GetComponent<Grid>();
    }

    public void Add(DropItemController drop)
    {
        Vector3Int cellPos = _grid.WorldToCell(drop.transform.position);

        Cell cell = GetCell(cellPos);

        if (cell == null)
            return;

        cell.DropObject.Add(drop);
    }

    public void Remove(DropItemController drop)
    {
        Vector3Int cellPos = _grid.WorldToCell(drop.transform.position);

        Cell cell = GetCell(cellPos);

        if (cell == null)
            return;

        cell.DropObject.Remove(drop);
    }

    private Cell GetCell(Vector3Int cellPos)
    {
        Cell cell = null;

        if (_cellDic.TryGetValue(cellPos, out cell) == false)
        {
            cell = new Cell();
            _cellDic.Add(cellPos, cell);
        }

        return cell;
    }

    public List<DropItemController> GatherDropObjects(Vector3 pos, float range)
    {
        List<DropItemController> drops = new List<DropItemController>();

        Vector3Int left = _grid.WorldToCell(pos + new Vector3(-range, 0, 0));
        Vector3Int right = _grid.WorldToCell(pos + new Vector3(range, 0, 0));
        Vector3Int bottom = _grid.WorldToCell(pos + new Vector3(0, 0, -range));
        Vector3Int top = _grid.WorldToCell(pos + new Vector3(0, 0, range));

        int minX = left.x;
        int maxX = right.x;
        int minZ = bottom.z;
        int maxZ = top.z;

        for (int x = minX; x <= maxX; x++)
        {
            for (int z = minZ; z <= maxZ; z++)
            {
                if (_cellDic.ContainsKey(new Vector3Int(x, 0, z)) == false)
                    continue;

                drops.AddRange(_cellDic[new Vector3Int(x, 0, z)].DropObject);
            }
        }

        return drops;
    }

}
