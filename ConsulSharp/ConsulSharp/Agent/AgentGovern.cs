﻿using ConsulSharp.Health;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsulSharp.Agent
{
    /// <summary>
    /// Service Govern
    /// </summary>
    public class AgentGovern : Govern
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseAddress">Base Address</param>
        public AgentGovern(string baseAddress = "http://localhost:8500") : base(baseAddress)
        {
        }

        #region Agent

        /// <summary>
        /// List Members,This endpoint returns the members the agent sees in the cluster gossip pool. Due to the nature of gossip, this is eventually consistent: the results may differ by agent. The strongly consistent view of nodes is instead provided by /v1/catalog/nodes.
        /// </summary>
        /// <returns></returns>
        public async Task<Member[]> ListMembers(ListMembersParmeter listMembersParmeter)
        {
            return await Get<Member[], ListMembersParmeter>("/agent/members", listMembersParmeter);
        }

        /// <summary>
        /// This endpoint returns the configuration and member information of the local agent. The Config element contains a subset of the configuration and its format will not change in a backwards incompatible way between releases. DebugConfig contains the full runtime configuration but its format is subject to change without notice or deprecation.
        /// </summary>
        /// <returns></returns>
        public async Task<ReadConfigurationResult> ReadConfiguration()
        {
            return await Get<ReadConfigurationResult>("/agent/self");
        }

        /// <summary>
        /// This endpoint instructs the agent to reload its configuration. Any errors encountered during this process are returned.Not all configuration options are reloadable.See the Reloadable Configuration section on the agent options page for details on which options are supported.
        /// </summary>
        /// <returns></returns>    
        public async Task<(bool result, string backJson)> ReloadAgent()
        {
            return await Put("", $"/agent/reload");
        }
        /// <summary>
        /// This endpoint places the agent into "maintenance mode". During maintenance mode, the node will be marked as unavailable and will not be present in DNS or API queries. This API call is idempotent.        Maintenance mode is persistent and will be automatically restored on agent restart.
        /// </summary>
        /// <returns></returns>    
        public async Task<(bool result, string backJson)> EnableMaintenanceMode(EnableMaintenanceModeParmeter  enableMaintenanceModeParmeter)
        {
            return await Put(enableMaintenanceModeParmeter, $"/agent/maintenance");
        }

        /// <summary>
        /// This endpoint returns the configuration and member information of the local agent.
        /// </summary>
        /// <returns></returns>
        public async Task<ViewMetricsResult> ViewMetrics()
        {
            return await Get<ViewMetricsResult>("/agent/metrics");
        }
        /// <summary>
        /// WriteLog
        /// </summary>
        public event WriteLogHandle WriteLog;
        /// <summary>
        /// WriteLog Delegate
        /// </summary>
        /// <param name="log"></param>
        public delegate void WriteLogHandle(string log);
        /// <summary>
        /// This endpoint streams logs from the local agent until the connection is closed.Must subscription WriteLog event.
        /// </summary>
        /// <returns></returns>
        public async Task StreamLogs()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri($"{_baseAddress}");
            var stream = await client.GetStreamAsync("/agent/monitor");
            while (stream.CanRead)
            {
                var bytes = new byte[1024];
                var result = await stream.ReadAsync(bytes, 0, bytes.Length);
                WriteLog?.Invoke(Encoding.Default.GetString(bytes).Trim('\0'));
            }
        }
        /// <summary>
        /// This endpoint instructs the agent to attempt to connect to a given address.
        /// </summary>
        /// <returns></returns>    
        public async Task<(bool result, string backJson)> JoinAgent(JoinAgentParmeter joinAgentParmeter)
        {
            return await Put(joinAgentParmeter, $"/agent/join");
        }

        /// <summary>
        /// This endpoint triggers a graceful leave and shutdown of the agent. It is used to ensure other nodes see the agent as "left" instead of "failed". Nodes that leave will not attempt to re-join the cluster on restarting with a snapshot.For nodes in server mode, the node is removed from the Raft peer set in a graceful manner.This is critical, as in certain situations a non-graceful leave can affect cluster availability.
        /// </summary>
        /// <returns></returns>    
        public async Task<(bool result, string backJson)> GracefulLeaveAndShutdown()
        {
            return await Put("", $"/agent/leave");
        }
        /// <summary>
        /// This endpoint instructs the agent to force a node into the left state. If a node fails unexpectedly, then it will be in a failed state. Once in the failed state, Consul will attempt to reconnect, and the services and checks belonging to that node will not be cleaned up. Forcing a node into the left state allows its old entries to be removed.
        /// </summary>
        /// <returns></returns>    
        public async Task<(bool result, string backJson)> ForceLeaveAndShutdown()
        {
            return await Put("", $"/agent/force-leave");
        }

        /// <summary>
        /// This endpoint updates the ACL tokens currently in use by the agent. It can be used to introduce ACL tokens to the agent for the first time, or to update tokens that were initially loaded from the agent's configuration. Tokens are not persisted, so will need to be updated again if the agent is restarted.
        /// </summary>
        /// <returns></returns>    
        public async Task<(bool result, string backJson)> UpdateACLToken(UpdateTokenParmeter token)
        {
            return await Put(token, $"/agent/acl_token");
        }
        /// <summary>
        ///  This endpoint updates the ACL tokens currently in use by the agent. It can be used to introduce ACL tokens to the agent for the first time, or to update tokens that were initially loaded from the agent's configuration. Tokens are not persisted, so will need to be updated again if the agent is restarted.
        /// </summary>
        /// <returns></returns>    
        public async Task<(bool result, string backJson)> UpdateACLAgentToken(UpdateTokenParmeter token)
        {
            return await Put(token, $"/agent/acl_agent_token");
        }
        /// <summary>
        ///  This endpoint updates the ACL tokens currently in use by the agent. It can be used to introduce ACL tokens to the agent for the first time, or to update tokens that were initially loaded from the agent's configuration. Tokens are not persisted, so will need to be updated again if the agent is restarted.
        /// </summary>
        /// <returns></returns>    
        public async Task<(bool result, string backJson)> UpdateACLAgentMasterToken(UpdateTokenParmeter token)
        {
            return await Put(token, $"/agent/acl_agent_master_token");
        }
        /// <summary>
        ///  This endpoint updates the ACL tokens currently in use by the agent. It can be used to introduce ACL tokens to the agent for the first time, or to update tokens that were initially loaded from the agent's configuration. Tokens are not persisted, so will need to be updated again if the agent is restarted.
        /// </summary>
        /// <returns></returns>    
        public async Task<(bool result, string backJson)> UpdateACLReplicationToken(UpdateTokenParmeter token)
        {
            return await Put(token, $"/agent/acl_replication_token");
        }
  

        #endregion

        #region Check
        /// <summary>
        /// view metrics
        /// </summary>
        /// <returns></returns>
        public async Task<QueryCheck> ListChecks()
        {
            return await Get<QueryCheck>("/agent/checks");
        }
        /// <summary>
        /// register check
        /// </summary>
        /// <returns></returns>    
        public async Task<(bool result, string backJson)> RegisterCheck(RegisterCheck check)
        {
            return await Put(check, $"/agent/check/register");
        }
        /// <summary>
        /// deregister check
        /// </summary>
        /// <returns></returns>    
        public async Task<(bool result, string backJson)> DeregisterCheck(string checkID)
        {
            return await Put("", $"/agent/check/deregister/{checkID}");
        }
        /// <summary>
        /// TTL check pass
        /// </summary>
        /// <returns></returns>    
        public async Task<(bool result, string backJson)> TTLCheckPass(TTLCheckOpt checkPass)
        {
            return await Put(checkPass, $"/agent/check/pass");
        }
        /// <summary>
        /// TTL check warn
        /// </summary>
        /// <returns></returns>    
        public async Task<(bool result, string backJson)> TTLCheckWarn(TTLCheckOpt checkPass)
        {
            return await Put(checkPass, $"/agent/check/warn");
        }
        /// <summary>
        /// TTL check fail
        /// </summary>
        /// <returns></returns>    
        public async Task<(bool result, string backJson)> TTLCheckFail(TTLCheckOpt checkPass)
        {
            return await Put(checkPass, $"/agent/check/fail");
        }
        /// <summary>
        /// TTL check update
        /// </summary>
        /// <returns></returns>    
        public async Task<(bool result, string backJson)> TTLCheckUpdate(TTLCheckUpdate checkUpdate)
        {
            return await Put(checkUpdate, $"/agent/check/update");
        }
        #endregion

        #region service
        /// <summary>
        /// List Services
        /// </summary>
        /// <returns></returns>    
        public async Task<Dictionary<string, ListService>> ListServices(TTLCheckOpt checkPass)
        {
            return await Get<Dictionary<string, ListService>>($"/agent/services");
        }

        /// <summary>
        /// register service
        /// </summary>
        /// <returns></returns>
        /// <param name="service">service</param>
        public async Task<(bool result, string backJson)> RegisterServices(Service service)
        {
            return await Put(service, $"/agent/service/register");
        }
        /// <summary>
        /// deregister service
        /// </summary>
        /// <returns></returns>
        /// <param name="serviceID">service ID</param>
        public async Task<(bool result, string backJson)> DeregisterServices(string serviceID)
        {
            return await Put("", $"/agent/service/deregister/{ serviceID}");
            //var client = new HttpClient();
            //client.BaseAddress = new Uri(_baseAddress);
            //var response = await client.PutAsync($"/agent/service/deregister/" + serviceID, null);
            //var backJson = await response.Content.ReadAsStringAsync();
            //return (result: response.StatusCode == System.Net.HttpStatusCode.OK, backJson: backJson);
        }
        #endregion

    }
}
