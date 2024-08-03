using Unity.Entities;
using Unity.Collections;

public struct CurrentUserComponent : IComponentData
{
    public FixedString64Bytes id;
    public FixedString64Bytes contactName;
    public FixedString64Bytes helperPin;
    public FixedString64Bytes givenName;
    public FixedString64Bytes familyName;
    public FixedString64Bytes email;
    public FixedString64Bytes country;
    public FixedString64Bytes gender;
    public FixedString64Bytes birthDate;
    public FixedString64Bytes preferredLanguage;
    public FixedString64Bytes displayName;
    public FixedString64Bytes personId;
    public FixedString64Bytes treeUserId;
}
