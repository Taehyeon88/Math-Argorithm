using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Task14_EnforceWeapon : MonoBehaviour
{

    [SerializeField] Text enforceInfoUI;
    [SerializeField] Text ExpInfo;
    [SerializeField] Slider ExpSlider;
    [SerializeField] Button EnforceButton;
    [SerializeField] Text InfoTextUI;

    [Header("탐색 버튼들")]
    [SerializeField] Button BruteForceButton;
    [SerializeField] Button ExpEffButton;
    [SerializeField] Button GoldEffButton;
    [SerializeField] Button ExpBigButton;

    //인덱스 --> 0: 소, 1: 중, 2: 대, 3: 특대
    private int[] enforceStoneExp = { 3, 5, 12, 20 };     //강화석 경험치
    private int[] enforceStonePrice = { 8, 12, 30, 45 };  //강화석 가격

    private int weaponGrade = 0;   //무기 등급
    private int neededExp = 8;     //필요 경험치
    void OnEnable()
    {
        EnforceButton.onClick.AddListener(EnforceWeapon);
        BruteForceButton.onClick.AddListener(BruteForce);
        ExpEffButton.onClick.AddListener(GreedyEffectiveExp);
        GoldEffButton.onClick.AddListener(GreadyEffectiveGold);
        ExpBigButton.onClick.AddListener(GreadyBigExp);

        enforceInfoUI.text = $"{weaponGrade}  -> {weaponGrade + 1}";
        ExpInfo.text = $"필요한 경험치 : {0}/{neededExp}";
        ExpSlider.value = 0;
        ExpSlider.maxValue = neededExp;
    }

    void OnDisable()
    {
        EnforceButton.onClick.RemoveListener(EnforceWeapon);
        BruteForceButton.onClick.RemoveListener(BruteForce);
        ExpEffButton.onClick.RemoveListener(GreedyEffectiveExp);
        GoldEffButton.onClick.RemoveListener(GreadyEffectiveGold);
        ExpBigButton.onClick.RemoveListener(GreadyBigExp);
    }

    void EnforceWeapon()  //장비 강화
    {
        Debug.Log("장비 강화 성공");

        weaponGrade++;
        neededExp = Mathf.CeilToInt(Mathf.Pow(weaponGrade + 1, 2f));

        //UI 정보 갱신
        enforceInfoUI.text = $"{weaponGrade}  -> {weaponGrade + 1}";
        ExpInfo.text = $"필요한 경험치 : {0}/{neededExp}";
        ExpSlider.value = 0;
        ExpSlider.maxValue = neededExp;
    }

    void SetEnforceStones(int Exp)
    {
        ExpInfo.text = $"필요한 경험치 : {Exp}/{neededExp}";
        ExpSlider.value = Exp;
    }

    void BruteForce()
    {
        int maxCount = Mathf.CeilToInt(neededExp / enforceStoneExp[0]);

        int coast = int.MaxValue;

        for (int q = 0; q < maxCount; q++)
        {
            for (int w = 0; w < maxCount; w++)
            {
                for (int e = 0; e < maxCount; e++)
                {
                    for (int r = 0; r < maxCount; r++)
                    {
                        if (enforceStoneExp[0] * q + enforceStoneExp[1] * w + enforceStoneExp[2] * e + enforceStoneExp[3] * r >= neededExp)   //필요 경험치를 넘을 경우
                        {

                        }
                    }
                }
            }
        }
    }
    void GreedyEffectiveExp()
    {

    }
    void GreadyEffectiveGold()
    {

    }
    void GreadyBigExp()
    {

    }
}
