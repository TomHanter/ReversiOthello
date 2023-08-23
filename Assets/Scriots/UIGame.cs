using System.Collections;
using UnityEngine;
using TMPro;

public class UIGame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI topText;
    [SerializeField] private TextMeshProUGUI blackScoreText;
    [SerializeField] private TextMeshProUGUI whiteScoreText;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private RectTransform playAgainButton;

    public void SetPlayerText(Player currentPlayer)
    {
        //switch case 
        topText.text = currentPlayer switch
        {
            Player.Black => "Black is Turn  <sprite name=ChipBlackUp>",
            Player.White => "White is Turn  <sprite name=ChipWhiteUp>",
            _ => topText.text
        };
    }

    public void SetSkippedText(Player skippedPlayer)
    {
        if (skippedPlayer == Player.Black)
        {
            topText.text = "Black CanMove  <sprite name=ChipBlackUp>";
        }
        else if (skippedPlayer == Player.White)
        {
            topText.text = "White CanMove  <sprite name=ChipWhiteUp>";
        }
    }

    public void SetTopText(string message)
    {
        topText.text = message;
    }

    private IEnumerator ScaleDown(RectTransform rect)
    {
        rect.LeanScale(Vector3.zero, 0.2f);
        yield return new WaitForSeconds(0.2f);//хардкод
        rect.gameObject.SetActive(false);
    }

    private IEnumerator ScaleUp(RectTransform rect)
    {
        rect.gameObject.SetActive(true);
        rect.localScale = Vector3.zero;
        rect.LeanScale(Vector3.one, 0.2f);
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator ShowScoreText()
    {
        yield return ScaleDown(topText.rectTransform);
        yield return ScaleUp(blackScoreText.rectTransform);
        yield return ScaleUp(whiteScoreText.rectTransform);
    }

    public void SetBlackScoreText(int score)
    {
        blackScoreText.text = $"<sprite name=ChipBlackUp> {score}";
    }

    public void SetWhiteScoreText(int score)
    {
        whiteScoreText.text = $"<sprite name=ChipWhiteUp> {score}";
    }

    public void SetWinnerText(Player winner)
    {
        winnerText.text = winner switch
        {
            Player.Black => "Black Won!",
            Player.White => "White Won!",
            Player.None => "It is a Tie!",
            _ => winnerText.text
        };
    }

    public IEnumerator ShowEndScreen()
    {
        yield return ScaleUp(winnerText.rectTransform);
        yield return ScaleUp(playAgainButton);
    }


}
