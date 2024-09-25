using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class GuildSystem : NetworkBehaviour
{
    public List<Guild> guilds = new List<Guild>(); // Retirer [Networked]

    public void CreateGuild(string guildName, string founderName)
    {
        if (Object.HasStateAuthority)
        {
            Guild newGuild = new Guild(guildName, founderName);
            guilds.Add(newGuild);
            RPC_NotifyGuildUpdate(founderName + " has founded the guild " + guildName);
        }
    }

    public void AddMemberToGuild(Guild guild, string playerName)
    {
        if (Object.HasStateAuthority && guilds.Contains(guild))
        {
            guild.guildMembers.Add(playerName);
            RPC_NotifyGuildUpdate(playerName + " has joined the guild " + guild.guildName);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_NotifyGuildUpdate(string message)
    {
        Debug.Log(message);
    }
}

[System.Serializable]
public class Guild
{
    public string guildName;
    public List<string> guildMembers;

    public Guild(string name, string founder)
    {
        guildName = name;
        guildMembers = new List<string> { founder };
    }
}
