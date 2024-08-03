using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct CreateFSEntity : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        Entity entity = state.EntityManager.CreateSingleton<FSComponent>("FSAuth");
        state.EntityManager.AddComponent<IdentityComponent>(entity);
        state.EntityManager.AddComponent<CurrentUserComponent>(entity);

        RefRW<FSComponent> fsComponent = SystemAPI.GetSingletonRW<FSComponent>();

        fsComponent.ValueRW.clientID = "";
        fsComponent.ValueRW.authProduction = "https://ident.familysearch.org/cis-web/oauth2/v3/authorization";
        fsComponent.ValueRW.tokenProduction = "https://ident.familysearch.org/cis-web/oauth2/v3/token";
        fsComponent.ValueRW.regProduction = "https://api.familysearch.org/";
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
