using System.Collections.Generic;
using IngredientStuff;
using UnityEngine;

namespace FoodProcessorStuff
{
    enum ProcessUnitState{
        OFF = 0,
        ON = 1
    }
    public class FoodProcessor
    {
        private GameObject _gameObject;
        private List<IngredientAttr> _inputAttrs;
        private List<IngredientAttr> _outputAttrs;
        private Ingredient _ingredientInProcess;
        private ProcessUnitState _isOn;

        private int _processingDelay;

        public FoodProcessor(GameObject gameObject, 
            List<IngredientAttr> inputAttrs, 
            List<IngredientAttr> outputAttrs, 
            int processingDelay)
        {
            _gameObject = gameObject;
            _processingDelay = processingDelay;
            _inputAttrs = inputAttrs;
            _outputAttrs = outputAttrs;
            
            _ingredientInProcess = null;
        }
        public void TurnOn()
        {
            _isOn = ProcessUnitState.ON;
            if (_ingredientInProcess == null)
            {
                return;
            }
            
            bool isCurrIngrAccepted = true;
            foreach (var attr in _inputAttrs)
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
                    .StartCoroutine(_ingredientInProcess.Process(_processingDelay));
            }
        }

        public void TurnOff()
        {
            _isOn = ProcessUnitState.OFF;
        }
        

    }
}