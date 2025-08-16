using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<T>();   //찾아보고
                if (instance == null)   //없으면 싱글톤 오브젝트 생성
                {
                    GameObject go = new GameObject();
                    go.name = typeof(T).Name;
                    instance = go.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this as T;   //T 타입을 가진 인스턴스 생성
        //    //DontDestroyOnLoad(gameObject);
        //}
        //else if(instance != this)
        //{
        //    Destroy(gameObject);
        //}
    }
}
