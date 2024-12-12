using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BonusUtils
{

    public enum BonusType { Add, Multiply, Substraction, Division }

    //chance                    12-11-10-9-8-7-6-5-4-3-2-1.5-1.2-1-0.9-0.8-0.7-0.6-0.5-0.4
    static int[] addValues = { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100 };
    static int[] multiplyValues = { 2, 3, 4 };

    public static Bonus GetRandomBonus()
    {
        BonusType randomBonusType = GetRandomBonusType();
        int value = 0;

        switch (randomBonusType)
        {
            case BonusType.Add:
                float myAddValue = Random.value;
                if (myAddValue <= 1f && myAddValue >= .25f)
                    value = addValues[Random.Range(0, 6)];
                else if (myAddValue < .25f && myAddValue >= .1f)
                    value = addValues[Random.Range(6, 10)];
                else if (myAddValue < .1 && myAddValue >= .07f)
                    value = addValues[Random.Range(10, 14)];
                else if (myAddValue < .07f && myAddValue >= 0f)
                    value = addValues[Random.Range(14, addValues.Length)];
                break;

            case BonusType.Multiply:
                float myMultiplyValue = Random.value;
                if (myMultiplyValue <= 1f && myMultiplyValue >= .07f)
                    value = multiplyValues[0];
                else if (myMultiplyValue < .07f && myMultiplyValue >= .03f)
                    value = multiplyValues[1];
                else if (myMultiplyValue < .03f)
                    value = multiplyValues[2];
                break;
            case BonusType.Substraction:
                float mySubsValue = Random.value;
                value = addValues[Random.Range(0, addValues.Length)];
                break;
            case BonusType.Division:
                float myDivisionValue = Random.value;
                if (myDivisionValue <= 1f && myDivisionValue >= .06f)
                    value = multiplyValues[0];
                else if (myDivisionValue < .06f && myDivisionValue >= .02f)
                    value = multiplyValues[1];
                else if (myDivisionValue < .02f)
                    value = multiplyValues[2];
                break;
        }

        return new Bonus(randomBonusType, value);
    }

    public static int GetRunnersAmountToAdd(int currentRunnersAmount, Bonus bonus)
    {
        switch (bonus.GetBonusType())
        {
            case BonusType.Add:
                return bonus.GetValue();
            case BonusType.Multiply:
                return (currentRunnersAmount * bonus.GetValue()) - currentRunnersAmount;
            case BonusType.Division:
                return currentRunnersAmount - (currentRunnersAmount / bonus.GetValue());
            case BonusType.Substraction:
                return bonus.GetValue();

        }

        return 0;
    }

    public static string GetBonusString(Bonus bonus)
    {
        string bonusString = null;

        switch (bonus.GetBonusType())
        {
            case BonusType.Add:
                bonusString += "+";
                break;

            case BonusType.Multiply:
                bonusString += "x";
                break;
            case BonusType.Division:
                bonusString += "÷";
                break;

            case BonusType.Substraction:
                bonusString += "-";
                break;
        }

        bonusString += bonus.GetValue();

        return bonusString;
    }
    private static BonusType GetRandomBonusType()
    {
        BonusType[] bonusTypes = (BonusType[])System.Enum.GetValues(typeof(BonusType));
        float randomValue = Random.value;

        if (randomValue >= 0 && randomValue < .5f)
            return bonusTypes[0];
        else if (randomValue >= .5f && randomValue < .75f)
            return bonusTypes[1];
        else if (randomValue >= .75f && randomValue < .9f)
            return bonusTypes[2];
        else
            return bonusTypes[3];

    }
}


[System.Serializable]
public struct Bonus
{
    [SerializeField] private BonusUtils.BonusType bonusType;
    public int value;

    public Bonus(BonusUtils.BonusType bonusType, int value)
    {
        this.bonusType = bonusType;
        this.value = value;
    }

    public BonusUtils.BonusType GetBonusType()
    {
        return bonusType;
    }

    public int GetValue()
    {
        return value;
    }
}