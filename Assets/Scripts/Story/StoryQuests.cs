using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "StoryQuests", menuName = "Venturer/Story/StoryQuests", order = 1)]
public class StoryQuests : ScriptableObject
{
    public List<Quest> acceptedQuests;
    public List<Quest> failedQuests;
    public List<Quest> finishedQuests;

    public CharacterStatisticsController playerStats;

    [Serializable]
    public class Quest
    {
        public string name;
        public string decription;
        public string reward;
        public int optimalLevel;
        public bool main;
    }
}