using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckPlayer : MonoBehaviour
{
    [SerializeField] Player player;

    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;

    private void Awake()
    {
        //button1.onClick.AddListener(() =>player.Initialize(1));
        //button2.onClick.AddListener(() =>player.Initialize(2));
        //button3.onClick.AddListener(() =>player.Initialize(3));
    }
}
