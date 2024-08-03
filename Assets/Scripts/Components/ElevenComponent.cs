using Unity.Entities;
using Unity.Collections;

public struct ElevenComponent : IComponentData
{
    public FixedString128Bytes clientId;
    public FixedString512Bytes maleVoice1;
    public FixedString512Bytes maleVoice2;
    public FixedString512Bytes femaleVoice;
}
