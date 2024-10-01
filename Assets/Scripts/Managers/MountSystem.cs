using Fusion;
using UnityEngine;

public class MountSystem : NetworkBehaviour
{
    public NetworkPrefabRef mountPrefab;
    private NetworkObject currentMount;

    public void SummonMount(Vector3 position)
    {
        if (Object.HasInputAuthority)
        {
            if (currentMount == null)
            {
                currentMount = Runner.Spawn(mountPrefab, position, Quaternion.identity, Object.InputAuthority);
                RPC_NotifyMountSummon();
            }
            else
            {
                Dismount();
            }
        }
    }

    public void Dismount()
    {
        if (Object.HasInputAuthority && currentMount != null)
        {
            Runner.Despawn(currentMount);
            currentMount = null;
            RPC_NotifyMountDismount();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_NotifyMountSummon()
    {
        Debug.Log("Mount summoned.");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_NotifyMountDismount()
    {
        Debug.Log("Dismounted.");
    }
}
