using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace WordSearch
{
    public class GameManager : MonoBehaviour
    {
        public List<string> Question = new List<string>();
        public List<TMP_InputField> Answers = new List<TMP_InputField>();

        public void CheckAnswer()
        {
            for (int i = 0; i < Question.Count; i++)
            {
                if (Question[i] == Answers[i].text)
                {
                    print(Answers[i].text + "Correct");
                }
            }
        }

    }
}