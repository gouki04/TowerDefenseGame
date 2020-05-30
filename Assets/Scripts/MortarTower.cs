using UnityEngine;

public class MortarTower : Tower
{
    [SerializeField, Range(0.5f, 2f)]
    private float shotsPerSecond = 1f;

    [SerializeField]
    private Transform mortar = default;

    public override TowerType TowerType => TowerType.Mortar;

    private float launchSpeed;

    private float launchProgress;

    [SerializeField, Range(0.5f, 3f)]
    private float shellBlastRadius = 1f;

    [SerializeField, Range(1f, 100f)]
    private float shellDamage = 10f;

    private void Awake()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        var x = targetingRange + 0.25001f;
        var y = -mortar.position.y;
        launchSpeed = Mathf.Sqrt(9.81f * (y + Mathf.Sqrt(x * x + y * y)));
    }

    public override void GameUpdate()
    {
        launchProgress += shotsPerSecond * Time.deltaTime;
        while (launchProgress >= 1f) {
            if (AcquireTarget(out var target)) {
                Launch(target);
                launchProgress -= 1f;
            }
            else {
                launchProgress = 0.999f;
            }
        }
    }

    public void Launch(TargetPoint target)
    {
        var launchPoint = mortar.position;
        var targetPoint = target.Position;
        targetPoint.y = 0f;

        Vector2 dir;
        dir.x = targetPoint.x - launchPoint.x;
        dir.y = targetPoint.z - launchPoint.z;

        var x = dir.magnitude;
        var y = -launchPoint.y;
        dir /= x;

        // gravity
        var g = 9.81f;
        // speed
        var s = launchSpeed;
        var s2 = s * s;

        var r = s2 * s2 - g * (g * x * x + 2f * y * s2);
        Debug.Assert(r >= 0f, "Launch velocity insufficient for range!", this);
        var tanTheta = (s2 + Mathf.Sqrt(r)) / (g * x);
        var cosTheta = Mathf.Cos(Mathf.Atan(tanTheta));
        var sinTheta = cosTheta * tanTheta;

        // 这里没搞懂为啥这么算。。
        mortar.localRotation = Quaternion.LookRotation(new Vector3(dir.x, tanTheta, dir.y));

        Game.SpawnShell().Initialize(
            launchPoint, targetPoint, 
            new Vector3(s * cosTheta * dir.x, s * sinTheta, s * cosTheta * dir.y),
            shellBlastRadius, shellDamage
        );

        //Vector3 prev = launchPoint, next;
        //for (var i = 1; i <= 10; ++i) {
        //    var t = i / 10f;
        //    var dx = s * cosTheta * t;
        //    var dy = s * sinTheta * t - 0.5f * g * t * t;
        //    next = launchPoint + new Vector3(dir.x * dx, dy, dir.y * dx);
        //    Debug.DrawLine(prev, next, Color.blue, 1f);
        //    prev = next;
        //}

        //Debug.DrawLine(launchPoint, targetPoint, Color.yellow, 1f);
        //Debug.DrawLine(
        //    new Vector3(launchPoint.x, 0.01f, launchPoint.z),
        //    new Vector3(launchPoint.x + dir.x * x, 0.01f, launchPoint.z + dir.y * x),
        //    Color.white, 1f
        //);
    }
}
