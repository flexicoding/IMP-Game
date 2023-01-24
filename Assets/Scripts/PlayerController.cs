using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public int damage = 5;
    public float FlySpeed = 5;
    public float YawAmount = 120;
    public int pitchAmmount;
    public float rollAmmount;
    public float shootingDelay;
    public TrailRenderer bulletTrail;
    public GameObject impact;
    public GameObject explosion;
    public int maxEnergy;
    public float energyRegenDelay = 0.1f;
    public int energyPerShot = 10;
    public int maxCrystals = 100;
    public int maxHP = 100;
    public float HPRegenDelay = 0.1f;
    public float dammageMultiplyer = 1;
    public GameObject Canvas;
    public string animName;
    public List<GameObject> barrels;

    public GameObject AI;

    private float Yaw;
    private Rigidbody rb;
    private bool canMove = true;
    private List<Transform> laserstarts = new List<Transform>();
    private float shootTimer;
    private Slider energySlider;
    private TextMeshProUGUI energyText;
    private int energy;
    private bool courotineRunning = false;
    private bool courotineRunning2 = false;
    private bool courotineRunning3 = false;
    [HideInInspector]
    public int HP;
    private Slider HPSlider;
    private TextMeshProUGUI HPText;
    private int crystals;
    private Slider crystalsSlider;
    private TextMeshProUGUI crystalsText;
    [SerializeField]
    private floatSO score;


    private void Start()
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.tag == "laserStart")
                laserstarts.Add(child.transform);
        }
        rb = GetComponent<Rigidbody>();
        energy = maxEnergy;
        energySlider = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<Slider>();
        energyText = GameObject.Find("Canvas").gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        energySlider.maxValue = maxEnergy;

        HPSlider = GameObject.Find("Canvas").transform.GetChild(1).GetComponent<Slider>();
        HPSlider.maxValue = maxHP;
        HPText = GameObject.Find("Canvas").gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        HP = maxHP;

        crystalsSlider = GameObject.Find("Canvas").transform.GetChild(2).GetComponent<Slider>();
        crystalsText = GameObject.Find("Canvas").gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        crystalsSlider.maxValue = maxCrystals;
        crystals = 0;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    // Update is called once per frame
    void Update()
    {
        #region movement
        // move forward
        if (Input.GetMouseButton(1) && canMove) rb.AddForce(transform.forward  * FlySpeed);//transform.position += transform.forward * FlySpeed * Time.deltaTime;

        // inputs
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // yaw, pitch, roll
        Yaw += horizontalInput * YawAmount * Time.deltaTime;
        float pitch = Mathf.Lerp(0, pitchAmmount, Mathf.Abs(verticalInput)) * Mathf.Sign(verticalInput);
        float roll = Mathf.Lerp(0, rollAmmount, Mathf.Abs(horizontalInput)) * -Mathf.Sign(horizontalInput);

        // apply rotation
        transform.localRotation = Quaternion.Euler(Vector3.up * Yaw + Vector3.right * pitch + Vector3.forward * roll);
        #endregion

        #region shooting
        if(Input.GetMouseButton(0))
        {
            if(shootTimer + shootingDelay < Time.time && energy >= energyPerShot * laserstarts.Count)
            {
                foreach(GameObject barrels in barrels)
                {
                    barrels.gameObject.GetComponent<Animator>().Play(animName);
                    print("anim");
                }
                foreach (Transform start in laserstarts)
                {
                    if (Physics.Raycast(start.position, start.forward, out RaycastHit hit, 250))
                    {
                        TrailRenderer trail = Instantiate(bulletTrail, start.position, Quaternion.identity);

                        StartCoroutine(spawnTrail(trail, hit.point, true));

                        gameObject.GetComponent<DammageManager>().dammaged(hit.collider.gameObject, damage, hit.collider.gameObject.tag);

                        shootTimer = Time.time;
                        energy -= energyPerShot;
                    }
                    else
                    {
                        TrailRenderer trail = Instantiate(bulletTrail, start.position, Quaternion.identity);
                        StartCoroutine(spawnTrail(trail, new Ray(start.position, start.forward).GetPoint(251), false));

                        shootTimer = Time.time;
                        energy -= energyPerShot;
                    }
                }
            }
        }
        #endregion

        #region stats
        if (energy < maxEnergy && !courotineRunning)
        {
            courotineRunning = true;
            StartCoroutine(addEnergy());
        }
        energySlider.value = energy;
        energyText.text = energy.ToString();

        if(HP < maxHP && !courotineRunning2)
        {
            courotineRunning2 = true;
            StartCoroutine(addHP());
        }
        HPSlider.value = HP;
        HPText.text = HP.ToString();

        crystalsSlider.value = crystals;
        crystalsText.text = crystals.ToString();
        #endregion

        #region gameOver
        if(HP <= 0)
        {
            Camera.main.transform.parent = null;
            Instantiate(explosion, Camera.main.transform.position, Quaternion.identity);
            gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            for(int i = 0; i < 3; i++) Canvas.transform.GetChild(i).gameObject.SetActive(false);
            Canvas.transform.GetChild(3).gameObject.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Canvas.transform.GetChild(4).gameObject.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if(Input.GetKey(KeyCode.LeftShift) && !courotineRunning3)
        {
            StartCoroutine(hyperjump());
        }

        #endregion
    }
    private void OnCollisionEnter(Collision collision)
    {
        float magnitude = rb.velocity.magnitude;
        canMove = false;
        StartCoroutine(waiter(Mathf.Round(magnitude * 1000) * 0.001f));
        gameObject.GetComponent<DammageManager>().dammaged(collision.gameObject, Mathf.RoundToInt(magnitude * 2), collision.gameObject.tag);

        HP -= Mathf.RoundToInt(magnitude * dammageMultiplyer);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("crystal"))
        {
            int value = other.gameObject.GetComponent<Dissappear>().value;
            Destroy(other.gameObject);
            if (crystals + value < maxCrystals) crystals += value;
            else crystals = maxCrystals;

        }
    }

    #region corotines
    IEnumerator waiter(float speed)
    {
        yield return new WaitForSeconds(0.05f * speed);
        canMove = true;
    }
    IEnumerator spawnTrail(TrailRenderer Trail, Vector3 HitPoint, bool hasHit)
    {
        float time = 0;
        Vector3 startPosition = Trail.transform.position;

        while (time < 1)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, HitPoint, time);
            time += Time.deltaTime / Trail.time;

            yield return null;
        }
        Trail.transform.position = HitPoint;
        if(hasHit) Instantiate(impact, HitPoint, Quaternion.identity);

        if (!Physics.Raycast(AI.GetComponent<Transform>().position, Camera.main.transform.forward)) AI.GetComponent<NewAiController>().health -= damage;

        Destroy(Trail.gameObject, Trail.time);
    }
    IEnumerator addEnergy()
    {
        yield return new WaitForSeconds(energyRegenDelay);
        energy++;
        courotineRunning = false;
        yield break;
    }
    IEnumerator addHP()
    {
        yield return new WaitForSeconds(HPRegenDelay);
        HP++;
        courotineRunning2 = false;
        yield break;
    }

    IEnumerator hyperjump()
    {
        courotineRunning3 = true;
        canMove = false;
        for (float i = 0; i <= 3; i += Time.deltaTime)
        {
            float normalizedTime = i / 3;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                Canvas.transform.GetChild(5).gameObject.SetActive(true);
                Canvas.transform.GetChild(5).gameObject.GetComponent<Image>().color = Color.Lerp(new Color(1,1,1,0), new Color(1, 1, 1, 1), normalizedTime);

                yield return null;
            }
            else
            {
                courotineRunning3 = false;
                canMove = true;
                Canvas.transform.GetChild(5).gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                Canvas.transform.GetChild(5).gameObject.SetActive(false);
                yield break;
            }
        }
        canMove = true;
        Time.timeScale = 0;
        Canvas.transform.GetChild(5).gameObject.SetActive(false);
        Canvas.transform.GetChild(6).gameObject.SetActive(true);
        score.Value += crystals;
        Canvas.transform.GetChild(6).transform.GetChild(1).transform.GetComponent<TextMeshProUGUI>().text += crystals;
        Canvas.transform.GetChild(6).transform.GetChild(2).transform.GetComponent<TextMeshProUGUI>().text += score.Value;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        yield break;
    }
    #endregion

    #region buttons
    public void TryAgain()
    {
        SceneManager.LoadScene("Game");
        score.Value += crystals;
        Time.timeScale = 1.0f;
    }
    public void Menu()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1.0f;
    }
    public void Resume()
    {
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Canvas.transform.GetChild(4).gameObject.SetActive(false);
    }
    public void Quit()
    {
        Application.Quit();
        print("quit");
    }
    #endregion
}
