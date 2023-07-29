using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObservableSO : ScriptableObject
{
    List<IObserver> subscribers = new List<IObserver>();

    public void Subscribe(IObserver obs)
    {
        //Debug.Log("Subscribing");
        subscribers.Add(obs);
    }

    public void UnSubscribe(IObserver obs)
    {
        subscribers.Remove(obs);
    }

    public void Notify()
    {
        foreach(IObserver subscriber in subscribers)
        {
            //Debug.Log("Notify");
            subscriber.Notify();
        }
    }
}
