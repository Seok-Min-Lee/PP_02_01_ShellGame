using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField] private MeshRenderer shellMesh;
    public int Id => id;
    private int id;

    private Color[] colors = new Color[3] 
    { 
        Color.red, 
        Color.green, 
        Color.blue 
    };
    private Vector3[] positions = new Vector3[3] 
    { 
        new Vector3(-3, 0, 0),
        new Vector3(0, 0, -3), 
        new Vector3(3, 0, 0) 
    };

    private Queue<PathPointToPoint> pathQueue = new Queue<PathPointToPoint>();

    public PathPointToPoint? lastPath { get; private set; } = null;
    private Vector3 startPos, targetPos, extraPos;

    private bool isMove = false;
    private float timer = 0f;

    private void Update()
    {
        if (isMove)
        {
            if (timer > 1f)
            {
                timer = 0f;

                if (pathQueue.Count > 0)
                {
                    lastPath = pathQueue.Dequeue();

                    startPos = lastPath.Value.from;
                    targetPos = lastPath.Value.to;
                    extraPos = lastPath.Value.extra;
                }
                else
                {
                    isMove = false;
                    return;
                }
            }

            timer += Time.deltaTime;
            // bezier curve
            Vector3 a = Vector3.Lerp(startPos, extraPos, timer);
            Vector3 b = Vector3.Lerp(extraPos, targetPos, timer);

            transform.position = Vector3.Lerp(a, b, timer);
        }
    }
    public void Init(int id)
    {
        this.id = id;
        shellMesh.material.color = colors[id];
        transform.position = positions[id];
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
            lastPath = pathQueue.Dequeue();

            startPos = lastPath.Value.from;
            targetPos = lastPath.Value.to;
            extraPos = lastPath.Value.extra;

            isMove = true;
        }
    }

    public void Select()
    {
        Debug.Log($"{name} Select!");
    }
}
