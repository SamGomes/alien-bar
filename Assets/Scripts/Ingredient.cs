using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;



public class IngredientObjectEvents : 
    MonoBehaviour, 
    IPointerDownHandler, 
    IPointerUpHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public Ingredient logic;
    public bool isBeingHeld;
    public bool isLocked;
    public Camera cam;
    private LineRenderer _lineRenderer;

    public void Start()
    {
        isBeingHeld = false;
        isLocked = false;
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        //Use this to tell when the user right-clicks on the Button
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            GameGlobals.GameManager.cursorOverlapBuffer.Add(GameGlobals.GameManager.cursorTextureDrag);
            isBeingHeld = true;
        }
    }
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            GameGlobals.GameManager.cursorOverlapBuffer.Remove(GameGlobals.GameManager.cursorTextureDrag);
            isBeingHeld = false;
        }
    }
    
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if(!isBeingHeld)
            GameGlobals.GameManager.cursorOverlapBuffer.Add(GameGlobals.GameManager.cursorTexturePicking);
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if(!isBeingHeld)
            GameGlobals.GameManager.cursorOverlapBuffer.Remove(GameGlobals.GameManager.cursorTexturePicking);
    }

    public void FixedUpdate()
    {
        if (!isLocked && isBeingHeld)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
    
            if (Physics.Raycast(ray, out hit))
            {
                transform.position = new Vector3(hit.point.x, 5, hit.point.z);
            }
            
        }
    }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum IngredientAttr
{
    UTENSIL,
    THIRPUNASOREC,
    
    LUAHH,
    HEFTT,
    
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
    WUDIF,
    KREW,
    FRUB,
    
    CAKE,
    DRUPPI_CAKE,
    LURI_CAKE,
    MARR_CAKE,
    
    KRUMSEC_MEET,
    TRYUU_MEET,
    FULADEC_EYE,
    RYNUMIDEC_LEG,
    
    
    AMERM,
    AKWA,
    ALOC
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
        events.isBeingHeld = false;
        events.logic = this;
    }
    

    public void Lock()
    {
        GameObject.GetComponent<IngredientObjectEvents>().isLocked = true;
    }

    public void Unlock()
    {
        GameObject.GetComponent<IngredientObjectEvents>().isLocked = false;
    }

}