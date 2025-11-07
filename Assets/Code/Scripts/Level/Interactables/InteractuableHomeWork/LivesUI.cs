using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartContainer;
    private List<Image> hearts = new List<Image>();
    //private int lives;
    public void InitLives(int lives)
    {
        //lives = initialLives;
        foreach (Transform child in heartContainer) Destroy(child.gameObject);
        hearts.Clear();

        for (int i = 0; i < lives; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer);
            hearts.Add(heart.GetComponent<Image>());

        }
    }

    /*public void LoseLife()
    {
        lives = Mathf.Max(0, lives - 1);
        UpdateLivesUI();
    }
    */
    public void UpdateLivesUI(int currentLives)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i >= currentLives)
                hearts[i].color = Color.gray;
            }
        }
}
