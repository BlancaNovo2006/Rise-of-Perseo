using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<GameObject> enemigos = new List<GameObject>();
    public static EnemyManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void RegistrarEnemigo(GameObject enemigo)
    {
        enemigo.AddComponent(enemigo);
    }
    public void EliminarEnemigo(GameObject enemigo)
    {
        Destroy(gameObject);
    }
    public bool TodosMuertos()
    {
        return enemigos.Count == 0;
    }
}
