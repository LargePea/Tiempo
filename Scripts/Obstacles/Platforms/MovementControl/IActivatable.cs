using UnityEngine;
using UnityEngine.Events;

public interface IActivatable
{
    bool IsActivated { get; set; }
    UnityEvent OnActivationUpdate { get; }

    public void LockActivateable();
    public void UnlockActivateable();
}
