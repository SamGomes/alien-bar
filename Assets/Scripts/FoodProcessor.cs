using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public class FoodProcessorObjectEvents : MonoBehaviour, IPointerClickHandler
{
    public FoodProcessor logic;
    private bool _isOn;
    private Vector3 baseScale;

    public List<IngredientAttr> _addedUtensilAttrs;
    
    public bool isBeingHeld;
    
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
                transform.localScale = baseScale * 1.2f;
                StartCoroutine(logic.TurnOn());
            }else{
                transform.localScale = baseScale;
                StartCoroutine(logic.TurnOff());
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
            if (otherIng.Attributes.Contains(IngredientAttr.UTENSIL))
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
    }
}
    
public enum ProcessUnitState{
    OFF = 0,
    ON = 1
}
public class FoodProcessor
{
    public GameObject GameObject { get; set; }

    public List<IngredientAttr> AcceptedAttrs { get; set; }

    public List<IngredientAttr> InputAttrs { get; set; }

    public List<IngredientAttr> OutputAttrs { get; set; }

    public Ingredient IngredientInProcess { get; set; }

    public List<IngredientAttr> RequiredUtensilAttrs { get; set; }

    public List<IngredientAttr> AddedUtensilAttrs { get; set; }

    public ProcessUnitState IsOn { get; set; }

    public int ProcessingDelay { get; set; }

    private FoodProcessorObjectEvents _foodProcEvents;

    public FoodProcessor(
        GameObject gameObject, 
        List<IngredientAttr> acceptedAttrs, 
        List<IngredientAttr> inputAttrs, 
        List<IngredientAttr> outputAttrs, 
        List<IngredientAttr> requiredUtensilAttrs,
        int processingDelay)
    {
        GameObject = gameObject;
        _foodProcEvents = GameObject.AddComponent<FoodProcessorObjectEvents>();
        _foodProcEvents.logic = this;
            
        ProcessingDelay = processingDelay;
        AcceptedAttrs = acceptedAttrs;
        InputAttrs = inputAttrs;
        OutputAttrs = outputAttrs;
            
        IngredientInProcess = null;
        
        AddedUtensilAttrs = new List<IngredientAttr>();
        RequiredUtensilAttrs = requiredUtensilAttrs;
    }
    
    
    public void ADDUtensil(Ingredient utensilToAdd)
    {
        List<IngredientAttr> utensilAttrsToAdd = 
            utensilToAdd.Attributes.Except(AddedUtensilAttrs).ToList();
        AddedUtensilAttrs.AddRange(utensilAttrsToAdd);
        Object.Destroy(utensilToAdd.GameObject);
    }

    public bool HasUtensils()
    {
        bool hasUtensils = true;
        if (RequiredUtensilAttrs.Count > 0)
        {
            foreach (var utensil in RequiredUtensilAttrs)
            {
                if (!AddedUtensilAttrs.Contains(utensil))
                {
                    hasUtensils = false;
                    break;
                }
            }
        }

        return hasUtensils;
    }
    
    
    public IEnumerator TurnOn()
    {
        IsOn = ProcessUnitState.ON;

        if (IngredientInProcess != null && HasUtensils())
        {

            bool isCurrIngrAccepted = true;
            foreach (var attr in AcceptedAttrs)
            {
                if (!IngredientInProcess.Attributes.Contains(attr))
                {
                    isCurrIngrAccepted = false;
                    break;
                }
            }

            if (isCurrIngrAccepted)
            {
                yield return (GameObject.GetComponent<MonoBehaviour>()
                    .StartCoroutine(
                        IngredientInProcess.Process(ProcessingDelay,
                            InputAttrs, OutputAttrs)
                    ));

                _foodProcEvents.GetComponent<AudioSource>().Play();
            }
            yield return null;
        }
        yield return null;
    }

    public IEnumerator TurnOff()
    {
        IsOn = ProcessUnitState.OFF;
        yield return null;
    }

    public void ClearAddedUtensilAttributes()
    {
        AddedUtensilAttrs.Clear();
    }
}