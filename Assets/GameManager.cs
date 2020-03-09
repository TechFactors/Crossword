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
            var Count = Question.Count;
            var Correct = 0;
            for (int i = 0; i < Question.Count; i++)
            {
                if (Question[i] == Answers[i].text)
                {
                    Correct += 1;
                }
                if (Correct == Count) print(Answers[i].text + "Correct");
            }
        }

    }
}