using Photon.Pun.Demo.Procedural;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEditor.Rendering;
using UnityEngine;

public class RoomSettingData : Singleton<RoomSettingData>
{
    #region 플레이어 설정
    [field:Header("플레이어 설정")]
    
    //3. 초당 닳는 배고픔 수치
    [field: Header("배고픔 수치")]
    [field: SerializeField] public float hungryPerSeconds { get; private set; } = 1.0f;
    
    //4. 초당 닳는 목마름 수치
    [field: Header("목마름 수치")]
    [field: SerializeField] public float thirstyPerSeconds { get; private set; } = 2.0f;

    //5. 초당 회복되는 피 회복량
    [field: Header("피 회복량")]
    [field: SerializeField] public float regenerationPerSeconds { get; private set; } = 1.0f;

    //32. 플레이어 부활 가능
    [field: Header("플레이어 부활 가능 ON/OFF")]
    [field: SerializeField] public bool isPlayerRespawn { get; private set; } = false;

    //31. 플레이어의 목숨
    [field: Header("플레이어 목숨 개수")]
    [field: SerializeField] public int playerLifeCount { get; private set; } = 3;
    [NonSerialized] public int playerLifeCurCount;

    //39. 플레이어 HP 비율
    [field: Header("플레이어 HP 비율")]
    [field: SerializeField] public float playerHPWeight { get; private set; } = 1.0f;

    //40. 플레이어 ATK 비율
    [field: Header("플레이어 Damage 비율")]
    [field: SerializeField] public float playerDamageWeight { get; private set; } = 1.0f;
    
    //41. 플레이어 DEF 비율
    [field: Header("플레이어 Defence 비율")]
    [field: SerializeField] public float playerDefWeight { get; private set; } = 1.0f;

    [field: Header("플레이어 레벨 업에 필요한 경험치량")]
    [field: SerializeField] public float playerLevelUpExp { get; private set; } = 40f;

    [field: Header("레벨업당 능력치 증가율 상승치")]
    [field: SerializeField] public float playerLevelUpPerStatWeights { get; private set; } = 0.03f;

    [field: Header("올라가는 스탯 비율")]
    [field: SerializeField] public float playerLevelUpStatWeight { get; private set; } = 0.1f;


    #endregion 플레이어 설정 [완]

    #region 전투 설정
    [field:Header("전투 설정")]

    //10. NPC 들끼리의 공격 판정
    [field: Header("NPC들끼리 공격 가능 ON/OFF")]
    [field: SerializeField] public bool isBattleBetweenNPCs { get; private set; } = false;

    //11. 몬스터들 끼리의 공격 판정
    [field: Header("몬스터들끼리 공격 가능 ON/OFF")]
    [field: SerializeField] public bool isBattleBetweenEnemys { get; private set; } = false;
    
    //12. 플레이어의 NPC 공격 판정
    [field: Header("플레이어가 NPC 공격 가능 ON/OFF")]
    [field: SerializeField] public bool isNPCAttackEnable { get; private set; } = false;

    //17. NPC 자동 공격 모드
    [field: Header("NPC 몬스터 선제공격기능 ON/OFF")]
    [field: SerializeField] public bool isNPCAttackMonster { get; private set; } = false;
    
    //35. 전체적인 크리티컬 비율
    [field: Header("모든 객체에 대해서 크리티컬 비율")]
    [field: SerializeField] public float criticalBalance { get; private set; } = 1.0f;
    
    //36. 전체적인 회피 비율
    [field: Header("모든 객체에 대해서 회피 비율")]
    [field: SerializeField] public float missBalance { get; private set; } = 1.0f;
    
    //48. 기절 기능 ON/OFF
    [field: Header("기절 기능 ON/OFF")]
    [field: SerializeField] public bool isStunEnable { get; private set; } = true;
    
    //37. 전체적인 스턴 비율
    [field: Header("모든 객체에 대해서 스턴 비율 (크리티컬 기반)")]
    [field: SerializeField] public float stunBalance { get; private set; } = 1.0f;

    //37. 전체적인 스턴 시간
    [field: Header("모든 객체에 대해서 스턴 시간")]
    [field: SerializeField] public float stunTime { get; private set; } = 1.0f;

    //48. 넉백 기능 ON/OFF
    [field: Header("넉백 기능 ON/OFF")]
    [field: SerializeField] public bool isknockBackEnable { get; private set; } = true;

    //37. 전체적인 넉백 비율
    [field: Header("모든 객체에 대해서 넉백 비율 (회피 기반)")]
    [field: SerializeField] public float KnockBackBalance { get; private set; } = 1.0f;

    //37. 전체적인 넉백 거리
    [field: Header("모든 객체에 대해서 넉백 거리")]
    [field: SerializeField] public float KnockBackDistance { get; private set; } = 1.0f;
    #endregion 전투 설정

    #region 보상 설정
    [field:Header("보상 설정")]

