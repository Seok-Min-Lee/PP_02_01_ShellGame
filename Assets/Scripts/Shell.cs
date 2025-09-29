using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public enum State
    {
        None,
        Shuffle,
        Show
    }
    [SerializeField] private MeshRenderer shellMesh;
    [SerializeField] [Range(1, 10)] private float speed;

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

    public int id { get; private set; }
    public bool isRight { get; private set; }
    public State state { get; private set; }
    public ShellMixer mixer { get; private set; }
    public PathPointToPoint? lastPath { get; private set; } = null;

    private Vector3 startPos, targetPos, extraPos;
    private float timer = 0f;

    private void Update()
    {
        if (state == State.Shuffle)
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
                    state = State.None;
                    mixer.ShuffleEnd();
                    return;
                }
            }

            timer += Time.deltaTime * speed;
            // bezier curve
            Vector3 a = Vector3.Lerp(startPos, extraPos, timer);
            Vector3 b = Vector3.Lerp(extraPos, targetPos, timer);

            transform.position = Vector3.Lerp(a, b, timer);
        }
    }
    public void Init(int id, bool isRight, ShellMixer mixer)
    {
        this.id = id;
        this.isRight = isRight;
        this.mixer = mixer;

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

            state = State.Shuffle;
        }
    }

    public void Show(Action<bool> callback = null)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOMoveY(1, 1f / speed));
        seq.Append(transform.DOMoveY(0, 1f / speed));
        seq.AppendCallback(() => callback?.Invoke(isRight));
    }
}
