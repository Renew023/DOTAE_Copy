using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class NPCQuestion : MonoBehaviour
{
    private Player player;
    [field: SerializeField] public List<Button> buttonAllPanel { get; private set; } = new();
    [field: SerializeField] public Button hotelButton { get; private set; }
    [field: SerializeField] public Button farmingButton { get; private set; }
    [field: SerializeField] public Button fishingButton { get; private set; }
    [field: SerializeField] public Button miningButton { get; private set; }
    [field: SerializeField] public Button cutterButton { get; private set; }
    [field: SerializeField] public Button upgradeButton { get; private set; }
    [field: SerializeField] public Button casinoButton { get; private set; }
    [field: SerializeField] public Button enchantButton { get; private set; }
    [field: SerializeField] public Button weaponBuyButton { get; private set; }
    [field: SerializeField] public Button armorBuyButton { get; private set; }
    [field: SerializeField] public Button materialBuyButton { get; private set; }
    [field: SerializeField] public Button positionBuyButton { get; private set; }
    [field: SerializeField] public Button otherBuyButton { get; private set; }
    [field: SerializeField] public Button trainerButton { get; private set; }
    [field: SerializeField] public Button questButton { get; private set; }

    //Test
    //[SerializeField] private GameObject dialogBox;
    //[SerializeField] private TextMeshProUGUI testDialog;

    //TODO : InputField 입력으로 인사말, 욕설 저장하고 출력 가능.

    private void Awake()
    {
        buttonAllPanel = new List<Button>
        {
            hotelButton,
            farmingButton,
            fishingButton,
            miningButton,
            cutterButton,
            upgradeButton,
            casinoButton,
            enchantButton,
            weaponBuyButton,
            armorBuyButton,
            materialBuyButton,
            positionBuyButton,
            otherBuyButton,
            trainerButton,
            questButton,
        };
    }
    private void Start()
    {
        player = FindObjectOfType<Player>();

        hotelButton.onClick.AddListener(() =>
        {
            player.characterRuntimeData.health.Current = player.characterRuntimeData.health.Max;
            //여관 회복
            //foreach (var part in player.characterRuntimeData.partDictionary.Keys)
            //{
            //    player.characterRuntimeData.partDictionary[(PartType)part].hp = Mathf.Min(player.characterRuntimeData.partDictionary[(PartType)part].hp + player.characterRuntimeData.maxPartDictionary[(PartType)part].hp * 0.3f, (float)player.characterRuntimeData.maxPartDictionary[(PartType)part].hp);
            //}
        });

        farmingButton.onClick.AddListener(() =>
        {
            //농사 배우기 : 돈 주고 농사 경험치 상승(일단은) 토지 임대료내고 농사 가능.
            //TODO : unLock 농사장 문

        });

        fishingButton.onClick.AddListener(() =>
        {
            //낚시 배우기 : 돈 주고 농사 경험치 상승(일단은) 혹은 배 빌리기 //배를 빌리면 항해가 가능해지고, 운항료는 2배로 받음.(왕복비라면서..) 
            //TODO : unLock 배

        });

        miningButton.onClick.AddListener(() =>
        {
            //광질 배우기 : 돈 주고 광질 경험치 상승 혹은 광산에 입장료내고 광질 가능.
            //TODO : 광부로 일 가능.

        });

        cutterButton.onClick.AddListener(() =>
        {
            //도끼질 배우기 : 돈주고 나무 많은 수목장 입장 가능.
            //TODO : 도끼꾼으로 일 가능.
        });

        upgradeButton.onClick.AddListener(() =>
        {
            //강화 해드림 + 대장장이 능력치의 확률 보정
            UIManager.Instance.ShowPanel(UIType.EnhanceUI);
        });

        casinoButton.onClick.AddListener(() =>
        {
            //도박 오픈 + 상대 딜러에 따라 확률 보정.
            UIManager.Instance.ShowPanel(UIType.SlotMachineUI);
        });

        enchantButton.onClick.AddListener(() =>
        {
            //TODO : 인첸트 패널
        });

        weaponBuyButton.onClick.AddListener(() =>
        {
            //TODO : 샵이 없음. NPC 인벤토리랑 비교해서 털기.
            UIManager.Instance.ShowPanel(UIType.NPCInventoryUI);
        });
        
        armorBuyButton.onClick.AddListener(() =>
        {
            //TODO : 샵이 없음. NPC 인벤토리랑 비교해서 털기.
            UIManager.Instance.ShowPanel(UIType.NPCInventoryUI);
        });

        materialBuyButton.onClick.AddListener(() =>
        {
            //TODO : 샵이 없음. NPC 인벤토리랑 비교해서 털기.
            UIManager.Instance.ShowPanel(UIType.NPCInventoryUI);
        });

        positionBuyButton.onClick.AddListener(() =>
        {
            //TODO : 샵이 없음. NPC 인벤토리랑 비교해서 털기.
            UIManager.Instance.ShowPanel(UIType.NPCInventoryUI);
        });

        otherBuyButton.onClick.AddListener(() =>
        {
            //TODO : 샵이 없음. NPC 인벤토리랑 비교해서 털기.
            UIManager.Instance.ShowPanel(UIType.NPCInventoryUI);
        });

        trainerButton.onClick.AddListener(() =>
        {
            //전투 배우기 : 돈 주고 죽지 않는 허수아비 공격 가능.
            //TODO : 전투 숙련도 증가.
        });

        questButton.onClick.AddListener(() =>
        {
            //TODO : 재료 구해오기 (처치 퀘스트도 증표 가져오는 방식)
            //DialogManager.Instance.ViewDialog();
            DialogManager.Instance.DialogList();
        });

        OffButton();
    }

    public void OffButton()
    {
        foreach (Button button in buttonAllPanel)
        {
            button.gameObject.SetActive(false);
        }
        //dialogBox.SetActive(false);
    }

    IEnumerator TimeAction(Action action, float time)
    {
        yield return new WaitForSeconds(time);
        action();
    }
}