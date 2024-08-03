using UnityEngine;
using Unity.Entities;
using Newtonsoft.Json;
using ElevenLabsResource;
using UnityEngine.Networking;

public partial class ElevenSendTextSystem : SystemBase
{
    protected override void OnCreate()
    {
        Enabled = false;
    }

    public void SendText(string textToSend)
    {
        RefRW<ElevenComponent> elevenComponent = SystemAPI.GetSingletonRW<ElevenComponent>();

        ElevenLabsJson elevenLabsJson = new ElevenLabsJson();
        elevenLabsJson.text = textToSend;
        elevenLabsJson.model_id = "eleven_monolingual_v1";

        string json = JsonConvert.SerializeObject(elevenLabsJson);

        foreach(RefRW<AncestorComponent> ancestorComponent in SystemAPI.Query<RefRW<AncestorComponent>>())
        {
            if (ancestorComponent.ValueRO.isSelected)
            {
                if (ancestorComponent.ValueRO.sex == "Male")
                {
                    UnityWebRequest webRequest = UnityWebRequest.Post(elevenComponent.ValueRO.maleVoice2.ToString(), json, "application/json");
                    webRequest.SetRequestHeader("Accept", "audio/mpeg");
                    webRequest.SetRequestHeader("xi-api-key", elevenComponent.ValueRO.clientId.ToString());

                    webRequest.downloadHandler = new DownloadHandlerAudioClip("", AudioType.MPEG);

                    UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();
                    asyncOperation.completed += (AsyncOperation op) => { ServerResponse(asyncOperation); };
                }

                if (ancestorComponent.ValueRO.sex == "Female")
                {
                    UnityWebRequest webRequest = UnityWebRequest.Post(elevenComponent.ValueRO.femaleVoice.ToString(), json, "application/json");
                    webRequest.SetRequestHeader("Accept", "audio/mpeg");
                    webRequest.SetRequestHeader("xi-api-key", elevenComponent.ValueRO.clientId.ToString());

                    webRequest.downloadHandler = new DownloadHandlerAudioClip("", AudioType.MPEG);

                    UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();
                    asyncOperation.completed += (AsyncOperation op) => { ServerResponse(asyncOperation); };
                }
            }
        }
    }

    private void ServerResponse(UnityWebRequestAsyncOperation op)
    {
        GameManager.Instance.PlaySound(DownloadHandlerAudioClip.GetContent(op.webRequest));
    }

    protected override void OnStartRunning()
    {
        
    }

    protected override void OnUpdate()
    {
        
    }
}
