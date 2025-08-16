using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogSystem : MonoBehaviour
{
    public Texture2D fogOfWarTexture;
    public SpriteMask spriteMask;
    public float mapSize;

    private Vector2 worldScale;
    private Vector2Int pixelScale;

    public void Awake() //초기화
    {
        pixelScale.x = fogOfWarTexture.width; //512
        pixelScale.y = fogOfWarTexture.height; //512
        worldScale.x = pixelScale.x / mapSize; //transform.localScale.x;/// 100f
        worldScale.y = pixelScale.y / mapSize;//transform.localScale.y;//100f
        for(int i=0; i< pixelScale.x; i++)
        {
            for (int j=0; j<pixelScale.y; j++)
            {
                fogOfWarTexture.SetPixel(i, j, Color.clear);
            }
        }
    }

    private Vector2Int WorldToPixel(Vector2 position) //좌표 중심 찾기
    {
        Vector2Int pixelPosition = Vector2Int.zero;

        float dx = position.x - transform.localPosition.x;
        float dy = position.y - transform.localPosition.y;

        pixelPosition.x = Mathf.RoundToInt(.5f * pixelScale.x + dx * (pixelScale.x / worldScale.x));
        pixelPosition.y = Mathf.RoundToInt(.5f * pixelScale.y + dy * (pixelScale.y / worldScale.y));

        return pixelPosition;
    }

    public void MakeHole(Vector2 position, float holeRadius) //구멍 만들기
    {
        Vector2Int pixelPosition = WorldToPixel(position);
        int radius = Mathf.RoundToInt(holeRadius * pixelScale.x / worldScale.x);
        int px, nx, py, ny, distance;
        for(int i=0; i < radius; i++)
        {
            distance = Mathf.RoundToInt(Mathf.Sqrt(radius * radius - i * i));
            for(int j=0; j< distance; j++)
            {
                px = Mathf.Clamp(pixelPosition.x + i, 0, pixelScale.x);
                nx = Mathf.Clamp(pixelPosition.x - i, 0, pixelScale.x);
                py = Mathf.Clamp(pixelPosition.y + j, 0, pixelScale.y);
                ny = Mathf.Clamp(pixelPosition.y - j, 0, pixelScale.y);

                fogOfWarTexture.SetPixel(px, py, Color.black);
                fogOfWarTexture.SetPixel(nx, py, Color.black);
                fogOfWarTexture.SetPixel(px, ny, Color.black);
                fogOfWarTexture.SetPixel(nx, ny, Color.black);
            }
        }
        fogOfWarTexture.Apply();
        CreateSprite();
    }
    private void CreateSprite() //구멍으로 만들 스프라이트
    {
        spriteMask.sprite = Sprite.Create(fogOfWarTexture, new Rect(0, 0, fogOfWarTexture.width, fogOfWarTexture.height), Vector2.one * .5f, 40);
    }
}
