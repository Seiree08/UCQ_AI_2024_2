using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menus : MonoBehaviour
{
    public GameObject targetObject;
    public Button toggleButton;

    void Start()
    {
        toggleButton.onClick.AddListener(ToggleObject);
    }

    void ToggleObject()
    {
        targetObject.SetActive(!targetObject.activeSelf);
    }
}
