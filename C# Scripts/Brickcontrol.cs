using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Brickcontrol : MonoBehaviour
{
    //生成的砖块列,共有两种，一种
    [SerializeField] GameObject[] brickPrefabs;
    int r = 0;

    //控制刺儿和砖块的生成
    public void CreatBrick()
    {
        int[] randomList = new int[] { 30, 30, 15, 10, 10 };
        int rand = Random.Range(0, randomList.Sum());
        int currProb = 0;
        for (int i = 0; i < randomList.Length; i++)
        {
            currProb += randomList[i];
            if (rand < currProb)
            {
                r = i;
                break;
            }
        }
        //x : -5.6 ~ 6.16, y : -4.67 ~ 4.33
        // 直接生成Brickcontrol的子物件
        GameObject brick = Instantiate(brickPrefabs[r], transform);
        brick.transform.position = new Vector3(Random.Range(-3.57f, 4.34f), -5.10f, 0f);
    }
}