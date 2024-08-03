using Unity.Entities;
using Unity.Collections;

public struct FSComponent : IComponentData
{
    public FixedString64Bytes clientID;
    public FixedString4096Bytes idToken;
    public FixedString64Bytes accessToken;
    public FixedString64Bytes outState;
    public FixedString128Bytes authCode;
    public FixedString128Bytes authProduction;
    public FixedString128Bytes tokenProduction;
    public FixedString128Bytes regProduction;
    public FixedString128Bytes codeVerifier;
}
