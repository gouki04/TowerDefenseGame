﻿using UnityEngine;

[CreateAssetMenu]
public class GameScenario : ScriptableObject
{
    [SerializeField]
    private EnemyWave[] waves = { };

    [SerializeField, Range(0, 10)]
    private int cycles = 1;

    [SerializeField, Range(0f, 1f)]
    private float cycleSpeedUp = 0.5f;

    public State Begin() => new State(this);

    [System.Serializable]
    public struct State
    {
        private GameScenario scenario;
        private int cycle, index;
        private float timeScale;
        private EnemyWave.State wave;

        public State(GameScenario scenario)
        {
            this.scenario = scenario;
            cycle = 0;
            index = 0;
            timeScale = 1f;
            Debug.Assert(scenario.waves.Length > 0, "Empty scenario!");
            wave = scenario.waves[0].Begin();
        }

        public bool Progress()
        {
            var deltaTime = wave.Progress(timeScale * Time.deltaTime);
            while (deltaTime >= 0f) {
                if (++index >= scenario.waves.Length) {
                    if (++cycle >= scenario.cycles && scenario.cycles > 0) {
                        return false;
                    }
                    index = 0;
                    timeScale += scenario.cycleSpeedUp;
                }
                wave = scenario.waves[index].Begin();
                deltaTime = wave.Progress(deltaTime);
            }
            return true;
        }
    }
}