using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using Object = UnityEngine.Object;

public class IngredientObjectEvents : MonoBehaviour, IPointerClickHandler
{
    public Ingredient logic;
    public bool isBeingHeld;
    public Camera cam;

    public void Start()
    {
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
                transform.position = new Vector3(hit.point.x,
                    10,
                    hit.point.z);
            }
        }
    }
}



public enum IngredientAttr
{
    CUP,
    PLATE,
    TOOL,
    
    WHOLE,
    CUT,
    COLD,
    HOT,
    
    FRUIT,
    ORANGE,
    LEMON,
    APPLE,
    
    OIL,
    DRINK
}

public class Ingredient
{
    private bool _isUtensil;
    private List<GameObject> _stateObjects;
    private GameObject _gameObject;


    private List<IngredientAttr> _attributes;
    private int _timeToProcess;
    
    private int _currIngState;

    private Camera _cam;
    
    // public List<GameObject> StateObjects => _stateObjects;
    public List<GameObject> GETStateObjects()
    {
        return _stateObjects;
    }

    public List<IngredientAttr> Attributes
    {
        get => _attributes;
        set => _attributes = value;
    }

    public GameObject GameObject
    {
        get => _gameObject;
        set => _gameObject = value;
    }

    public int TimeToProcess
    {
        get => _timeToProcess;
        set => _timeToProcess = value;
    }
   
    public Ingredient(bool isTemplate, bool isUtensil, 
        Camera cam, Vector3 spawnPos, 
        GameObject table, List<GameObject> stateObjects, 
        List<IngredientAttr> attributes, int timeToProcess)
    {
        _isUtensil = isUtensil;
        
        _currIngState = 0;
        
        _stateObjects = stateObjects;
        _cam = cam;
        
        _attributes = attributes;
        _timeToProcess = timeToProcess;
        
        //only instantiate if it is not a template
        if (!isTemplate)
        {
            Debug.Log(spawnPos);
            _gameObject = Object.Instantiate(_stateObjects[0], spawnPos, Quaternion.identity);
            var events = _gameObject.AddComponent<IngredientObjectEvents>();
            events.cam = _cam;
            events.logic = this;
        }
        else
        {
            _gameObject = _stateObjects[0];
        }

    }
        
    public IEnumerator Process(int machineDelay, List<IngredientAttr> inputtedAttr, List<IngredientAttr> outputtedAttr)
    {
        yield return new WaitForSeconds(_timeToProcess + machineDelay);
        _attributes = _attributes.Except(inputtedAttr).ToList();
        _attributes.AddRange(outputtedAttr);
        
        Object.Destroy(_gameObject);
        _gameObject = Object.Instantiate(_stateObjects[++_currIngState], 
            _gameObject.transform.position, 
            Quaternion.identity);
        var events = _gameObject.AddComponent<IngredientObjectEvents>();
        events.cam = _cam;
        events.logic = this;
    }

    public List<IngredientAttr> GETAttributes()
    {
        return _attributes;
    }
    
    public bool IsUtensil()
    {
        return _isUtensil;
    }
}