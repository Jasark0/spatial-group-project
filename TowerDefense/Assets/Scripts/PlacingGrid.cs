using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

class PlacingGrid : MonoBehaviour
{
    [SerializeField] GameObject buildingPrefab, enemyPrefab;

    [SerializeField] Tilemap buildingTileMap, enemyTileMap;

    [SerializeField] List<Vector3> buildingLocations, enemyLocations;

    void Start()
    {
        buildingLocations = GetPlacedTileWorldPositions(buildingTileMap);
        enemyLocations = GetPlacedTileWorldPositions(enemyTileMap);

        SpawnBuildings();
        InvokeRepeating(
            methodName: "SpawnEnemy",
            time: Random.Range(1, 4),
            repeatRate: Random.Range(2, 5));
    }

    List<Vector3> GetPlacedTileWorldPositions(Tilemap tilemap)
    {
        List<Vector3> positions = new List<Vector3>();

        // Loop through all potential tile positions within tilemap bounds
        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))  // Check if a tile exists
            {
                Vector3 worldPos = tilemap.CellToWorld(pos);
                positions.Add(worldPos);
            }
        }

        return positions;
    }

    private void SpawnBuildings()
    {
        foreach (Vector3 pos in buildingLocations)
        {
            Instantiate(
                buildingPrefab,
                new Vector3(
                    x: pos.x + .5f,
                    y: pos.y + .5f,
                    z: pos.z
                ),
                Quaternion.identity
            );
        }
    }

    private void SpawnEnemy()
    {
        int i = Random.Range(0, enemyLocations.Count);

        Instantiate(
            enemyPrefab,
            new Vector3(
                x: enemyLocations[i].x + .5f,
                y: enemyLocations[i].y + .5f,
                z: enemyLocations[i].z
            ),
            Quaternion.identity
        );
    }
}
