using UnityEngine;
using Unity.Entities;
using Newtonsoft.Json;
using SendTextResource;
using ReceiveTextResource;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

public partial class AISendTextSystem : SystemBase
{
    protected override void OnCreate()
    {
        Enabled = false;
    }

    protected override void OnStartRunning()
    {
        World.GetExistingSystemManaged<ElevenSendTextSystem>().Enabled = true;

        RefRW<AIComponent> aiComponent = SystemAPI.GetSingletonRW<AIComponent>();

        string apiRequest = string.Format("{0}/workspaces/{1}/sessions/{2}/sessionCharacters/{3}:sendText",
            aiComponent.ValueRO.baseUriRuntime,
            aiComponent.ValueRO.workSpace,
            aiComponent.ValueRO.sessionID,
            aiComponent.ValueRO.characterSessionId);

        SendTextJson sendTextJson = new SendTextJson();
        sendTextJson.text = "Hello";

        string json = JsonConvert.SerializeObject(sendTextJson);

        UnityWebRequest webRequest = UnityWebRequest.Post(apiRequest, json, "application/json");
        webRequest.SetRequestHeader("authorization", $"Basic {aiComponent.ValueRO.clientIdRuntime}");
        webRequest.SetRequestHeader("Grpc-Metadata-session-id", aiComponent.ValueRO.sessionID.ToString());

        webRequest.downloadHandler = new DownloadHandlerBuffer();

        UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();
        asyncOperation.completed += (AsyncOperation op) => { ServerResponse(asyncOperation); };
    }

    public void SendText(string playerMessage)
    {
        string sanitized = Regex.Replace(playerMessage, "<.*?", string.Empty);

        sanitized = System.Net.WebUtility.HtmlEncode(sanitized);

        RefRW<AIComponent> aiComponent = SystemAPI.GetSingletonRW<AIComponent>();

        string apiRequest = string.Format("{0}/workspaces/{1}/sessions/{2}/sessionCharacters/{3}:sendText",
            aiComponent.ValueRO.baseUriRuntime,
            aiComponent.ValueRO.workSpace,
            aiComponent.ValueRO.sessionID,
            aiComponent.ValueRO.characterSessionId);

        SendTextJson sendTextJson = new SendTextJson();
        sendTextJson.text = sanitized;

        string json = JsonConvert.SerializeObject(sendTextJson);

        UnityWebRequest webRequest = UnityWebRequest.Post(apiRequest, json, "application/json");
        webRequest.SetRequestHeader("authorization", $"Basic {aiComponent.ValueRO.clientIdRuntime}");
        webRequest.SetRequestHeader("Grpc-Metadata-session-id", aiComponent.ValueRO.sessionID.ToString());

        webRequest.downloadHandler = new DownloadHandlerBuffer();

        UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();
        asyncOperation.completed += (AsyncOperation op) => { ServerResponse(asyncOperation); };
    }

    private void ServerResponse(UnityWebRequestAsyncOperation op)
    {
        if (op.webRequest.responseCode == 200)
        {
            ReceiveTextJson receiveTextJson = JsonConvert.DeserializeObject<ReceiveTextJson>(op.webRequest.downloadHandler.text);

            string aiResponse = "";

            for (int i = 0; i < receiveTextJson.textList.Count; i++)
            {
                aiResponse = aiResponse + receiveTextJson.textList[i];
            }

            World.GetExistingSystemManaged<ElevenSendTextSystem>().SendText(aiResponse);

            GameManager.Instance.SetAIText(aiResponse);
        }
    }

    protected override void OnUpdate()
    {
        
    }
}
