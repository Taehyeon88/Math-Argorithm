using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class Test2_ArraySort : MonoBehaviour
{
    public void Start()
    {
        int[] data = GenerateRandomArray(10);
        //StartSelectionSort(data);
        StartQuickSort(data, 0, data.Length - 1);
        foreach (var item in data)
        {
            //Debug.Log(item);
        }
    }

    int[] GenerateRandomArray(int size)
    {
        int[] arr = new int[size];
        System.Random rand = new System.Random();
        for (int i = 0; i < size; i++)
        {
            arr[i] = rand.Next(0, 10);
        }
        return arr;
    }

    public static void StartSelectionSort(int[] arr) //선택정렬 함수
    {                                                //설명: 첫 인덱스부터 차례대로 가장 작은 수를 찾아서 앞으로 재배치
        int n = arr.Length;                          //시간 복잡도: n(n - 1)X 1/2 --> (n-1) + (n-2) .... + 2 + 1
        for (int i = 0; i < n - 1; i++)
        {
            int minIndex = i;
            for (int j = i + 1; j < n; j++)
            {
                if (arr[j] < arr[minIndex])
                {
                    minIndex = j;
                }
            }
            // swap
            int temp = arr[minIndex];
            arr[minIndex] = arr[i];
            arr[i] = temp;
        }
    }

    //버블 정렬 함수
    //설명(예시):
    //배열: [29, 10, 14, 37, 13]
    //1회전:
    // (29,10) → 교환 → [10, 29, 14, 37, 13] / (29,14) → 교환 → [10, 14, 29, 37, 13]  / (29,37) → 그대로  /  (37,13) → 교환 → [10, 14, 29, 13, 37]

    //2회전:
    // (10,14) → 그대로 / (14,29) → 그대로 /  (29,13) → 교환 → [10, 14, 13, 29, 37]

    //3회전:
    // (10,14) → 그대로 /  (14,13) → 교환 → [10, 13, 14, 29, 37]

    //4회전:
    // (10,13) → 그대로
    //최종 결과: [10, 13, 14, 29, 37]
    //시간 복잡도: n(n - 1)X 1/2 --> (n-1) + (n-2) .... + 2 + 1

    public static void StartBubbleSort(int[] arr)
    {
        int n = arr.Length;
        for (int i = 0; i < n - 1; i++)
        {
            bool swapped = false;
            for (int j = 0; j < n - i - 1; j++)
            {
                if (arr[j] > arr[j + 1])
                {
                    //swap
                    int temp = arr[j];
                    arr[j] = arr[j + 1];
                    arr[j + 1] = temp;
                    swapped = true;
                }
            }
            //이비 정렬된 경우 조기 종료
            if (!swapped) break;
        }
    }

    //뀍 정렬
    //설명:
    //분할 정복(Divide & Conquer) 방식
    //1. 피벗(Pivot) 선택
    //2. 피벗보다 작은 값은 왼쪽, 큰 값은 오른쪽으로 분할
    //3. 각 부분 배열에 대해 재귀적으로 정렬 수행

    //예시:
    //배열: [29, 10, 14, 37, 13]
    //피벗: 29  → 왼쪽: [10, 14, 13], 피벗: [29], 오른쪽: [37]
    //왼쪽[10, 14, 13] → 피벗: 10  → [ ], [10], [14, 13]
    //[14, 13] → 피벗: 14  → [13], [14], [ ]

    //→ 정렬 후 왼쪽 전체: [10, 13, 14]
    //오른쪽[37] → 이미 정렬됨

    // 최종 결과: [10, 13, 14, 29, 37]

    //시간 복잡도:n(총개수_탐색 개수) X log(n)(분할 횟수)


    public static void StartQuickSort(int[] arr, int low, int high)
    {
        if (low < high)
        {
            int pivotIndex = Partition(arr, low, high);
            //Debug.Log(pivotIndex);
            StartQuickSort(arr, low, pivotIndex - 1);
            //Debug.Log(pivotIndex);
            StartQuickSort(arr, pivotIndex + 1, high);
        }
    }

    private static int Partition(int[] arr, int low, int high)
    {
        int pivot = arr[high];
        int i = (low - 1);

        for (int j = low; j < high; j++)
        {
            if (arr[j] <= pivot)
            {
                i++;
                //swap
                int temp = arr[i];
                arr[i] = arr[j];
                arr[j] = temp;
            }
        }
        //pivot자리 교환
        int temp2 = arr[i + 1];
        arr[i + 1] = arr[high];
        arr[high] = temp2;

        return i + 1;
    }
}