    //8. 몬스터 아이템 드랍비율 
    [field: Header("몬스터 아이템 드랍 비율 (적용 X)")]
    [field: SerializeField] public float monsterItemDropPercent { get; private set; } = 1.0f;

    //14. 자원 채집으로 얻는 경험치 획득
    [field: Header("자원 채집으로 얻는 경험치 획득 ON/OFF")]
    [field: SerializeField] public bool isGetMaterial { get; private set; } = false;

    //15. 자원 채집으로 얻는 경험치 획득 배율
    [field: Header("자원 채집으로 얻는 경험치 획득량")]
    [field: SerializeField] public float isGetMaterialExpPercent { get; private set; } = 1.0f;

    //16. 전투로 얻는 경험치 획득 배율
    [field: Header("전투 경험치 획득 배율")]
    [field: SerializeField] public float isKillExpPercent { get; private set; } = 1.0f;
    #endregion 보상 설정


    #region 인게임 설정
    [field: Header("인게임 설정")]
    //1. 하루 시간
    [field: Header("하루 시간")]
    [field: SerializeField] public float SecondsPerDay { get; private set; } = 60f;

    //24. 초기에 스폰될 파밍 블럭의 수
    [field: Header("초기 스폰될 블럭의 수")]
    [field: SerializeField] public int farmingBlockSettingCount { get; private set; } = 100;

    //49. 하루에 생성될 파밍 블럭의 개수
    [field: Header("매일 생성될 블럭의 수")]
    [field: SerializeField] public int farmingBlockPerOneDayCount { get; private set; } = 10;
    
    //50. 하루당 생성될 비율 증가
    [field: Header("일차마다 생성될 블럭의 수 증가량")]
    [field: SerializeField] public float farmingBlockPerOneDayWeight { get; private set; } = 1.5f;
    
    //51. 최대 파밍 블럭의 개수
    [field: Header("파밍 블럭의 총 생성 개수 (렉 방지)")]
    [field: SerializeField] public float farmingBlockMaxBlock { get; private set; } = 200;
    [NonSerialized] public int blockCount;

    //52. 최대 몬스터의 수
    [field: Header("최대 생성될 몬스터의 수 (렉 방지)")]
    [field: SerializeField] public int monsterMaxCount { get; private set; } = 50;
    
    //53. 최대 npc의 수
    [field: Header("최대 생성될 NPC의 수 (렉 방지)")]
    [field: SerializeField] public int npcMaxCount { get; private set; } = 20;
    
    //54. Frame 수치 조정
    [field: Header("프레임 수치 조정 (렉 방지)")]
    [field: SerializeField] public int gameFrame { get; private set; } = 20;
    
    //55. 파밍블럭 공격 데미지
    [field: Header("파밍블럭에 가해지는 데미지 양")]
    [field: SerializeField] public int farmingBlockAttackDamage { get; private set; } = 1;
    
    //56. 파밍블럭 공격 시 나오는 아이템의 양
    [field: Header("파밍블럭 공격데미지 1당 나오는 아이템의 양")]
    [field: SerializeField] public int farmingBlockAttackCountItem { get; private set; } = 1;

    [field: Header("파밍블럭 아이템 드랍률")]
    [field: SerializeField] public float farmingBlockItemDropPercent { get; private set; } = 1.0f;

    [field: Header("배고픔 목마름 다 달면 죽기 ON/OFF")]
    [field: SerializeField] public bool hungryThirstyZeroDeath { get; private set; } = false;

    [field: Header("목마름 배고픔당 닳는 체력의 배율")]
    [field: SerializeField] public float hungryThirstyZeroHealthMinus { get; private set; } = 0.7f;


    #endregion 인게임 설정


    #region Wave 설정
    [field: Header("Wave 설정")]

    //30. 웨이브 클리어 보상이 있는지
    [field: Header("웨이브 클리어 보상 On/OFF")]
    [field: SerializeField] public bool isWaveClearReward { get; private set; } = false;

    //29. 랜덤 몬스터를 소환할 것인지
    [field: Header("랜덤 몬스터 스폰 ON/OFF")]
    [field: SerializeField] public bool isRandomMonsterSpawn { get; private set; } = false;

    //28. 웨이브 끝나야 몬스터 소환하는지
    [field: Header("웨이브 이후 몬스터 소환 On/OFF")]
    [field: SerializeField] public bool isWaveClearMonsterSpawn { get; private set; } = false;

    //2. 하루마다 강해지는 몬스터의 능력치 배율 (중복된 내용)
    //[field: SerializeField] public float monsterStatUpPerDay { get; private set; } = 1.2f;

    //6, 몬스터 랜덤 스폰 소환주기 최소
    [field: Header("몬스터 랜덤 스폰주기 최소값")]
    [field: SerializeField] public float spawnMonsterMinLoopSeconds { get; private set; } = 1.0f;
    
    //7. 몬스터 랜덤 스폰 소환주기 최대
    [field: Header("몬스터 랜덤 스폰주기 최대값")]
    [field: SerializeField] public float spawnMonsterMaxLoopSeconds { get; private set; } = 1.0f;
    
