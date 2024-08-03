using UnityEngine;
using Unity.Entities;
using UnityEngine.Networking;
using AncestorInfoResource;
using Newtonsoft.Json;

public partial class FSAncestorInfoSystem : SystemBase
{
    protected override void OnCreate()
    {
        Enabled = false;
    }

    protected override void OnStartRunning()
    {
        RefRW<FSComponent> fsComponent = SystemAPI.GetSingletonRW<FSComponent>();

        foreach (RefRW<AncestorComponent> ancestorComponent in SystemAPI.Query<RefRW<AncestorComponent>>())
        {
            if (ancestorComponent.ValueRO.isSelected)
            {
                string apiRoute = "platform/tree/persons/";
                string apiRequest = string.Format("{0}{1}{2}",
                    fsComponent.ValueRO.regProduction,
                    apiRoute,
                    ancestorComponent.ValueRO.pid);

                UnityWebRequest webRequest = UnityWebRequest.Get(apiRequest);
                webRequest.SetRequestHeader("Accept", "application/x-gedcomx-v1+json");
                webRequest.SetRequestHeader("Authorization", "Bearer " + fsComponent.ValueRO.accessToken);

                webRequest.downloadHandler = new DownloadHandlerBuffer();

                UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();
                asyncOperation.completed += (AsyncOperation op) => { ServerResponse(asyncOperation); };
            }
        }
    }

    private void ServerResponse(UnityWebRequestAsyncOperation op)
    {
        AncestorInfoJson ancestorInfoJson = JsonConvert.DeserializeObject<AncestorInfoJson>(op.webRequest.downloadHandler.text);

        foreach (RefRW<AncestorComponent> ancestorComponent in SystemAPI.Query<RefRW<AncestorComponent>>())
        {
            if (ancestorComponent.ValueRO.isSelected)
            {
                ancestorComponent.ValueRW.birthDate = ancestorInfoJson.persons[0].display.birthDate;
                ancestorComponent.ValueRW.birthLocation = ancestorInfoJson.persons[0].display.birthPlace;
                ancestorComponent.ValueRW.deathDate = ancestorInfoJson.persons[0].display.deathDate;
                ancestorComponent.ValueRW.deathLocation = ancestorInfoJson.persons[0].display.deathPlace;
            }
        }

        World.GetExistingSystemManaged<AICreateSystem>().Enabled = true;

        Enabled = false;
    }

    protected override void OnUpdate()
    {
           
    }

    public void StartSystem(string pid)
    {
        foreach (RefRW<AncestorComponent> ancestorComponent in SystemAPI.Query<RefRW<AncestorComponent>>())
        {
            if (ancestorComponent.ValueRO.pid == pid)
            {
                ancestorComponent.ValueRW.isSelected = true;
            }
        }

        Enabled = true;
    }
}
