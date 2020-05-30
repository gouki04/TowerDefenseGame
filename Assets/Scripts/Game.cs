using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    private Vector2Int boardSize = new Vector2Int(11, 11);

    [SerializeField]
    private GameBoard board = default;

    [SerializeField]
    private GameTileContentFactory tileContentFactory = default;

    [SerializeField]
    private WarFactory warFactory = default;

    private Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);

    private GameBehaviorCollection enemies = new GameBehaviorCollection();
    private GameBehaviorCollection nonEnemies = new GameBehaviorCollection();

    private TowerType selectedTowerType;

    [SerializeField]
    private GameScenario scenario = default;

    private GameScenario.State activeScenario;

    [SerializeField, Range(0, 100)]
    private int startingPlayerHealth = 10;

    private int playerHealth;

    private const float pausedTimeScale = 0.01f;

    [SerializeField, Range(1f, 10f)]
    private float playSpeed = 1f;

    private static Game instance;

    private void OnEnable()
    {
        instance = this;
    }

    private void Awake()
    {
        playerHealth = startingPlayerHealth;
        board.Initialize(boardSize, tileContentFactory);
        board.ShowGrid = true;
        activeScenario = scenario.Begin();
    }

    private void OnValidate()
    {
        if (boardSize.x < 2) {
            boardSize.x = 2;
        }
        if (boardSize.y < 2) {
            boardSize.y = 2;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            HandleTouch();
        }
        else if (Input.GetMouseButtonDown(1)) {
            HandleAlternaiveTouch();
        }

        if (Input.GetKeyDown(KeyCode.V)) {
            board.ShowPaths = !board.ShowPaths;
        }
        if (Input.GetKeyDown(KeyCode.G)) {
            board.ShowGrid = !board.ShowGrid;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            selectedTowerType = TowerType.Laser;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            selectedTowerType = TowerType.Mortar;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            Time.timeScale =
                Time.timeScale > pausedTimeScale ? pausedTimeScale : playSpeed;
        }
        else if (Time.timeScale > pausedTimeScale) {
            Time.timeScale = playSpeed;
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            BeginNewGame();
        }

        if (playerHealth <= 0 && startingPlayerHealth > 0) {
            Debug.Log("Defeat!");
            BeginNewGame();
        }

        if (!activeScenario.Progress() && enemies.IsEmpty) {
            Debug.Log("Victory!");
            BeginNewGame();
            activeScenario.Progress();
        }

        enemies.GameUpdate();
        // 这里是为了解决Enemy是在场景中心创建然后再移动出生点的，但物理引擎有可能会在塔的检测里查询到这个Enemy
        // 这里在更新前强制更新下物理引擎里的Transform
        Physics.SyncTransforms();
        board.GameUpdate();
        nonEnemies.GameUpdate();
    }

    private void BeginNewGame()
    {
        playerHealth = startingPlayerHealth;
        enemies.Clear();
        nonEnemies.Clear();
        board.Clear();
        activeScenario = scenario.Begin();
    }

    public static void EnemyReachedDestination()
    {
        instance.playerHealth -= 1;
    }

    private void HandleTouch()
    {
        var tile = board.GetTile(TouchRay);
        if (tile != null) {
            if (Input.GetKey(KeyCode.LeftShift)) {
                board.ToggleTower(tile, selectedTowerType);
            }
            else {
                board.ToggleWall(tile);
            }
        }
    }

    private void HandleAlternaiveTouch()
    {
        var tile = board.GetTile(TouchRay);
        if (tile != null) {
            if (Input.GetKey(KeyCode.LeftShift)) {
                board.ToggleDestination(tile);
            }
            else {
                board.ToggleSpawnPoint(tile);
            }
        }
    }

    public static void SpawnEnemy(EnemyFactory factory, EnemyType type)
    {
        var spawnPoint = instance.board.GetSpawnPoint(
            Random.Range(0, instance.board.SpawnPointCount)
        );
        var enemy = factory.Get((EnemyType)(type));
        enemy.SpawnOn(spawnPoint);

        instance.enemies.Add(enemy);
    }

    public static Shell SpawnShell()
    {
        var shell = instance.warFactory.Shell;
        instance.nonEnemies.Add(shell);
        return shell;
    }

    public static Explosion SpawnExplosion()
    {
        var explosion = instance.warFactory.Explosion;
        instance.nonEnemies.Add(explosion);
        return explosion;
    }
}
