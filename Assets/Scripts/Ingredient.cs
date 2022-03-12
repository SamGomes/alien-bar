using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

public class IngredientObjectEvents : MonoBehaviour, IPointerClickHandler
{
    public Ingredient logic;
    public bool isBeingHeld;
    public Camera cam;
    private LineRenderer _lineRenderer;

    public void Start()
    {
//        GameObject newLine = new GameObject("Line");
//        _lineRenderer = newLine.AddComponent<LineRenderer>();
        isBeingHeld = true;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //Use this to tell when the user right-clicks on the Button
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            isBeingHeld = !isBeingHeld;
        }
    }

    public void FixedUpdate()
    {
        
        if (isBeingHeld)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
    
            if (Physics.Raycast(ray, out hit))
            {
                transform.position = new Vector3(hit.point.x, 15, hit.point.z);
                //
                // _lineRenderer.startColor = Color.red;
                // _lineRenderer.endColor = Color.red;
                //
                // // set width of the renderer
                // _lineRenderer.startWidth = 0.3f;
                // _lineRenderer.endWidth = 0.3f;
                //
                // // set the position
                // _lineRenderer.SetPosition(0, transform.position);
                // _lineRenderer.SetPosition(1, new Vector3(hit.point.x, 10, hit.point.z));
            }
            
        }
    }
}


[JsonConverter(typeof(StringEnumConverter))]
public enum IngredientAttr
{
    UTENSIL,
    CUP,
    PLATE,
    TOOL,
    
    WHOLE,
    CUT,
    COLD,
    HOT,
    
    FRUIT,
    ORGEINE,
    LIEM,
    APPIA,
    
    OIL,
    FREET,
    STRALECK,
    NUMMABINE,
    
    DESSERT,
    COLEFF,
    SMALL,
    MEDIUM,
    FULL,
    
    DRUPPI_CAKE,
    LURI_CAKE,
    MARR_CAKE,
    
    KRUMSEC_MEET,
    TRYUU_MEET,
    FULADEC_EYE,
    RYNUMIDEC_LEG,
    
    
    DRINK
}

public class Ingredient
{
    public Camera Cam { get; set; }
    public List<string> StateObjectPaths { get; set; }
    public List<IngredientAttr> Attributes { get; set; }
    public GameObject GameObject { get; set; }
    public int TimeToProcess { get; set; }
    public int CurrIngState { get; set; }
    
    public Ingredient(bool isTemplate, 
        Camera cam, Vector3 spawnPos, 
        List<string> stateObjectPaths, 
        List<IngredientAttr> attributes, int timeToProcess)
    {
        CurrIngState = 0;
        
        StateObjectPaths = stateObjectPaths;
        Cam = cam;
        
        Attributes = attributes;
        TimeToProcess = timeToProcess;
        
        //only instantiate if it is not a template
        if (!isTemplate)
        {
            Debug.Log(spawnPos);
            GameObject = Object.Instantiate(
                Resources.Load<GameObject>(StateObjectPaths[0]), 
                spawnPos, Quaternion.identity);
            var events = GameObject.AddComponent<IngredientObjectEvents>();
            events.cam = Cam;
            events.logic = this;
        }
        else
        {
            GameObject = Resources.Load<GameObject>(StateObjectPaths[0]);
        }

    }
        
    public IEnumerator Process(int machineDelay, List<IngredientAttr> inputtedAttr, List<IngredientAttr> outputtedAttr)
    {
        yield return new WaitForSeconds(TimeToProcess + machineDelay);
        Attributes = new List<IngredientAttr>(Attributes.Except(inputtedAttr).ToList());
        Attributes.AddRange(outputtedAttr);
        
        Object.Destroy(GameObject);
        GameObject = Object.Instantiate(
            (GameObject) Resources.Load(StateObjectPaths[++CurrIngState]), 
            GameObject.transform.position, 
            Quaternion.identity);
        var events = GameObject.AddComponent<IngredientObjectEvents>();
        events.cam = Cam;
        events.logic = this;
    }

}