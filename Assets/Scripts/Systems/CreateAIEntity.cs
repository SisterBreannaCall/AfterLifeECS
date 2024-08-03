using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct CreateAIEntity : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        Entity entity = state.EntityManager.CreateSingleton<AIComponent>("AI Chat");

        RefRW<AIComponent> aiComponent = SystemAPI.GetSingletonRW<AIComponent>();

        aiComponent.ValueRW.baseUri = "https://api.inworld.ai/studio/v1";
        aiComponent.ValueRW.baseUriRuntime = "https://api.inworld.ai/v1";
        aiComponent.ValueRW.workSpace = "";
        aiComponent.ValueRW.clientId = "";
        aiComponent.ValueRW.clientIdRuntime = "";
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
