using System;
using System.Collections;
using System.Collections.Generic;
using UniStorm;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class InventoryController : MonoBehaviour {
        public GameObject cloudButton;
        public GameObject lighteningButton;
        public GameObject waterButton;
        public GameObject rainButton;
            
        public GameObject craftButton;
        
        private Dictionary<GameObject, bool> weatherSelected = new ();
        private Dictionary<GameObject, Image> buttonSelectedImages = new();

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

            rainButton.GetComponent<Button>().onClick.AddListener(OnCraftButtonClicked);
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
            // Implement crafting logic here
            Debug.Log("Craft button clicked. Implement crafting logic here.");
            // Example: Check if the player has enough resources to craft an item
            // If so, create the item and update the inventory UI
        }
    }
}