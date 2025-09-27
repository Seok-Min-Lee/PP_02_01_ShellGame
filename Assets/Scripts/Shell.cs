using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public int Id => id;
    [SerializeField] private int id;

    private Queue<PathPointToPoint> pathQueue = new Queue<PathPointToPoint>();

    private Vector3 currentPos;
    private Vector3 targetPos;
    private Vector3 extraPos;
    private int moveCount = 0;
    private bool isMove = false;
    private float timer = 0f;
    private void Start()
    {
        
    }
    private void Update()
    {
        if (isMove)
        {
            if (timer > 1f)
            {
                timer = 0f;

                isMove = false;
                return;
            }

            timer += Time.deltaTime;
            // bezier curve
            Vector3 a = Vector3.Lerp(currentPos, extraPos, timer);
            Vector3 b = Vector3.Lerp(extraPos, targetPos, timer);

            transform.position = Vector3.Lerp(a, b, timer);
        }
    }
    public void Reload(IEnumerable<PathPointToPoint> pathes)
    {
        foreach (PathPointToPoint path in pathes)
        {
            pathQueue.Enqueue(path);
        }
    }
    public void Launch()
    {
        if (pathQueue.Count > 0)
        {
            PathPointToPoint path = pathQueue.Dequeue();

            currentPos = path.from;
            targetPos = path.to;
            extraPos = path.extra;

            moveCount++;
            isMove = true;
        }
    }

    public void Open()
    {

    }

    public void Close()
    {

    }

    private void Stop()
    {
        
    }
}
