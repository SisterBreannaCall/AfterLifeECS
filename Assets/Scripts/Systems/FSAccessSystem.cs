using UnityEngine;
using Unity.Entities;
using Newtonsoft.Json;
using AccessTokenResource;
using UnityEngine.Networking;
using System.Collections.Generic;

public partial class FSAccessSystem : SystemBase
{
    protected override void OnCreate()
    {
        Enabled = false;
    }

    protected override void OnStartRunning()
    {
        RefRW<FSComponent> fsComponent = SystemAPI.GetSingletonRW<FSComponent>();

        Dictionary<string, string> formData = new Dictionary<string, string>();
        formData.Add("code", fsComponent.ValueRO.authCode.ToString());
        formData.Add("grant_type", "authorization_code");
        formData.Add("client_id", fsComponent.ValueRO.clientID.ToString());
        formData.Add("code_verifier", fsComponent.ValueRO.codeVerifier.ToString());

        UnityWebRequest webRequest = UnityWebRequest.Post(fsComponent.ValueRO.tokenProduction.ToString(), formData);
        webRequest.SetRequestHeader("Accept", "application/json");
        webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        webRequest.downloadHandler = new DownloadHandlerBuffer();

        UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();
        asyncOperation.completed += (AsyncOperation op) => { ServerResponse(asyncOperation); };
    }

    private void ServerResponse(UnityWebRequestAsyncOperation op)
    {
        RefRW<FSComponent> fsComponent = SystemAPI.GetSingletonRW<FSComponent>();

        AccessTokenJson accessTokenJson = JsonConvert.DeserializeObject<AccessTokenJson>(op.webRequest.downloadHandler.text);
        fsComponent.ValueRW.accessToken = accessTokenJson.access_token;
        fsComponent.ValueRW.idToken = accessTokenJson.id_token;

        World.GetExistingSystemManaged<FSDecodeJwtSystem>().Enabled = true;

        Enabled = false;
    }

    protected override void OnUpdate()
    {
        
    }
}
