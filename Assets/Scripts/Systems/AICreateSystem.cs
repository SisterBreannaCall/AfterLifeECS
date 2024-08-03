using UnityEngine;
using Unity.Entities;
using Newtonsoft.Json;
using UnityEngine.Networking;
using CreateCharacterResource;

public partial class AICreateSystem : SystemBase
{
    protected override void OnCreate()
    {
        Enabled = false;
    }

    protected override void OnUpdate()
    {
        
    }

    protected override void OnStartRunning()
    {
        RefRW<AIComponent> aiComponent = SystemAPI.GetSingletonRW<AIComponent>();

        foreach (RefRW<AncestorComponent> ancestorComponent in SystemAPI.Query<RefRW<AncestorComponent>>())
        {
            if (ancestorComponent.ValueRO.isSelected)
            {
                CreateCharJson createCharJson = new CreateCharJson();
                createCharJson.defaultCharacterDescription = new DefaultCharacterDescription();
                createCharJson.defaultCharacterDescription.givenName = ancestorComponent.ValueRO.name.ToString();
                createCharJson.defaultCharacterDescription.chracterRole = "storyteller";

                if (ancestorComponent.ValueRO.sex == "Male")
                {
                    string description = string.Format("{0} was born on {1} at {2}. He died on {3} at {4}. His lifespan was {5}. His person id number on FamilySearch is {6}.",
                        "{character}",
                        ancestorComponent.ValueRO.birthDate,
                        ancestorComponent.ValueRO.birthLocation,
                        ancestorComponent.ValueRO.deathDate,
                        ancestorComponent.ValueRO.deathLocation,
                        ancestorComponent.ValueRO.lifeSpan,
                        ancestorComponent.ValueRO.pid);

                    createCharJson.defaultCharacterDescription.description = description;
                }
                else if (ancestorComponent.ValueRO.sex == "Female")
                {
                    string description = string.Format("{0} was born on {1} at {2}. She died on {3} at {4}. Her lifespan was {5}. Her person id number on FamilySearch is {6}.",
                        "{character}",
                        ancestorComponent.ValueRO.birthDate,
                        ancestorComponent.ValueRO.birthLocation,
                        ancestorComponent.ValueRO.deathDate,
                        ancestorComponent.ValueRO.deathLocation,
                        ancestorComponent.ValueRO.lifeSpan,
                        ancestorComponent.ValueRO.pid);

                    createCharJson.defaultCharacterDescription.description = description;
                }

                string json = JsonConvert.SerializeObject(createCharJson);

                string apiRequest = string.Format("{0}/workspaces/{1}/characters",
                    aiComponent.ValueRO.baseUri,
                    aiComponent.ValueRO.workSpace);

                UnityWebRequest webRequest = UnityWebRequest.Post(apiRequest, json, "application/json");
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
            World.GetExistingSystemManaged<AIDeploySystem>().Enabled = true;
        }

        Enabled = false;
    }
}
