using System.Collections;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private GameEvent[] events;
    [SerializeField] private float intervalSeconds = 5f;

    private int lastEventIndex = -1;

    private void Start()
    {
        StartCoroutine(EventLoop());
    }

    private IEnumerator EventLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervalSeconds);

            int index = PickNextEventIndex();
            if (index < 0) continue;

            lastEventIndex = index;
            StartCoroutine(events[index].Execute());
        }
    }

    //random event no same on a row
    private int PickNextEventIndex()
    {
        if (events == null || events.Length == 0) return -1;
        if (events.Length == 1) return 0;

        int index;
        do
        {
            index = Random.Range(0, events.Length);
        } while (index == lastEventIndex);

        return index;
    }
}
