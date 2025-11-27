using System;
using System.Collections.Generic;

namespace Code.Scripts.Checkpoint
{
    [Serializable]
    public class SerializableBoolDictionary
    {
       
        [Serializable]
        public struct Entry
        {
            public string Key;
            public bool Value;
        }
        
        public List<Entry> entries = new List<Entry>();
        
        public Dictionary<string, bool> ToDictionary()
        {
            var dict = new Dictionary<string, bool>();
            foreach (var entry in entries)
            {
                dict[entry.Key] = entry.Value;
            }
            return dict;
        }
        
        public void FromDictionary(Dictionary<string, bool> dict)
        {
            entries.Clear();
            foreach (var kvp in dict)
            {
                entries.Add(new Entry { Key = kvp.Key, Value = kvp.Value });
            }
        }
        
    }
}