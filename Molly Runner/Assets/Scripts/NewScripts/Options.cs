using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Options : MonoBehaviour
{
    [Header(" Bonuses ")]
    [SerializeField] private bool randomBonuses;
    [SerializeField] private Bonus rightBonus;
    [SerializeField] private Bonus leftBonus;

    [Header(" Components ")]
    [SerializeField] private Collider[] doorsColliders;
    [SerializeField] private TextMeshPro rightDoorText;
    [SerializeField] private TextMeshPro leftDoorText;

    // Start is called before the first frame update
    void Start()
    {
        if (randomBonuses)
            SetRandomBonuses();

        ConfigureBonusTexts();

        doorsColliders = GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider collider in doorsColliders)
        {
            collider.isTrigger = true;
        }

    }

    private void SetRandomBonuses()
    {
        rightBonus = BonusUtils.GetRandomBonus();
        leftBonus = BonusUtils.GetRandomBonus();

        if (rightBonus.value == leftBonus.value)
            leftBonus = BonusUtils.GetRandomBonus();

    }

    private void ConfigureBonusTexts()
    {
        if (rightDoorText != null)
            rightDoorText.text = BonusUtils.GetBonusString(rightBonus);
        if (leftDoorText != null)
            leftDoorText.text = BonusUtils.GetBonusString(leftBonus);

    }
    public Bonus bonus;
    public int GetRunnersAmountToAdd(Collider collidedDoor, int currentRunnersAmount)
    {
        currentRunnersAmount = ((int)GameManager.instance.totalBabyList.Count);

        DisableDoors();


        if (collidedDoor.gameObject.name == "leftbox")
            bonus = leftBonus;
        else
            bonus = rightBonus;

        return BonusUtils.GetRunnersAmountToAdd(currentRunnersAmount, bonus);
    }

    private void DisableDoors()
    {
        foreach (Collider c in doorsColliders)
        {
            c.enabled = false;
        }


    }

}
