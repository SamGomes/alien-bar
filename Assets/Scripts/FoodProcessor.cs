using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FoodProcessorObjectEvents : MonoBehaviour, IPointerClickHandler
{
    public FoodProcessor logic;
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //Use this to tell when the user right-clicks on the Button
        if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            logic.TurnOn();
        }

        //Use this to tell when the user left-clicks on the Button
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            logic.TurnOff();
        }
    }
}
    
enum ProcessUnitState{
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
    private ProcessUnitState _isOn;

    private int _processingDelay;

    public FoodProcessor(GameObject gameObject, 
        List<IngredientAttr> acceptedAttrs, 
        List<IngredientAttr> inputAttrs, 
        List<IngredientAttr> outputAttrs, 
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
    }

    public void SETIngredientInProcess(Ingredient ingredientInProcess)
    {
        _ingredientInProcess = ingredientInProcess;
    }
        
    public void TurnOn()
    {
        _isOn = ProcessUnitState.ON;
        if (_ingredientInProcess == null)
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
}