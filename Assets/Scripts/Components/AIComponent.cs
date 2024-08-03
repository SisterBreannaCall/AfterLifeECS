using Unity.Entities;
using Unity.Collections;

public struct AIComponent : IComponentData
{
    public FixedString64Bytes workSpace;
    public FixedString64Bytes baseUri;
    public FixedString4096Bytes clientId;
    public FixedString4096Bytes clientIdRuntime;
    public FixedString64Bytes baseUriRuntime;
    public FixedString128Bytes sessionID;
    public FixedString128Bytes characterSessionId;
}
