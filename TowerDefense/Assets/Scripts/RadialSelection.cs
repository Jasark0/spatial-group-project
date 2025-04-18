using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.InputSystem;

public class RadialSelection : MonoBehaviour
{
    public OVRInput.Button spawnButton;
    [Range(2,10)]
    public int numberOfRadialPart;
    public TurretUpgradeRadialMenu turretUpgradeRadialMenu;
    public GameObject radialPartPrefab;
    public Transform radialPartCanvas;
    public float angleBetweenPart = 10;
    public Transform handTransform;

    public GameObject gameObjectName;
    public GameObject health;
    public GameObject cost;
    public GameObject level;

    public UnityEvent<int> OnPartSelected;

    public float spacing = 10f;

    [HideInInspector] public List<GameObject> spawnedParts = new List<GameObject>();
    private int currentSelectedRadialPart = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnRadialPart();
        }

        if ( OVRInput.GetDown(spawnButton) && turretUpgradeRadialMenu.ifTurretIsSelected)
        {
            SpawnRadialPart();
        }

        if ( OVRInput.Get(spawnButton))
        {
            GetSelectedRadialPart();
        }

        if ( OVRInput.GetUp(spawnButton))
        {
            HideAndTriggerSelected();
        }
        
        
        
        
        
        // SpawnRadialPart();
        // GetSelectedRadialPart();
    }

    public void HideAndTriggerSelected()
    {
        OnPartSelected.Invoke(currentSelectedRadialPart);
        radialPartCanvas.gameObject.SetActive(false);
    }

    public void GetSelectedRadialPart()
    {
        Vector3 centerToHand = handTransform.position - radialPartCanvas.position;
        Vector3 centerToHandProjected = Vector3.ProjectOnPlane(centerToHand, radialPartCanvas.forward);

        float angle = Vector3.SignedAngle(radialPartCanvas.up, centerToHandProjected, -radialPartCanvas.forward);
        
        if ( angle < 0)
        {
            angle += 360;
        }
        currentSelectedRadialPart = (int)angle * numberOfRadialPart / 360;
        bool foundSelected = false;
        for ( int i = 0; i < spawnedParts.Count; i++)
        {
            if (i == currentSelectedRadialPart)
            {
                spawnedParts[i].GetComponent<Image>().color = Color.red;
                spawnedParts[i].transform.localScale = 1.1f * Vector3.one;
                
                if ( turretUpgradeRadialMenu != null)
                {
                    // Debug.Log("yo: {" + i + "}");
                    cost.gameObject.SetActive(true);
                    level.gameObject.SetActive(true); 
                    turretUpgradeRadialMenu.UpdateRadialSelectionText(i);
                }
                foundSelected = true;

            }
            else
            {
                spawnedParts[i].GetComponent<Image>().color = Color.white;
                spawnedParts[i].transform.localScale = Vector3.one;

                if ( turretUpgradeRadialMenu != null)
                {
                    if (foundSelected != true)
                    {
                        cost.gameObject.SetActive(false);
                        level.gameObject.SetActive(false);
                    }
                        
                }
            }
        }
    }

    public void SpawnRadialPart()
    {
        radialPartCanvas.gameObject.SetActive(true);
        radialPartCanvas.position = handTransform.position;
        radialPartCanvas.rotation = handTransform.rotation;

        gameObjectName.gameObject.SetActive(true);
        health.gameObject.SetActive(true);
        // cost.gameObject.SetActive(true);
        
        // level.gameObject.SetActive(true);
         gameObjectName.transform.position = radialPartCanvas.position + new Vector3(0, .150f, 0);
        health.transform.position = radialPartCanvas.position+ new Vector3(0, .140f, 0);
        cost.transform.position = radialPartCanvas.position + new Vector3(0, .030f, 0);
        level.transform.position = radialPartCanvas.position + new Vector3(0, -.024f, 0); 
        foreach (var item in spawnedParts)
        {
            Destroy(item);
        }
        spawnedParts.Clear();

        for (int i = 0; i < numberOfRadialPart; i++)
        {
            float angle = -i * 360 / numberOfRadialPart - angleBetweenPart/2;
            Vector3 radialPartEulerAngle = new Vector3(0, 0, angle);
            // Vector3 radialPartEulerAngle = radialPartCanvas.transform.eulerAngles + new Vector3(0, 0, angle);

            GameObject spawnedRadialPart = Instantiate(radialPartPrefab, radialPartCanvas);
            spawnedRadialPart.transform.position = radialPartCanvas.position;
            spawnedRadialPart.transform.localEulerAngles = radialPartEulerAngle;

            spawnedRadialPart.GetComponent<Image>().fillAmount = 1 / (float)numberOfRadialPart - (angleBetweenPart / 360);

            // Find and fix text component if it exists
            ConfigureRadialText(spawnedRadialPart, i, angle);

            spawnedParts.Add(spawnedRadialPart);
        }

        
    }

    
    private void ConfigureRadialText(GameObject radialPart, int index, float angle)
{
    TMPro.TextMeshProUGUI textComponent = radialPart.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    
    if (textComponent != null)
    {
        textComponent.text = $"{index}";
        
        // Calculate the angular width of the visible segment
        float angularWidth = (360f / numberOfRadialPart) - angleBetweenPart;
        float localMidAngle = angularWidth / 2f; // Midpoint within the segment

        // Convert angle to radians
        float radians = localMidAngle * Mathf.Deg2Rad;

        // Calculate position relative to the radial part's center
        float radius = 70f; // Adjust this value to move text closer/further
        float x = Mathf.Sin(radians) * radius;
        float y = Mathf.Cos(radians) * radius;

        // Apply position and rotation
        RectTransform textRectTransform = textComponent.GetComponent<RectTransform>();
        textRectTransform.anchoredPosition = new Vector2(x + spacing, y + spacing);
        // textRectTransform.localRotation = Quaternion.Euler(0, 0, localMidAngle);
        textRectTransform.localRotation = Quaternion.Euler(0, 0, -radialPart.transform.localEulerAngles.z);

        textComponent.enabled = true;
    }
}

    public void SetRadialPartText(int index, string text)
    {
        if (index >= 0 && index < spawnedParts.Count)
        {
            TMPro.TextMeshProUGUI textComponent = spawnedParts[index].GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = text;
            }
        }
    }
}
