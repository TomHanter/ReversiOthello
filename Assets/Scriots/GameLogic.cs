using Assets.Scriots;
using Scriots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    [SerializeField] private Chip chipBlackUp;
    [SerializeField] private Chip chipWhiteUp;
    [SerializeField] private GameObject highlightPrefab;
    [SerializeField] private UIGame uiManager;

    private readonly Dictionary<Player, Chip> chipPrefabs = new();
    private readonly RuleOfGame ruleOfGame = new();
    private readonly Chip[,] chips = new Chip[8, 8];
    private readonly List<GameObject> highlights = new();

    private void Start()
    {
        chipPrefabs[Player.Black] = chipBlackUp;
        chipPrefabs[Player.White] = chipWhiteUp;

        AddStartChips();
        ShowLegalMoves();
        uiManager.SetPlayerText(ruleOfGame.CurrentPlayer);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector3 impact = hitInfo.point;
                Position boardPos = SceneToBoardPos(impact);
                OnBoardCliked(boardPos);
            }
        }
    }

    private void ShowLegalMoves()
    {
        foreach (Position boardPos in ruleOfGame.LegalMoves.Keys)
        {
            Vector3 scenePos = BoardToScenePos(boardPos) + Vector3.up * 0.01f;
            GameObject highlight = Instantiate(highlightPrefab, scenePos, Quaternion.identity);
            highlights.Add(highlight);
        }
    }

    private void HideLegalMoves()
    {
        highlights.ForEach(Destroy);
        highlights.Clear();
    }

    private void OnBoardCliked(Position boardPos)
    {
        if (ruleOfGame.MakeMove(boardPos, out MoveInfo moveInfo))
        {
            StartCoroutine(OnMoveMade(moveInfo));
        }
    }

    private IEnumerator OnMoveMade(MoveInfo moveInfo)
    {
        HideLegalMoves();
        yield return ShowMove(moveInfo);
        yield return ShowTurnOutCome(moveInfo);
        ShowLegalMoves();
    }

    private Position SceneToBoardPos(Vector3 scenePos)
    {
        int column = (int)(scenePos.x - 0.25f);
        int row = 7 - (int)(scenePos.z - 0.25f);
        return new Position(row, column);
    }

    private Vector3 BoardToScenePos(Position boardPos)
    {
        return new Vector3(boardPos.Column + 0.75f, 0, 7 - boardPos.Row + 0.75f);
    }

    private void SpawnChip(Chip prefab, Position boardPos)
    {
        Vector3 scenePos = BoardToScenePos(boardPos) + Vector3.up * 0.1f;
        chips[boardPos.Row, boardPos.Column] = Instantiate(prefab, scenePos, Quaternion.identity);
    }

    private void AddStartChips()
    {
        foreach (Position boardPos in ruleOfGame.OccupiedPosition())
        {
            Player player = ruleOfGame.Board[boardPos.Row, boardPos.Column];
            SpawnChip(chipPrefabs[player], boardPos);
        }
    }

    private void FlipChips(List<Position> positions)
    {
        foreach (Position boardPos in positions)
        {
            chips[boardPos.Row, boardPos.Column].Flip();
        }
    }

    private IEnumerator ShowMove(MoveInfo moveInfo)
    {
        SpawnChip(chipPrefabs[moveInfo.Player], moveInfo.Position);
        yield return new WaitForSeconds(0.33f);
        FlipChips(moveInfo.Outflanked);
        yield return new WaitForSeconds(0.83f);
    }

    private IEnumerator ShowTurnSkipped(Player skippedPlayer)
    {
        uiManager.SetSkippedText(skippedPlayer);
        yield return uiManager;
    }

    private IEnumerator ShowGameOver(Player winner)
    {
        uiManager.SetTopText("Neither Player Can Move");
        yield return uiManager;

        yield return uiManager.ShowScoreText();
        yield return new WaitForSeconds(0.5f);

        yield return ShowCounting();

        uiManager.SetWinnerText(winner);
        yield return uiManager.ShowEndScreen();
    }

    private IEnumerator ShowTurnOutCome(MoveInfo moveInfo)
    {
        if (ruleOfGame.GameOver)
        {
            yield return ShowGameOver(ruleOfGame.Winner);
            yield break;
        }

        Player currentPlayer = ruleOfGame.CurrentPlayer;

        if (currentPlayer == moveInfo.Player)
        {
            yield return ShowTurnSkipped(currentPlayer.Opponent());
        }

        uiManager.SetPlayerText(currentPlayer);
    }

    private IEnumerator ShowCounting()
    {
        int black = 0, white = 0;

        foreach (Position pos in ruleOfGame.OccupiedPosition())
        {
            Player player = ruleOfGame.Board[pos.Row, pos.Column];

            if (player == Player.Black)
            {
                black++;
                uiManager.SetBlackScoreText(black);
            }
            else //if (player == Player.White)
            {
                white++;
                uiManager.SetWhiteScoreText(white);
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator RestartGame()
    {
        yield return uiManager;
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    public void OnPlayAgainClicked()
    {
        StartCoroutine(RestartGame());
    }
}
