using UnityEngine;
using Unity.Entities;
using UnityEngine.Networking;

public partial class AIEndSessionSystem : SystemBase
{
    protected override void OnCreate()
    {
        Enabled = false;
    }

    protected override void OnStartRunning()
    {
        RefRW<AIComponent> aiComponent = SystemAPI.GetSingletonRW<AIComponent>();

        foreach (RefRW<AncestorComponent> ancestorComponent in SystemAPI.Query<RefRW<AncestorComponent>>())
        {
            if (ancestorComponent.ValueRO.isSelected)
            {
                string characterToDelete = ancestorComponent.ValueRO.name.ToString();
                characterToDelete = characterToDelete.ToLower();
                characterToDelete = characterToDelete.Replace(" ", "_");
                characterToDelete = characterToDelete.Replace(".", "");

                string apiRequest = string.Format("{0}/workspaces/{1}/characters/{2}",
                    aiComponent.ValueRO.baseUri,
                    aiComponent.ValueRO.workSpace,
                    characterToDelete);

                UnityWebRequest webRequest = UnityWebRequest.Delete(apiRequest);
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
            RefRW<AIComponent> aiComponent = SystemAPI.GetSingletonRW<AIComponent>();

            aiComponent.ValueRW.sessionID = "";
            aiComponent.ValueRW.characterSessionId = "";

            World.GetExistingSystemManaged<AISendTextSystem>().Enabled = false;

            World.GetExistingSystemManaged<ElevenSendTextSystem>().Enabled = false;

            foreach (RefRW<AncestorComponent> ancestorComponent in SystemAPI.Query<RefRW<AncestorComponent>>())
            {
                if (ancestorComponent.ValueRO.isSelected)
                {
                    ancestorComponent.ValueRW.isSelected = false;

                    Enabled = false;
                }
            }
        }
    }

    protected override void OnUpdate()
    {
        
    }
}
