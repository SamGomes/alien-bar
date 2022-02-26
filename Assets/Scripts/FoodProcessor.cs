using System.Collections.Generic;
using IngredientStuff;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FoodProcessorStuff
{
    
    public class ObjectEvents : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
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
            _gameObject.AddComponent<ObjectEvents>();
            
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
}