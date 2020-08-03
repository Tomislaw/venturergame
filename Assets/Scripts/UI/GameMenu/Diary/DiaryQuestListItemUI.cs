using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiaryQuestListItemUI : MonoBehaviour
{
    public Image image;
    public TMPro.TMP_Text text;

    [SerializeField]
    private StoryQuests.Quest quest;

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void SetQuest(StoryQuests.Quest quest)
    {
        this.quest = quest;
        if (this.quest == null)
        {
            text.text = "";
            image.color = Color.white;
        }
        else
        {
            text.text = quest.name;
        }
    }
}