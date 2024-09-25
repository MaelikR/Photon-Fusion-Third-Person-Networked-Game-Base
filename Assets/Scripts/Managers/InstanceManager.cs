using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class InstanceManager : NetworkBehaviour
{
    private Dictionary<string, List<string>> instances = new Dictionary<string, List<string>>();

    public void CreateInstance(string instanceName)
    {
        if (Object.HasStateAuthority && !instances.ContainsKey(instanceName))
        {
            instances.Add(instanceName, new List<string>());
            RPC_NotifyInstanceUpdate("Instance " + instanceName + " created.");
        }
    }

    public void JoinInstance(string instanceName, string playerName)
    {
        if (Object.HasStateAuthority && instances.ContainsKey(instanceName))
        {
            instances[instanceName].Add(playerName);
            RPC_NotifyInstanceUpdate(playerName + " joined instance " + instanceName);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_NotifyInstanceUpdate(string message)
    {
        Debug.Log(message);
    }
}
