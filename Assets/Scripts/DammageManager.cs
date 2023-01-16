using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DammageManager : MonoBehaviour
{
    //public List<GameObject> crystals = new List<GameObject>();
    public GameObject crystal;
    public GameObject shatteredAsteroid;
    public List<int> asteroidsHP = new List<int>();


    private Dictionary<GameObject, int> damagedAsteroids = new Dictionary<GameObject, int>();
    private float randomCrystalScale;
    private int randomCrystalValue;

    public void dammaged(GameObject damagedObj, int damage, string objTag)
    {
        if(damagedObj.CompareTag("tiny"))
        {
            try
            {
                damagedAsteroids[damagedObj] -= damage;
            }
            catch(KeyNotFoundException)
            {
                damagedAsteroids.Add(damagedObj, asteroidsHP[0] - damage);
            }
            if (damagedAsteroids[damagedObj] <= 0)
            {
                GameObject asteroidVar = Instantiate(shatteredAsteroid, damagedObj.transform.position, Quaternion.identity);
                StartCoroutine(waiter(damagedObj.transform.position, asteroidVar, "tiny"));
                Destroy(damagedObj);
            }
            return;
        }
        if(damagedObj.CompareTag("small"))
        {
            try
            {
                damagedAsteroids[damagedObj] -= damage;
            }
            catch(KeyNotFoundException)
            {
                damagedAsteroids.Add(damagedObj, asteroidsHP[1] - damage);
            }
            if (damagedAsteroids[damagedObj] <= 0)
            {
                GameObject asteroidVar = Instantiate(shatteredAsteroid, damagedObj.transform.position, Quaternion.identity);
                StartCoroutine(waiter(damagedObj.transform.position, asteroidVar, "small"));
                Destroy(damagedObj);
            }
            return;
        }
        if(damagedObj.CompareTag("medium"))
        {
            try
            {
                damagedAsteroids[damagedObj] -= damage;
            }
            catch(KeyNotFoundException)
            {
                damagedAsteroids.Add(damagedObj, asteroidsHP[2] - damage);
            }
            if (damagedAsteroids[damagedObj] <= 0)
            {
                GameObject asteroidVar = Instantiate(shatteredAsteroid, damagedObj.transform.position, Quaternion.identity);
                StartCoroutine(waiter(damagedObj.transform.position, asteroidVar, "medium"));
                Destroy(damagedObj);
            }
            return;
        }
        if(damagedObj.CompareTag("large"))
        {
            try
            {
                damagedAsteroids[damagedObj] -= damage;
            }
            catch(KeyNotFoundException)
            {
                damagedAsteroids.Add(damagedObj, asteroidsHP[3] - damage);
            }
            if (damagedAsteroids[damagedObj] <= 0)
            {
                GameObject asteroidVar = Instantiate(shatteredAsteroid, damagedObj.transform.position, Quaternion.identity);
                StartCoroutine(waiter(damagedObj.transform.position, asteroidVar, "large"));
                Destroy(damagedObj);
            }
            return;
        }
        if(damagedObj.CompareTag("gigant"))
        {
            try
            {
                damagedAsteroids[damagedObj] -= damage;
            }
            catch(KeyNotFoundException)
            {
                damagedAsteroids.Add(damagedObj, asteroidsHP[4] - damage);
            }
            if (damagedAsteroids[damagedObj] <= 0)
            {
                GameObject asteroidVar =  Instantiate(shatteredAsteroid, damagedObj.transform.position, Quaternion.identity);
                StartCoroutine(waiter(damagedObj.transform.position, asteroidVar, "gigant"));
                Destroy(damagedObj);
            }
            return;
        }

        void randomizeCrystals(float minValue, float maxValue)
        {
            randomCrystalScale = Random.Range(minValue, maxValue);
            randomCrystalValue = Random.Range(Mathf.RoundToInt(Mathf.Pow(minValue, 2) * 5), Mathf.RoundToInt(Mathf.Pow(maxValue, 2) * 20));
        }

        void instantiateCrystals(Vector3 Pos, string asteroidTag)
        {
            GameObject newCrystal = Instantiate(crystal, Pos, Quaternion.identity);
            newCrystal.transform.localScale = crystal.transform.localScale * randomCrystalScale;
            newCrystal.GetComponent<Dissappear>().value = randomCrystalValue;
        }

        IEnumerator waiter(Vector3 asteroidPos, GameObject obj, string tag)
        {
            switch(tag)
            {
                case "tiny":
                    randomizeCrystals(0.1f, 0.3f);
                    break;
                
                case "small":
                    randomizeCrystals(0.3f, 0.5f);
                    break;

                case "medium":
                    randomizeCrystals(0.5f, 0.8f);
                    break;

                case "large":
                    randomizeCrystals(0.8f, 1.2f);
                    break;

                case "gigant":
                    randomizeCrystals(1.2f, 1.5f);
                    break;

                default:
                    Debug.LogError("undefined object destroyed");
                    break;
            }
            instantiateCrystals(asteroidPos, tag);

            yield return new WaitForSeconds(1);
            Destroy(obj);
            yield break;
        }
    }
}
