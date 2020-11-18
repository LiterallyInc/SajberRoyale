using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    public static Core Instance;
    public const float SpawnOdds = 0.4f;
    // Start is called before the first frame update
    private void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
    private void ConfigureLoot()
    {

    }
}
