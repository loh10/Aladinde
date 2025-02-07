using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CustomizationMenu : MonoBehaviour
{
    public Image[] sprites;
    public Sprite[] unlockedSprites;
    public Image currentHat;      
    private bool[] _unlockedHats;  

    private void Start()
    {
        FirstConnexion();
    }

    /// <summary>
    /// Function for CustomizationMenu the first time it'll be used
    /// </summary>
    private void FirstConnexion()
    {
        _unlockedHats = new bool[sprites.Length];

        currentHat.sprite = unlockedSprites[0];
        for (int i = 1; i < _unlockedHats.Length; i++)
        {
            _unlockedHats[i] = false;
            _unlockedHats[0] = true;
        }
    }
    /// <summary>
    /// Change hat if unlocked
    /// </summary>
    /// <param name="index"></param>
    public void ChangeHat(int index)
    {
        if (index < 0 || index >= sprites.Length) return;

        else if (_unlockedHats[index])  
        {
            currentHat.sprite = unlockedSprites[index];
        }
        else
        {
             return;
        }
    }


    /// <summary>
    /// Function for unlocking hat in customization menu, unlock the sprite of hats in their respective places in the menu and the possibility to wear them
    /// </summary>
    /// <param name="index"></param>
    
    public void UnlockHat(int index)
    {
        if (index >= 1 && index < _unlockedHats.Length)
        {
            sprites[index].sprite = unlockedSprites[index];
            _unlockedHats[index] = true;
        }
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }
}
