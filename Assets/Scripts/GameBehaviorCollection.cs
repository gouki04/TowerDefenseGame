using System.Collections.Generic;

[System.Serializable]
public class GameBehaviorCollection
{
    private List<GameBehavior> behaviors = new List<GameBehavior>();

    public void Add(GameBehavior enemy)
    {
        behaviors.Add(enemy);
    }

    public void GameUpdate()
    {
        for (var i = 0; i < behaviors.Count; ++i) {
            if (!behaviors[i].GameUpdate()) {
                var lastIndex = behaviors.Count - 1;
                behaviors[i] = behaviors[lastIndex];
                behaviors.RemoveAt(lastIndex);
                i -= 1;
            }
        }
    }
}
