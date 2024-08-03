using Unity.Entities;
using Unity.Collections;

public struct IdentityComponent : IComponentData
{
    public FixedString64Bytes sub;
    public FixedString64Bytes country;
    public FixedString64Bytes gender;
    public FixedString128Bytes iss;
    public FixedString64Bytes language;
    public FixedString64Bytes sessionId;
    public FixedString64Bytes givenName;
    public FixedString64Bytes locale;
    public FixedString64Bytes aud;
    public FixedString64Bytes affiliateAccount;
    public FixedString64Bytes exp;
    public FixedString64Bytes iat;
    public FixedString64Bytes familyName;
    public FixedString64Bytes email;
}
