using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "ScriptableObjects", menuName = "ScriptableObjects/ItemScriptable", order = 1)]
public class ItemScriptable : ScriptableObject
{
    public Sprite lockImage;
    public List<ScriptableStruct> hats;
    public List<ScriptableStruct> jackets;
    public List<ScriptableStruct> pants;

}

[System.Serializable]
public struct ScriptableStruct
{
    public Sprite cutImage;
    public Sprite woreImage;
    public int itemPrice;
    public int defenceValue;
    public int attack;
    public int itemID;
}

