using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Transform doorTransform;
    [SerializeField] float doorSpeed = 10;
    [SerializeField] Transform openTransform;

    Vector3 closedPosition;
    Vector3 openPosition;
    Coroutine moveRoutine;

    private bool isOpen = false;

    private void Awake()
    {
        closedPosition = doorTransform.position;
        openPosition = openTransform.position;
    }

    public void Open()
    {
        isOpen = true;
        MoveTo(openPosition);
    }

    public void Close()
    {
        isOpen = false;
        MoveTo(closedPosition);
    }

    private void MoveTo(Vector3 target)
    {
        // stop any in-progress movement
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveRoutine(target));
    }

    private IEnumerator MoveRoutine(Vector3 target)
    {
        while (doorTransform.position != target)
        {
            doorTransform.position = Vector3.MoveTowards(
                doorTransform.position, target, doorSpeed * Time.deltaTime);
            yield return null;
        }

        moveRoutine = null;
    }
}