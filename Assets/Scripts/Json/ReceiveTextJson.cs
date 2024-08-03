using System.Collections.Generic;

namespace ReceiveTextResource
{
    public class CustomEvent
    {
        public string customEvent;
        public List<object> parameters;
    }

    public class Emotion
    {
        public string behavior;
        public string strength;
    }

    public class Parameters
    {
    }

    public class RelationshipUpdate
    {
        public int trust;
        public int respect;
        public int familiar;
        public int flirtatious;
        public int attraction;
    }

    public class ReceiveTextJson
    {
        public string name;
        public List<string> textList;
        public CustomEvent customEvent;
        public Parameters parameters;
        public Emotion emotion;
        public string sessionId;
        public RelationshipUpdate relationshipUpdate;
        public List<object> activeTriggers;
    }
}
