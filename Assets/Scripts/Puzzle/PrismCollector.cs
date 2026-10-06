using UnityEngine;

public class PrismCollector : MonoBehaviour
{
    public void CollectPrism()
    {
        // do some fancy animation here later!

        SpectrumManager.Instance.UnlockNextColor();

        Destroy(gameObject);
    }
}
