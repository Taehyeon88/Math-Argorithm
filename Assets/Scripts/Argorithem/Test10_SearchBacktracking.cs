using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test10_SearchBacktracking : MonoBehaviour
{
    int[] cards = { 2, 3, 5, 7, 9 };
    int limit = 15;

    void Start()
    {
        Search(0, new List<int>(), 0);
    }

    void Search(int i, List<int> list, int sum)
    {
        if (sum > limit) return;
        if (i == cards.Length)
        {
            Debug.Log($"{string.Join(",", list)} = {sum}");
            return;
        }
        list.Add(cards[i]);
        Search(i + 1, list, sum + cards[i]); //card[i]를 선택하는 경우

        list.RemoveAt(list.Count - 1);
        Search(i + 1, list, sum);            //card[i]를 선택하지 않은 경우
    }
}
