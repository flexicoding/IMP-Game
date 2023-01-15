using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GenerateField : MonoBehaviour
{
    #region listInList
    [System.Serializable]
    public class Asteroids
    {
        public List<GameObject> asteroids;
    }
    public List<Asteroids> asteroidsList;
    #endregion

    string[] positionsString;
    private static List<Vector3> positionsVectors = new List<Vector3>();
    string myFilePath, fileName;

    private GameObject randomAsteroid;

    void Start()
    {
        int j = 0;
        foreach(Transform child in GameObject.Find("AsteroidPrefabs").transform)
        {
            for(int k = child.transform.childCount - 1; k >= 0; k--)
            {
                asteroidsList[j].asteroids.Add(child.transform.GetChild(k).gameObject);
            }
            j++;
        }
        Destroy(GameObject.Find("AsteroidPrefabs"));

        fileName = "positions.txt";
        myFilePath = Application.dataPath + "/" + fileName;
        positionsString = File.ReadAllLines(myFilePath);
        int i = 0;
        foreach (string line in positionsString)
        {
            StringToVector3(positionsString[i]);
            //print(i);
            i++;
        }
    }

    public Vector3 StringToVector3(string sVector)
    {
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        string[] sArray = sVector.Split(' ');

        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2])) /1000;

        //print(result);
        positionsVectors.Add(result);
        randomAsteroidGenerator();
        //GameObject newObj = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), result, Quaternion.identity, gameObject.transform);
        GameObject newObj = Instantiate(randomAsteroid, result, Quaternion.identity, gameObject.transform);
        newObj.transform.localScale = newObj.transform.localScale * 4.5f;

        return result;
    }

    private void randomAsteroidGenerator()
    {
        float randomNum = Random.Range(0f, 100f);
        int myNum = 0;

        if(randomNum <= 5) myNum = 0;
        else if(randomNum <= 30 && randomNum > 5) myNum = 1;
        else if (randomNum < 70 && randomNum > 30) myNum = 2;
        else if (randomNum < 95 && randomNum > 70) myNum = 3;
        else if (randomNum < 100 && randomNum > 95) myNum = 4;

        randomAsteroid = asteroidsList[myNum].asteroids[Random.Range(0,asteroidsList[myNum].asteroids.Count)];

    }
}
