
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileInside
{
    public GameObject tileObstacle;
    public List<GameObject> tileObstacleCoins;
}

public class TriggerBasedActivation : MonoBehaviour
{
    public List<TileInside> tileInsides;
    public int lastActiveInsideNumber;

    public void GenerateTile()
    {
        int randomObstacleNumber = Random.Range(0, tileInsides.Count);
        lastActiveInsideNumber = randomObstacleNumber;
        tileInsides[randomObstacleNumber].tileObstacle.SetActive(true);
    }

    public void UnGenerateTile()
    {
        tileInsides[lastActiveInsideNumber].tileObstacle.SetActive(false);
        ObstacleIsActiveCoins(true);

    }

    public void ObstacleIsActiveCoins(bool isActive)
    {
        for (int i = 0; i < tileInsides[lastActiveInsideNumber].tileObstacleCoins.Count; i++)
        {
            tileInsides[lastActiveInsideNumber].tileObstacleCoins[i].SetActive(isActive);
        }
    }
}



