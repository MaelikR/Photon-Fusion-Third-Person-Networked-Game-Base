using Fusion;
using UnityEngine;

public class HousingSystem : NetworkBehaviour
{
    public NetworkPrefabRef housePrefab; // Utilise NetworkPrefabRef pour les objets de Fusion
    private NetworkObject currentHouse;

    public void BuyHouse(Vector3 position)
    {
        if (Object.HasInputAuthority)
        {
            if (currentHouse == null)
            {
                currentHouse = Runner.Spawn(housePrefab, position, Quaternion.identity, Object.InputAuthority);
                RPC_NotifyHouseBought();
            }
        }
    }

    public void CustomizeHouse(string customization)
    {
        if (currentHouse != null && Object.HasInputAuthority)
        {
            Debug.Log("House customized with " + customization);
            RPC_NotifyHouseCustomized(customization);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_NotifyHouseBought()
    {
        Debug.Log("House bought and placed.");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_NotifyHouseCustomized(string customization)
    {
        Debug.Log("House customized: " + customization);
    }
}
