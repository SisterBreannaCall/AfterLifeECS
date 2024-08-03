using Unity.Entities;
using Unity.Collections;

public struct AncestorComponent : IComponentData
{
    public FixedString64Bytes name;
    public FixedString64Bytes pid;
    public FixedString64Bytes sex;
    public FixedString64Bytes lifeSpan;
    public FixedString64Bytes birthDate;
    public FixedString128Bytes birthLocation;
    public FixedString64Bytes deathDate;
    public FixedString128Bytes deathLocation;
    public bool isSelected;
}
