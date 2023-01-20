using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILockable
{
    public bool IsLocked { get; set; }
    public void Unlock() { }
    public void Lock() { }
}
