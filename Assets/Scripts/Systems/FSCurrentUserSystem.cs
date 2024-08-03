using UnityEngine;
using Unity.Entities;
using Newtonsoft.Json;
using CurrentUserResource;
using UnityEngine.Networking;

public partial class FSCurrentUserSystem : SystemBase
{
    protected override void OnCreate()
    {
        Enabled = false;
    }

    protected override void OnStartRunning()
    {
        RefRW<FSComponent> fsComponent = SystemAPI.GetSingletonRW<FSComponent>();

        string apiRoute = "platform/users/current";
        string request = string.Format("{0}{1}", fsComponent.ValueRO.regProduction, apiRoute);

        UnityWebRequest webRequest = UnityWebRequest.Get(request);
        webRequest.SetRequestHeader("Accept", "application/json");
        webRequest.SetRequestHeader("Authorization", "Bearer " + fsComponent.ValueRO.accessToken);

        webRequest.downloadHandler = new DownloadHandlerBuffer();

        UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();
        asyncOperation.completed += (AsyncOperation op) => { ServerResponse(asyncOperation); };
    }

    private void ServerResponse(UnityWebRequestAsyncOperation op)
    {
        RefRW<CurrentUserComponent> currentUserComponent = SystemAPI.GetSingletonRW<CurrentUserComponent>();

        CurrentUserJson currentUser = JsonConvert.DeserializeObject<CurrentUserJson>(op.webRequest.downloadHandler.text);

        currentUserComponent.ValueRW.id = currentUser.users[0].id;
        currentUserComponent.ValueRW.contactName = currentUser.users[0].contactName;
        currentUserComponent.ValueRW.helperPin = currentUser.users[0].helperAccessPin;
        currentUserComponent.ValueRW.givenName = currentUser.users[0].givenName;
        currentUserComponent.ValueRW.familyName = currentUser.users[0].familyName;
        currentUserComponent.ValueRW.email = currentUser.users[0].email;
        currentUserComponent.ValueRW.country = currentUser.users[0].country;
        currentUserComponent.ValueRW.gender = currentUser.users[0].gender;
        currentUserComponent.ValueRW.birthDate = currentUser.users[0].birthDate;
        currentUserComponent.ValueRW.preferredLanguage = currentUser.users[0].preferredLanguage;
        currentUserComponent.ValueRW.displayName = currentUser.users[0].displayName;
        currentUserComponent.ValueRW.personId = currentUser.users[0].personId;
        currentUserComponent.ValueRW.treeUserId = currentUser.users[0].treeUserId;

        World.GetExistingSystemManaged<FSAncestrySystem>().Enabled = true;

        Enabled = false;
    }

    protected override void OnUpdate()
    {
        
    }
}
