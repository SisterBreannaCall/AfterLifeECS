using Unity.Burst;
using Unity.Entities;
using static System.Net.WebRequestMethods;

[BurstCompile]
public partial struct CreateElevenEntity : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        Entity entity = state.EntityManager.CreateSingleton<ElevenComponent>("Eleven Labs");

        RefRW<ElevenComponent> elevenComponent = SystemAPI.GetSingletonRW<ElevenComponent>();

        elevenComponent.ValueRW.clientId = "";
        elevenComponent.ValueRW.maleVoice1 = "https://api.elevenlabs.io/v1/text-to-speech/TxGEqnHWrfWFTfGW9XjX/stream";
        elevenComponent.ValueRW.maleVoice2 = "https://api.elevenlabs.io/v1/text-to-speech/2EiwWnXFnvU5JabPnv8n/stream";
        elevenComponent.ValueRW.femaleVoice = "https://api.elevenlabs.io/v1/text-to-speech/oWAxZDx7w5VEj9dCyTzz/stream";
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
