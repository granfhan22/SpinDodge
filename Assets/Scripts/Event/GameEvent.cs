using System.Collections;
using UnityEngine;

// Base class cho mọi event nguy hiểm mà EventManager có thể trigger.
public abstract class GameEvent : MonoBehaviour
{
    public abstract IEnumerator Execute();
}
