using UnityEngine;
using Unity.Entities;
using UnityEngine.Networking;
using System;

public partial class AIListCharactersSystem : SystemBase
{
    protected override void OnCreate()
    {
        Enabled = false;
    }

    protected override void OnStartRunning()
    {
        RefRW<AIComponent> aiComponent = SystemAPI.GetSingletonRW<AIComponent>();

        string apiRequest = string.Format("{0}/workspaces/{1}/characters",
            aiComponent.ValueRO.baseUri,
            aiComponent.ValueRO.workSpace);

        UnityWebRequest webRequest = UnityWebRequest.Get(apiRequest);
        webRequest.SetRequestHeader("Grpc-Metadata-X-Authorization-Bearer-Type", "studio_api");
        webRequest.SetRequestHeader("Authorization", $"Basic {aiComponent.ValueRO.clientId}");

        webRequest.downloadHandler = new DownloadHandlerBuffer();

        UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();
        asyncOperation.completed += (AsyncOperation op) => { ServerResponse(asyncOperation); };
    }

    private void ServerResponse(UnityWebRequestAsyncOperation op)
    {
        Debug.Log(op.webRequest.downloadHandler.text);

        Enabled = false;
    }

    protected override void OnUpdate()
    {
        
    }
}
