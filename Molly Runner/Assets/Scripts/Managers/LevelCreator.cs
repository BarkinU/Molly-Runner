using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    public int level;

    [Space]
    [Space]
    [Header("                                                  Options")]
    public GameObject optionsPrefab;
    public enum OptionsChooseIndex
    {
        left, right, both
    }
    public OptionsChooseIndex optionChoose;
    public List<Transform> optionSpawnPoints;
    public int levelOptionCount = 5;
    public int tempOptionSpawnPointIndex;

    [Space]
    [Space]
    [Header("                                                  Obstacles")]
    public int levelObstacleCount = 6;
    public int tempObstaclePointIndex;
    public int tempObstacleToSpawnIndex;
    public List<GameObject> obstacles;
    public List<Transform> obstaclePoints;


    public void BuildLevel()
    {
        BuildObstacles();
    }

    private void BuildObstacles()
    {
        level++;

        GameObject levelParent = new GameObject();
        levelParent.transform.position = Vector3.zero;
        levelParent.name = "level" + level;

        #region Obstacles

        var tempObstacleList = new List<GameObject>(obstacles);
        var tempObstaclePointList = new List<Transform>(obstaclePoints);

        // create parent for obstacles
        GameObject obstacleParent = new GameObject();
        obstacleParent.transform.position = Vector3.zero;
        obstacleParent.name = "ObstacleParent";

        for (int i = 0; i < levelObstacleCount; i++)
        {
            // get random value to spawn random objects
            tempObstacleToSpawnIndex = Random.Range(0, tempObstacleList.Count);
            tempObstaclePointIndex = Random.Range(0, tempObstaclePointList.Count);

            // Spawn obstacles
            Instantiate(tempObstacleList[tempObstacleToSpawnIndex], tempObstaclePointList[tempObstaclePointIndex].position + new Vector3(0, +tempObstacleList[tempObstacleToSpawnIndex].transform.localPosition.y, 0), Quaternion.identity, obstacleParent.transform);

            // remove spawned object from temp list
            tempObstacleList.RemoveAt(tempObstacleToSpawnIndex);
            tempObstaclePointList.RemoveAt(tempObstaclePointIndex);
        }
        #endregion

        #region Options
        // hold a temporary copy of spawn points of options
        var tempOptionSpawnPointList = new List<Transform>(optionSpawnPoints);

        // create parent for options
        GameObject optionsParent = new GameObject();
        optionsParent.transform.position = Vector3.zero;
        optionsParent.name = "OptionsParent";

        for (int i = 0; i < levelOptionCount; i++)
        {
            // get random value to spawn random objects
            optionChoose = (OptionsChooseIndex)Random.Range(0, 3);
            tempOptionSpawnPointIndex = Random.Range(0, tempOptionSpawnPointList.Count);

            switch (optionChoose)
            {
                //disables right door & instantiates
                case OptionsChooseIndex.left:

                    GameObject opt1 = Instantiate(optionsPrefab, tempOptionSpawnPointList[tempOptionSpawnPointIndex].position, Quaternion.identity, optionsParent.transform);
                    opt1.transform.GetChild(0).gameObject.SetActive(false);

                    break;
                //disables left door & instantiates
                case OptionsChooseIndex.right:

                    GameObject opt2 = Instantiate(optionsPrefab, tempOptionSpawnPointList[tempOptionSpawnPointIndex].position, Quaternion.identity, optionsParent.transform);
                    opt2.transform.GetChild(1).gameObject.SetActive(false);

                    break;
                //instantiates option
                case OptionsChooseIndex.both:

                    Instantiate(optionsPrefab, tempOptionSpawnPointList[tempOptionSpawnPointIndex].position, Quaternion.identity, optionsParent.transform);

                    break;
            }
            tempOptionSpawnPointList.RemoveAt(tempOptionSpawnPointIndex);
        }
        #endregion

        obstacleParent.transform.SetParent(levelParent.transform);
        optionsParent.transform.SetParent(levelParent.transform);
    }

    public void ClearLevel()
    {
        if (GameObject.Find("level" + level) != null)
        {
            DestroyImmediate(GameObject.Find("level" + level));
            level--;
        }
    }

}
