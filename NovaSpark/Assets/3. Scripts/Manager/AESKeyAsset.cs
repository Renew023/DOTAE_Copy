using UnityEngine;

[CreateAssetMenu(fileName = "AesKeyAsset", menuName = "ScriptableObjects/AES Key Asset")]
public class AesKeyAsset : ScriptableObject
{
    [TextArea] public string key;
    [TextArea] public string iv;
}