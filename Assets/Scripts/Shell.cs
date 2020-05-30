using UnityEngine;

public class Shell : WarEntity
{
    private Vector3 launchPoint, targetPoint, launchVelocity;
    private float age, blastRadius, damage;

    public void Initialize(
        Vector3 launchPoint, Vector3 targetPoint, Vector3 launchVelocity,
        float blastRadius, float damage
    ) {
        this.launchPoint = launchPoint;
        this.targetPoint = targetPoint;
        this.launchVelocity = launchVelocity;
        this.blastRadius = blastRadius;
        this.damage = damage;
    }

    public override bool GameUpdate()
    {
        age += Time.deltaTime;
        var p = launchPoint + launchVelocity * age;
        p.y -= 0.5f * 9.81f * age * age;

        if (p.y <= 0f) {
            Game.SpawnExplosion().Initialize(targetPoint, blastRadius, damage);
            OriginFactory.Reclaim(this);
            return false;
        }

        transform.localPosition = p;

        var d = launchVelocity;
        d.y -= 9.81f * age;
        transform.localRotation = Quaternion.LookRotation(d);

        // TODO 暂停或者慢镜头时，这里也要限制速度
        Game.SpawnExplosion().Initialize(p, 0.1f);
        return true;
    }
}
