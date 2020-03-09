using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordSearch
{
    public class Tile : MonoBehaviour
    {
        public int Column { set; get; }
        public int Row { set; get; }
        public char Letter { set; get; }
        public bool HasLetter { set; get; }
        public bool Searched { set; get; }
        public GameObject tileBG;

        char[] chars = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        Vector2 tilePosition;

        public void SetTilePosition(int column, int row)
        {
            var size = tileBG.GetComponent<SpriteRenderer>().bounds.size.x;

            tilePosition = new Vector3((column * size), (-row * size));
            transform.position = tilePosition;
        }

        public void SetTileData(char c)
        {
            var charsToList = new List<char>(chars);
            int index = charsToList.IndexOf(c);

            if (index < 0) return;

            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(index + 1).gameObject.SetActive(true);
            Letter = c;
            HasLetter = true;
        }

        public void ResetTileData()
        {
            var charsToList = new List<char>(chars);
            int index = charsToList.IndexOf(Letter);
            transform.GetChild(index + 1).gameObject.SetActive(false);
            HasLetter = false;
        }
    }
}