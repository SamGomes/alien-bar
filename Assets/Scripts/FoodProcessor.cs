using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using Object = UnityEngine.Object;

public class FoodProcessorObjectEvents : MonoBehaviour, IPointerClickHandler
{
    public FoodProcessor logic;
    private bool _isOn;
    private Vector3 baseScale;

    public List<IngredientAttr> _addedUtensilAttrs;

    public void Start()
    {
        baseScale = transform.localScale;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //Use this to tell when the user left-clicks on the Button
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            _isOn = !_isOn;
            
            if (_isOn)
            {
                Debug.Log("Processor Turned On!");
                logic.TurnOn();
            }else{
                Debug.Log("Processor Turned Off!");
                logic.TurnOff();
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered processor");
        transform.localScale = 1.1f * baseScale;
        var ingEvents = other.GetComponent<IngredientObjectEvents>();
        if (ingEvents != null)
        {
            Ingredient otherIng = ingEvents.logic;
            if (otherIng.IsUtensil())
            {
                logic.ADDUtensil(otherIng);
            }
            else
            {
                logic.IngredientInProcess = otherIng;
            }
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        logic.IngredientInProcess = null;
        logic.ClearAddedUtensilAttributes();
        transform.localScale = baseScale;
    }


    public void Update()
    {
        _addedUtensilAttrs = logic.AddedUtensilAttrs;
        
//        if (logic.IsOn == ProcessUnitState.ON && logic.IngredientInProcess != null)
//        {
//            logic.TurnOff();
//            transform.localScale = baseScale;
//            
//            logic.IngredientInProcess = null;
//            logic.ClearAddedUtensilAttributes();
//        }
    }
}
    
public enum ProcessUnitState{
    OFF = 0,
    ON = 1
}
public class FoodProcessor
{
    private GameObject _gameObject;
    private List<IngredientAttr> _acceptedAttrs;
    private List<IngredientAttr> _inputAttrs;
    private List<IngredientAttr> _outputAttrs;
    private Ingredient _ingredientInProcess;

    private List<IngredientAttr> _requiredUtensilAttrs;
    private List<IngredientAttr> _addedUtensilAttrs;
    
    private ProcessUnitState _isOn;

    private int _processingDelay;
    
    
    public GameObject GameObject
    {
        get => _gameObject;
        set => _gameObject = value;
    }

    public List<IngredientAttr> AcceptedAttrs
    {
        get => _acceptedAttrs;
        set => _acceptedAttrs = value;
    }

    public List<IngredientAttr> InputAttrs
    {
        get => _inputAttrs;
        set => _inputAttrs = value;
    }

    public List<IngredientAttr> OutputAttrs
    {
        get => _outputAttrs;
        set => _outputAttrs = value;
    }

    public Ingredient IngredientInProcess
    {
        get => _ingredientInProcess;
        set => _ingredientInProcess = value;
    }

    public List<IngredientAttr> RequiredUtensilAttrs
    {
        get => _requiredUtensilAttrs;
        set => _requiredUtensilAttrs = value;
    }

    public List<IngredientAttr> AddedUtensilAttrs
    {
        get => _addedUtensilAttrs;
        set => _addedUtensilAttrs = value;
    }

    public ProcessUnitState IsOn
    {
        get => _isOn;
        set => _isOn = value;
    }

    public int ProcessingDelay
    {
        get => _processingDelay;
        set => _processingDelay = value;
    }


    public FoodProcessor(GameObject gameObject, 
        List<IngredientAttr> acceptedAttrs, 
        List<IngredientAttr> inputAttrs, 
        List<IngredientAttr> outputAttrs, 
        List<IngredientAttr> requiredUtensilAttrs,
        int processingDelay)
    {
        _gameObject = gameObject;
        _gameObject.AddComponent<FoodProcessorObjectEvents>();
        _gameObject.GetComponent<FoodProcessorObjectEvents>().logic = this;
            
        _processingDelay = processingDelay;
        _acceptedAttrs = acceptedAttrs;
        _inputAttrs = inputAttrs;
        _outputAttrs = outputAttrs;
            
        _ingredientInProcess = null;
        
        _addedUtensilAttrs = new List<IngredientAttr>();
        _requiredUtensilAttrs = requiredUtensilAttrs;
    }
    
    
    public void ADDUtensil(Ingredient utensilToAdd)
    {
        List<IngredientAttr> utensilAttrsToAdd = 
            utensilToAdd.GETAttributes().Except(_addedUtensilAttrs).ToList();
        _addedUtensilAttrs.AddRange(utensilAttrsToAdd);
        Object.Destroy(utensilToAdd.GameObject);
    }
        
    public void TurnOn()
    {
        _isOn = ProcessUnitState.ON;

        bool hasUtensils = true;
        if (_requiredUtensilAttrs.Count > 0)
        {
            foreach (var utensil in _requiredUtensilAttrs)
            {
                if (!_addedUtensilAttrs.Contains(utensil))
                {
                    hasUtensils = false;
                    break;
                }
            }
        }

        if (_ingredientInProcess == null || !hasUtensils)
        {
            return;
        }
            
        bool isCurrIngrAccepted = true;
        foreach (var attr in _acceptedAttrs)
        {
            if (!_ingredientInProcess.GETAttributes().Contains(attr))
            {
                isCurrIngrAccepted = false;
                break;
            }
        }
            
        if (isCurrIngrAccepted)
        {
            _gameObject.GetComponent<MonoBehaviour>()
                .StartCoroutine(
                    _ingredientInProcess.Process(_processingDelay,
                        _inputAttrs,_outputAttrs)
                );
        }
    }

    public void TurnOff()
    {
        _isOn = ProcessUnitState.OFF;
    }

    public void ClearAddedUtensilAttributes()
    {
        _addedUtensilAttrs.Clear();
    }
}