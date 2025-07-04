using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniStorm;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    [Serializable]
    public class CompositeRule {
        public string name;
        public List<GameObject> weatherButtons;
        public WeatherType weatherType;
    }
    
    public class InventoryController : MonoBehaviour {
        public GameObject cloudButton;
        public GameObject lighteningButton;
        public GameObject waterButton;
        public GameObject rainButton;
            
        public GameObject craftButton;
        
        private Dictionary<GameObject, bool> weatherSelected = new ();
        private Dictionary<GameObject, Image> buttonSelectedImages = new();
        
        // 添加合成规则映射列表，可在编辑器内编辑
        public List<CompositeRule> compositeRules = new();

        public void Awake() {
            cloudButton.GetComponent<Button>().onClick.AddListener(OnCloudButtonClicked);
            weatherSelected[cloudButton] = false;
            buttonSelectedImages[cloudButton] = cloudButton.transform.Find("Selected").GetComponent<Image>();

            lighteningButton.GetComponent<Button>().onClick.AddListener(OnLighteningButtonClicked);
            weatherSelected[lighteningButton] = false;
            buttonSelectedImages[lighteningButton] = lighteningButton.transform.Find("Selected").GetComponent<Image>();

            waterButton.GetComponent<Button>().onClick.AddListener(OnWaterButtonClicked);
            weatherSelected[waterButton] = false;
            buttonSelectedImages[waterButton] = waterButton.transform.Find("Selected").GetComponent<Image>();
            
            rainButton.GetComponent<Button>().onClick.AddListener(OnRainButtonClicked);
            weatherSelected[rainButton] = false;
            buttonSelectedImages[rainButton] = rainButton.transform.Find("Selected").GetComponent<Image>();
            
            craftButton.GetComponent<Button>().onClick.AddListener(OnCraftButtonClicked);
        }

        public void Update() {
            foreach (var button in weatherSelected) {
                var selectedImage = buttonSelectedImages[button.Key];
                selectedImage.enabled = button.Value;
            }
        }

        private void OnCloudButtonClicked() {
            weatherSelected[cloudButton] = !weatherSelected[cloudButton];
        }

        private void OnLighteningButtonClicked() {
            weatherSelected[lighteningButton] = !weatherSelected[lighteningButton];
        }

        private void OnWaterButtonClicked() {
            weatherSelected[waterButton] = !weatherSelected[waterButton];
        }

        private void OnRainButtonClicked() {
            weatherSelected[rainButton] = !weatherSelected[rainButton];
        }

        private void OnCraftButtonClicked() {
            bool ruleFound = false;
            // 检索 CompositeRules 中选中的按钮组合
            foreach (var rule in compositeRules) {
                // 如果该规则要求的所有按钮均处于选中状态，则认为符合
                bool allSelected = rule.weatherButtons.All(btn => weatherSelected.ContainsKey(btn) && weatherSelected[btn]);
                if (allSelected) {
                    ruleFound = true;
                    // 调用 UniStorm 的天气变化（立即生效），这里假设 UniStormWeatherSystem.Instance.ChangeWeather 接受 weatherType 和立即生效参数 true
                    UniStormManager.Instance.ChangeWeatherInstantly(rule.weatherType);
                }
            }
            if (!ruleFound) {
                Debug.Log("没有找到符合的天气按钮组合。");
            }
            // 清空所有按钮选中状态
            foreach (var button in weatherSelected.Keys.ToList()) {
                weatherSelected[button] = false;
            }
        }
    }
}