using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

public struct BakeMeshJob : IJobParallelFor
{
    [ReadOnly]
    [DeallocateOnJobCompletion]

    public NativeArray<int> MeshIds;

    public void Execute(int index)
    {
        Physics.BakeMesh(MeshIds[index], false);
    }
}
