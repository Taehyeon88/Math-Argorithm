using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.UI;


public class Task7_BruteForce : MonoBehaviour
{
    //쉬운 버전

    const int maxcost = 20;           //사용 가능한 비용
    const int cardTypeCount = 6;      //카드 종류 개수
    const int drawCardCount = 4;      //한번에 드로우하는 카드 개수

    int maxdamage = -1;
    int idamage = 0;
    float damageRate = 1;

    ShotCard[] shots = new ShotCard[cardTypeCount]    //카드 종료별 정의 : shots[카드 인덱스] == 해당 카드
    {
        new ShotCard("QuickShot", 6, 2, ShotEffectType.AddDamage),
        new ShotCard("HeavyShot", 8, 3, ShotEffectType.AddDamage),
        new ShotCard("MultyShot", 16, 5, ShotEffectType.AddDamage),
        new ShotCard("TripleShot", 24, 7, ShotEffectType.AddDamage),
        new ShotCard("PowerUp", 50, 6, ShotEffectType.PowerUp),
        new ShotCard("Duplecate", 0, 5, ShotEffectType.Duplicate)
    };

    int[] cardCounts = { 4, 3, 2, 2, 2, 2};             //각 카드 종류별 개수 (초기) (카드 개수)

    int[] currentCardCounts = new int[cardTypeCount];   //현재 덱의 카드 종류별 개수 (카드 개수)

    int[] currentCardSequence = new int[drawCardCount]; //현재 사용할 카드 순서 배열 (카드 인덱스)          

    List<int> handCards = new List<int>();             //손에 들고 있는 카드 리스트 (카드 인덱스)

    void Start()
    {
        cardCounts.CopyTo(currentCardCounts, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DrawCards();
            Attack();
            Initialize();
        }
    }

    private void Attack()          //공격 처리 함수
    {
        Debug.Log($"현재 덱에서의 카드 개수: {string.Join(", ", currentCardCounts)}");
        Debug.Log($"공격력: {maxdamage}, 사용한 카드 순서: {string.Join(", ", currentCardSequence)}");
    }

    private void Initialize()       //턴 종료시, 초기화 함수
    {
        foreach (int usedCardId in currentCardSequence)  //손의 카드리스트에서 사용한 카드들 제거
        {
            if(handCards.Contains(usedCardId))
                handCards.Remove(usedCardId);
        }
        currentCardSequence = new int[drawCardCount];    //카드 순서 배열 초기화
        maxdamage = 0;                                   //최대 데미지 초기화
    }

    private void DrawCards()
    {
        int deckCount = 0;
        int drawCount = drawCardCount - this.handCards.Count;

        foreach (var cardCount in currentCardCounts)
            deckCount += cardCount;

        if (deckCount < drawCount)                  //현재 덱의 개수가 드로우할 개수보다 작을 경우
        {
            Debug.Log("덱을 초기화 하였습니다. 덱을 섞습니다.");
            cardCounts.CopyTo(currentCardCounts, 0); //다시 현재 카드 개수 초기화
            return;
        }

        while (drawCount > 0)                    //덱에서 드로우 개수만클 카드 랜덤 드로우
        {
            int randomValue = UnityEngine.Random.Range(0, cardTypeCount);
            int temp = currentCardCounts[randomValue];
            if (temp <= 0)
            {
                continue;
            }
            else
            {
                handCards.Add(randomValue);
                currentCardCounts[randomValue]--;  //드로우한 카드를 덱에서 제거
                drawCount--;
            }
        }

        Debug.Log($"드로우후, 손의 카드: {string.Join(", ", handCards)}");

        //카드를 조합할 수 있는 모든 경우 반복 체크
        for (int q = 0; q < 2; q++)
        {
            for (int w = 0; w < 2; w++)
            {
                for (int e = 0; e < 2; e++)
                {
                    for (int r = 0; r < 2; r++)
                    {
                        int icost = q * shots[handCards[0]].cost + w * shots[handCards[1]].cost + e * shots[handCards[2]].cost + r * shots[handCards[3]].cost;
                        if (icost <= maxcost)
                        {
                            List<int> temp = new List<int>();
                            if(q == 1) temp.Add(handCards[0]);
                            if(w == 1) temp.Add(handCards[1]);
                            if(e == 1) temp.Add(handCards[2]);
                            if(r == 1) temp.Add(handCards[3]);

                            if (temp.Count > 1)
                                FindAllCase(temp, new Queue<int>());  //모든 경우의 수를 찾기 및 저장
                        }
                    }
                }
            }
        }
    }

    private void FindAllCase(List<int> remain, Queue<int> result) //현재 조합의 카드 리스트의 모든 순서배열의 경우 체크 재귀함수
    {
        int count = remain.Count;

        if (count <= 0)
        {
            //Debug.Log("카드 번호 : " + string.Join(", ", result));
            CheckDamage(new List<int>(result));
            return;
        }

        while (count > 0)
        {
            int index = remain[count - 1];

            Queue<int> resultTemp = new Queue<int>(result);
            resultTemp.Enqueue(index);

            List<int> remainTemp = new List<int>(remain);
            remainTemp.Remove(index);

            FindAllCase(remainTemp, resultTemp);
            count--;
        }
    }

    private void CheckDamage(List<int> cards)     //현재 순서의 카드 리스트의 효과 호출 및 최대 데미지 갱신 함수
    {
        for (int i = 0; i < cards.Count; i++)
            SetEffect(cards, i);

        if (idamage > maxdamage)  //현재 데미지가 최대 데미지를 넘을 경우
        {
            maxdamage = idamage;
            //Debug.Log($"최대 공격력 갱신 - 카드 순서: {string.Join(", ", cards)}");
            currentCardSequence = cards.ToArray();
        }

        damageRate = 1;
        idamage = 0;
    }

    private void SetEffect(List<int> cards, int cardIndex)   //현재 카드의 효과 적용함수
    {
        if (cardIndex <= 0) return;

        ShotCard card = shots[cards[cardIndex]];       //카드 찾기 ( 특정 경우의 수의 특정 카드의 인덱스 받기 )

        switch (card.effectType)
        {
            case ShotEffectType.PowerUp:
                damageRate += card.damage / 100f;
                break;

            case ShotEffectType.Duplicate:
                SetEffect(cards, cardIndex - 1);              //현재 경우의 수의 이전 카드의 효과 실행
                break;

            case ShotEffectType.AddDamage:
                idamage += Mathf.FloorToInt(card.damage * damageRate);
                damageRate = 1;
                break;
        }
    }
}

public class ShotCard
{
    public string name;
    public int damage;                 //경우 따라서 damageRate로도 쓰임
    public int cost;

    public ShotEffectType effectType;  //이펙트 유형

    public ShotCard(string name, int damage, int cost, ShotEffectType effectType)
    {
        this.name = name;
        this.damage = damage;
        this.cost = cost;
        this.effectType = effectType;
    }
}

public enum ShotEffectType
{
    None,
    AddDamage,
    PowerUp,
    Duplicate
}