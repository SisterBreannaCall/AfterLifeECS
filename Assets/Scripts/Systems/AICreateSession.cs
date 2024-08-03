using UnityEngine;
using Unity.Entities;
using Newtonsoft.Json;
using OpenSessionResource;
using SessionInfoResource;
using UnityEngine.Networking;

public partial class AICreateSession : SystemBase
{
    protected override void OnCreate()
    {
        Enabled = false;
    }

    protected override void OnStartRunning()
    {
        RefRW<AIComponent> aiComponent = SystemAPI.GetSingletonRW<AIComponent>();
        RefRW<CurrentUserComponent> currentUserComponent = SystemAPI.GetSingletonRW<CurrentUserComponent>();

        foreach (RefRW<AncestorComponent> ancestorComponent in SystemAPI.Query<RefRW<AncestorComponent>>())
        {
            if (ancestorComponent.ValueRO.isSelected)
            {
                string characterName = ancestorComponent.ValueRO.name.ToString();
                characterName = characterName.ToLower();
                characterName = characterName.Replace(" ", "_");
                characterName = characterName.Replace(".", "");

                // Use Uri Runtime not BaseUri
                string apiRequest = string.Format("{0}/workspaces/{1}/characters/{2}:openSession",
                    aiComponent.ValueRO.baseUriRuntime,
                    aiComponent.ValueRO.workSpace,
                    characterName);

                string name = string.Format("/workspaces/{0}/characters/{1}",
                    aiComponent.ValueRO.workSpace,
                    characterName);

                OpenSessionJson openSessionJson = new OpenSessionJson();
                openSessionJson.name = name;
                openSessionJson.user = new User();
                openSessionJson.user.givenName = currentUserComponent.ValueRO.givenName.ToString();
                openSessionJson.user.gender = currentUserComponent.ValueRO.gender.ToString();

                string json = JsonConvert.SerializeObject(openSessionJson);

                UnityWebRequest webRequest = UnityWebRequest.Post(apiRequest, json, "application/json");
                webRequest.SetRequestHeader("authorization", $"Basic {aiComponent.ValueRO.clientIdRuntime}");

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

            SessionInfoJson sessionInfoJson = JsonConvert.DeserializeObject<SessionInfoJson>(op.webRequest.downloadHandler.text);

            aiComponent.ValueRW.sessionID = sessionInfoJson.name;

            aiComponent.ValueRW.characterSessionId = sessionInfoJson.sessionCharacters[0].character;

            GameManager.Instance.ActivateAiChat();

            World.GetExistingSystemManaged<AISendTextSystem>().Enabled = true;

            Enabled = false;
        }
    }

    protected override void OnUpdate()
    {
        
    }
}
