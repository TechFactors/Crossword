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
        public List<TextMeshProUGUI> Answers = new List<TextMeshProUGUI>();
        void Start()
        {
            foreach (var item in Answers)
            {
                print(item.text);
            }
        }
        public void CheckAnswer()
        {
            foreach (var item in Answers)
            {
                print(item.text);
            }
        }
    }
}