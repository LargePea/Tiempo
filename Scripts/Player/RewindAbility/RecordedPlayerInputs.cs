using System.Collections.Generic;
using System;
using UnityEngine;

//Packaged position and rotation values for events
[Serializable]
public struct RecordedPlayerInputs
{
    public List<Vector3> Positions { get; private set; }
    public List<Vector3> Rotations { get; private set; }
    public List<Vector3> CameraPositions { get; private set; }
    public List<Vector3> CameraRotations { get; private set; }
    public List<float> JumpInputs { get; private set; }
    public List<float> JumpCancels { get; private set; }
    public List<float> PauseInputs { get; private set; }

    public List<float> UnpauseInputs { get; private set; }

    public RecordedPlayerInputs(List<Vector3> Positions, List<Vector3> Rotations, List<Vector3> CameraPositions, List<Vector3> CameraRotations,  List<float> JumpInputs, List<float> JumpCancels, List<float> PauseInputs, List<float> UnpauseInputs)
    {
        this.Positions = Positions;
        this.Rotations = Rotations;
        this.JumpInputs = JumpInputs;
        this.PauseInputs = PauseInputs;
        this.JumpCancels = JumpCancels;
        this.UnpauseInputs = UnpauseInputs;
        this.CameraPositions = CameraPositions;
        this.CameraRotations = CameraRotations;
    }

    public void Clear()
    {
        Positions?.Clear();
        Rotations?.Clear();
        JumpInputs?.Clear();
        PauseInputs?.Clear();
        JumpCancels?.Clear();
        UnpauseInputs?.Clear();
        CameraPositions?.Clear();
        CameraRotations?.Clear();
    }
}
