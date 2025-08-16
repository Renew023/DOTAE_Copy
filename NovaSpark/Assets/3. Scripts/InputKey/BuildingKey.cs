using Photon.Pun.Demo.Procedural;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class BuildingKey : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _blockList = new();// TODO : 나중에 실제 아이템으로 변경
    
    [SerializeField] 
    private GameObject _selectBlock; //Prefabs
    private SpriteRenderer _selectBlockPreview;

    [SerializeField]
    private GameObject _tilePrefab;
    private Tilemap _buildTileMap;

    [SerializeField]
    private LayerMask _obstacleLayer; //추후 건물, 장애물, 장식 등을 구분할 수도 있음.

    private Collider2D _obstacle;

    private bool isBuild;

    private void Awake()
    {
        _buildTileMap = Instantiate(_tilePrefab).GetComponentInChildren<Tilemap>();
    }

    private void OnEnable()
    {
        if(_selectBlock == null)
        {
            _selectBlockPreview = new GameObject().AddComponent<SpriteRenderer>();
            SetPreview(_blockList[0]);
        }
        StartCoroutine(BlockCheck());
    }

    private void SetPreview(GameObject setBlock)
    {
        _selectBlock = setBlock;
        _selectBlockPreview.sprite = _selectBlock.GetComponent<SpriteRenderer>().sprite;
        _selectBlockPreview.sortingOrder = _selectBlock.GetComponent<SpriteRenderer>().sortingOrder;
        _selectBlock.transform.localScale = _buildTileMap.transform.localScale;
        _selectBlockPreview.transform.localScale = _buildTileMap.transform.localScale;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void OnBuildSelect1(InputAction.CallbackContext context)
    {
        SetPreview(_blockList[0]);
    }

    public void OnBuildSelect2(InputAction.CallbackContext context)
    {
        SetPreview(_blockList[1]);
    }
    
    public void OnBuildSelect3(InputAction.CallbackContext context)
    {
        SetPreview(_blockList[2]);
    }

    public void OnSetBlock(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            if (isBuild == true)
            {
                if (_obstacle != null)
                {
                    //Debug.Log("현재 위치에는 설치할 수 없습니다.");
                }
                else
                {
                    //TODO : 설치 그 위치에
                    //StartCoroutine으로 건설 시간 이후 건설.
                    //그러나, 건설 도중 방해 받으면 취소됨.
                    //일단 생성은 하되, 천천히 만들어짐. 
                    //GameObject block = Instantiate(_selectBlock, _selectBlockPreview.transform.localPosition, Quaternion.identity);
                    StartCoroutine(Building(5f));
                    //TODO : 사용 개수 감소 
                }
            }
            //if(Ray) == true
            //TODO : 블럭을 놓는다. 그 위치에 단, 물건이 있으면 눌려도 안됨   
        }
    }
    IEnumerator Building(float time)
    {
        SoundManager.Instance.PlaySFXAsync(SoundParameterData.Building_SFXParameterHash);

        GameObject block = Instantiate(_selectBlock, _selectBlockPreview.transform.localPosition, Quaternion.identity);
        float progressTime = 0.1f;
        while (time > progressTime)
        {
            block.transform.localScale = new Vector2(1f, progressTime/time);
            progressTime += Time.unscaledDeltaTime;
            yield return null;
        }
        block.transform.localScale = new Vector2(1f, 1f);
    }
    
    IEnumerator BlockCheck()
    {
        while(true)
        {
            Vector3 mouseDelta = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector3Int playerCell = _buildTileMap.WorldToCell(gameObject.transform.position);
            Vector3Int mouseCell = _buildTileMap.WorldToCell(mouseDelta);

            float distance = Vector3Int.Distance(playerCell, mouseCell);
            //Debug.Log("BuildCheck +" + distance);

            if (distance < 3f)
            {
                _selectBlockPreview.gameObject.SetActive(true);

                Vector3 pos = _buildTileMap.GetCellCenterWorld(mouseCell);
                _selectBlockPreview.transform.localPosition = pos;
                _obstacle = Physics2D.OverlapBox(pos, Vector2.one*0.49f, 0, _obstacleLayer);

                if (_obstacle != null)
                {
                    //TODO 현재 선택된 블럭의 Color를 빯갛게 표시해준다.
                    isBuild = false;
                    _selectBlockPreview.color = Color.red;
                }
                else
                {
                    isBuild = true;
                    _selectBlockPreview.color = Color.white;
                }
            }
            else
            {
                isBuild = false;
                _selectBlockPreview.gameObject.SetActive(false);
            }
            yield return null;

        }
    }
}
