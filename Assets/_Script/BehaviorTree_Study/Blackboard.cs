using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Blackboard : MonoBehaviour
{
    public float timeOfDay;
    public Text clock;

    static Blackboard instance;
    public static Blackboard Instance
    {
        get
        {
            if (!instance)
            {
                Blackboard[] blackboards = GameObject.FindObjectsOfType<Blackboard>();
                if (blackboards != null)
                {
                    if (blackboards.Length == 1)
                    {
                        instance = blackboards[0];
                        return instance;
                    }
                }
                GameObject go = new GameObject("Blackboard", typeof(Blackboard));
                instance = go.GetComponent<Blackboard>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
        set
        {
            instance = value as Blackboard;
        }
        
    }

    void Start()
    {
        StartCoroutine("UpdateClock");
    }

    IEnumerator UpdateClock()
    {
        while(true)
        {
            timeOfDay++;
            if(timeOfDay > 23) timeOfDay = 0;
            clock.text = timeOfDay + " : 00";
            yield return new WaitForSeconds(1f);
        }
    }
}
