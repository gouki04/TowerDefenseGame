using UnityEngine;

public abstract class Tower : GameTileContent
{
    [SerializeField, Range(1.5f, 10.5f)]
    protected float targetingRange = 1.5f;

    public abstract TowerType TowerType { get; }

    protected bool AcquireTarget(out TargetPoint target)
    {
        if (TargetPoint.FillBuffer(transform.localPosition, targetingRange)) {
            target = TargetPoint.RandomBufferer;
            return true;
        }
        target = null;
        return false;
    }

    protected bool TrackTarget(ref TargetPoint target)
    {
        if (target == null) {
            return false;
        }
        var a = transform.localPosition;
        var b = target.Position;
        var x = a.x - b.x;
        var z = a.z - b.z;

        // 下面的0.125是因为这里判断距离优点粗糙，物理系统是根据Enemy上的球型包围盒检测的
        // 为了防止这里判断超出距离，下一帧物理系统又把它检测到，这里加上球型包围盒的半径0.125
        var r = targetingRange + 0.125f * target.Enemy.Scale;
        if (x * x + z * z > r * r) {
            target = null;
            return false;
        }
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var position = transform.localPosition;
        position.y += 0.01f;
        Gizmos.DrawWireSphere(position, targetingRange);
    }
}