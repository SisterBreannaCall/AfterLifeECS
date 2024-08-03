using UnityEngine;
using Unity.Entities;
using UnityEngine.Networking;

public partial class AIDeploySystem : SystemBase
{
    protected override void OnCreate()
    {
        Enabled = false;
    }

    protected override void OnStartRunning()
    {
        RefRW<AIComponent> aiComponent = SystemAPI.GetSingletonRW<AIComponent>();

        foreach(RefRW<AncestorComponent> ancestorComponent in SystemAPI.Query<RefRW<AncestorComponent>>())
        {
            if (ancestorComponent.ValueRO.isSelected)
            {
                string characterName = ancestorComponent.ValueRO.name.ToString();
                characterName = characterName.ToLower();
                characterName = characterName.Replace(" ", "_");
                characterName = characterName.Replace(".", "");

                string apiRequest = string.Format("{0}/workspaces/{1}/characters/{2}:deploy",
                    aiComponent.ValueRO.baseUri,
                    aiComponent.ValueRO.workSpace,
                    characterName);

                UnityWebRequest webRequest = UnityWebRequest.Post(apiRequest, "", "application/json");
                webRequest.SetRequestHeader("Grpc-Metadata-X-Authorization-Bearer-Type", "studio_api");
                webRequest.SetRequestHeader("Authorization", $"Basic {aiComponent.ValueRO.clientId}");

                webRequest.downloadHandler = new DownloadHandlerBuffer();

                UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();
                asyncOperation.completed += (AsyncOperation op) => { ServerResponse(asyncOperation); };
            }
        }
    }

    private void ServerResponse(UnityWebRequestAsyncOperation op)
    {
        if (op.webRequest.responseCode == 200)
        {
            World.GetExistingSystemManaged<AICreateSession>().Enabled = true;
        }

        Enabled = false;
    }

    protected override void OnUpdate()
    {
        
    }
}