    //9. 유저를 도와주는 NPC 소환 수
    [field: Header("시작 시 생성되는 NPC의 수")]
    [field: SerializeField] public int startNpcSpawnCount { get; private set; } = 10;
    
    //20. 몬스터 최소 소환 수
    [field: Header("Wave 몬스터 소환 수 최소")]
    [field: SerializeField] public int spawnMonsterWaveMin { get; private set; } = 1;
    
    //21. 몬스터 최대 소환 수
    [field: Header("Wave 몬스터 소환 수 최대")]
    [field: SerializeField] public int spawnMonsterWaveMax { get; private set; } = 3;
    
    //22. 몬스터 웨이브 최소 주기
    [field: Header("몬스터 웨이브 최소 주기")]
    [field: SerializeField] public float monsterWaveMinSeconds { get; private set; }
    
    //23. 몬스터 웨이브 최대 주기
    [field: Header("몬스터 웨이브 최대 주기")]
    [field: SerializeField] public float monsterWaveMaxSeconds { get; private set; }

    //59.
    [field: Header("몬스터 웨이브당 스폰되는 몬스터 배율 증가")]
    [field: SerializeField] public float monsterWaveSpawnValue { get; private set; }
    //60.
    [field: Header("몬스터 웨이브당 소환 주기 빨라짐")]
    [field: SerializeField] public float monsterWaveSpawnSecondsValue { get; private set; }

    //38. 웨이브마다 생성되는 NPC의 수
    [field: Header("웨이브마다 생성되는 NPC의 수")]
    [field: SerializeField] public int waveNpcCount { get; private set; } = 1;
    
    //34. 웨이브마다 강해지는 몬스터 배율
    [field: Header("웨이브마다 강해지는 NPC/몬스터 배율")]
    [field: SerializeField] public float monsterWaveStatUpWeight { get; private set; } = 0.1f;
    
    //25. 웨이브 클리어 경험치
    [field: Header("웨이브 클리어 경험치")]
    [field: SerializeField] public int waveClearExp { get; private set; } = 3;

    //26. 웨이브 클리어 골드
    [field: Header("웨이브 클리어 골드 (적용 X)")]
    [field: SerializeField] public int waveClearGold { get; private set; } = 3;
    
    //27. 웨이브 클리어 카운트당 보상 배율
    [field: Header("웨이브당 커지는 보상 배율")]
    [field: SerializeField] public float waveClearCountWeight { get; private set; } = 1.0f;

    [field: Header("웨이브 미클리어 시 진행 시간 차이 ON/OFF")]
    [field: SerializeField] public bool waveNotClearSpawnTime { get; private set; } = true;

    //27. 웨이브 미클리어 시 진행 시간 배율
    [field: Header("웨이브 미클리어 시 진행 시간 배율")]
    [field: SerializeField] public float waveNotClearTime { get; private set; } = 0.2f;

    [field: Header("웨이브 클리어 시 최소 달성 시간 배율")]
    [field: SerializeField] public float waveClearMinTime { get; private set; } = 0.8f;

    //58. 

    #endregion Wave 설정

    #region 몬스터 및 NPC 게임설정
    [field: Header("몬스터 및 NPC 게임 설정")]

    //44. 에너미 DEF 비율
    [field: Header("에너미 NPC 이동속도 추가 비율")]
    [field: SerializeField] public float AISpeedWeight { get; private set; } = 1.0f;

    //33. NPC, Enemy 어택 사거리 배율
    [field: Header("NPC 및 에너미 공격 사거리 배율 (1 넘기지 않기 권장)")]
    [field: SerializeField] public float attackDistance { get; private set; } = 1.0f;

    //42. 에너미 HP 비율
    [field: Header("에너미 HP 비율")]
    [field: SerializeField] public float enemyHPWeight { get; private set; } = 1.0f;
    
    //43. 에너미 ATK 비율
    [field: Header("에너미 ATK 비율")]
    [field: SerializeField] public float enemyDamageWeight { get; private set; } = 1.0f;

    //44. 에너미 DEF 비율
    [field: Header("에너미 DEF 비율")]
    [field: SerializeField] public float enemyDefWeight { get; private set; } = 1.0f;
    
    //45. NPC HP 비율
    [field: Header("NPC HP 비율")]
    [field: SerializeField] public float npcHPWeight { get; private set; } = 1.0f;
    
    //46. NPC ATK 비율
    [field: Header("NPC ATK 비율")]
    [field: SerializeField] public float npcDamageWeight { get; private set; } = 1.0f;

    //47. NPC DEF 비율
    [field: Header("NPC DEF 비율")]
    [field: SerializeField] public float npcDefWeight { get; private set; } = 1.0f;
    #endregion 몬스터 및 NPC 게임 설정

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = gameFrame;
    }

    public void Init()
    {
        playerLifeCurCount = playerLifeCount;
    }
}
