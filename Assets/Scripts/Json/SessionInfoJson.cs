using System.Collections.Generic;

namespace SessionInfoResource
{
    public class CharacterAssets
    {
        public string avatarImg;
        public string avatarImgOriginal;
    }

    public class SessionInfoJson
    {
        public string name;
        public List<SessionCharacter> sessionCharacters;
        public string loadedScene;
    }

    public class SessionCharacter
    {
        public string name;
        public string character;
        public string displayName;
        public CharacterAssets characterAssets;
    }
}
