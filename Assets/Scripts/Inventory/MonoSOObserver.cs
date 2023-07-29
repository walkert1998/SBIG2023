using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSOObserver : MonoBehaviour, IObserver
{
    public ObservableSO observable;
    private bool subscribed = false;

    protected virtual void OnEnable()
    {
        Subscribe();
    }

    protected virtual void OnDisable()
    {
        UnSubscribe();
    }

    private void Subscribe()
    {
        if (!subscribed)
        {
            observable.Subscribe(this);
            subscribed = true;
        }
    }

    private void UnSubscribe()
    {
        if (subscribed)
        {
            observable.UnSubscribe(this);
            subscribed = false;
        }
    }

    public abstract void Notify();
}
