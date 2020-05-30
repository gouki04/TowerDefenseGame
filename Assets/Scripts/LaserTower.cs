﻿using UnityEngine;

public class LaserTower : Tower
{
    [SerializeField, Range(1f, 100f)]
    private float damagePerSecond = 10f;

    private TargetPoint target;

    [SerializeField]
    private Transform turret, laserBeam = default;

    private Vector3 laserBeamScale;

    public override TowerType TowerType => TowerType.Laser;

    private void Awake()
    {
        laserBeamScale = laserBeam.localScale;
    }

    public override void GameUpdate()
    {
        if (TrackTarget(ref target) || AcquireTarget(out target)) {
            Shoot();
        }
        else {
            laserBeam.localScale = Vector3.zero;
        }
    }

    private void Shoot()
    {
        var point = target.Position;
        turret.LookAt(point);
        laserBeam.localRotation = turret.localRotation;

        var d = Vector3.Distance(turret.position, point);
        laserBeamScale.z = d;
        laserBeam.localScale = laserBeamScale;
        laserBeam.localPosition = turret.localPosition + 0.5f * d * laserBeam.forward;

        target.Enemy.ApplyDamage(damagePerSecond * Time.deltaTime);
    }
}