using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Task7_BruteForce : MonoBehaviour
{
    //쉬운 버전

    int maxcost = 15;
    int maxdamage = -1;
    int idamage = 0;
    int icost = 0;

    string hi = " ";
    string he = " ";
    string qu = " ";
    string eu = " ";

    int[] damage = { 6, 8, 16, 24 };
    float damageRate = 0.5f;
    int[] sequence = { 0, 0, 0, 0, 1, 2 };  //0은 없음, 1은 이후, 2는 이전
    int[] cost = { 2, 3, 5, 7, 6, 5 };
    int[] cardcount = { 4, 3, 2, 2, 2, 2 };

    void Start()
    {
        for (int q = 0; q <= cardcount[0]; q++)
        {
            for (int w = 0; w <= cardcount[1]; w++)
            {
                for (int e = 0; e <= cardcount[2]; e++)
                {
                    for (int r = 0; r <= cardcount[3]; r++)
                    {
                        icost = q * cost[0] + w * cost[1] + e * cost[2] + r * cost[3];
                        if (icost <= maxcost)
                        {
                            idamage = q * damage[0] + w * damage[1] + e * damage[2] + r * damage[3];
                            if (idamage > maxdamage)
                            {
                                maxdamage = idamage;
                                hi = $"사용 할 카드 목록 : 퀵샷{q}장, 해비 샷{w}장, 멀티샷{e}장, 트리플 샷{e}장";
                                he = $"소모한 코스트 : {icost}";
                                qu = $"소모한 카드 장 수 : {q + w + e + r}장";
                                eu = $"총 데미지 : {maxdamage}";
                            }
                        }
                    }
                }
            }
        }
        Debug.Log(hi);
        Debug.Log(eu);
        Debug.Log(qu);
        Debug.Log(he);
    }
}