using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShellMixer : MonoBehaviour
{
    public Shell[] Shells => shells;

    [SerializeField] private Ctrl_Main ctrl;
    [SerializeField] private Shell[] shells;

    [SerializeField] private int shuffleCount;

    private Dictionary<int, Vector3> pointDictionary = new Dictionary<int, Vector3>();
    private Dictionary<(int, int), List<PathPointToPoint>> pathDictionary = new Dictionary<(int, int), List<PathPointToPoint>>();

    private int shellCount;
    public void Init(int shellCount, int speed, int shuffleCount)
    {
        this.shellCount = shellCount;
        this.shuffleCount = shuffleCount;

        Vector3[] points = GetCirclePoints(count: shellCount, radius: 3);

        pointDictionary = new Dictionary<int, Vector3>();
        int rightIndex = UnityEngine.Random.Range(0, shellCount);
        for (int i = 0; i < shells.Length; i++)
        {
            if (i < shellCount)
            {
                shells[i].Init(
                    id: i,
                    speed: speed,
                    isRight: i == rightIndex,
                    position: points[i],
                    mixer: this
                );

                pointDictionary.Add(i, shells[i].transform.position);
            }
            else
            {
                shells[i].gameObject.SetActive(false);
            }
        }

        pathDictionary = CreatePathes(pointDictionary);
    }
    public void Preview()
    {
        for (int i = 0; i < shellCount; i++)
        {
            if (shells[i].isRight)
            {
                shells[i].Show((bool b) =>
                {
                    ShuffleCalculate(shuffleCount);
                    ShuffleStart();
                });
            }
        }
    }
    public void ShuffleCalculate(int shuffleCount)
    {
        Dictionary<int, List<PathPointToPoint>> pathesDictionary = new Dictionary<int, List<PathPointToPoint>>();
        List<int> columns = new List<int>();
        int[,] matrix = new int[shellCount, shuffleCount + 1];

        // Init
        for (int i = 0; i < shellCount; i++)
        {
            pathesDictionary.Add(i, new List<PathPointToPoint>());
            columns.Add(shells[i].id);
            matrix[i, 0] = shells[i].lastPath.HasValue ? shells[i].lastPath.Value.toId : i;
        }

        // �� �ϸ����� ��ġ Ȯ��
        List<int> lastShuffles = columns;
        int c = 0;
        while (c < shuffleCount)
        {
            List<int> newShuffles = Utils.Shuffle(lastShuffles);

            if (!lastShuffles.SequenceEqual(newShuffles))
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    matrix[i, c + 1] = newShuffles[i];
                }

                lastShuffles = newShuffles;
                c++;
            }
        }

        // ��ΰ� ��ġ�� �ʵ��� �й�
        HashSet<int> usedGroupId = new HashSet<int>();
        for (int i = 0; i < shuffleCount; i++)
        {
            usedGroupId.Clear();

            for (int j = 0; j < columns.Count; j++)
            {
                int fromId = matrix[j, i];
                int toId = matrix[j, i + 1];

                List<PathPointToPoint> pathes = pathDictionary[(fromId, toId)].Where(path => !usedGroupId.Contains(path.groupId)).ToList();

                PathPointToPoint path = pathes[UnityEngine.Random.Range(0, pathes.Count)];

                pathesDictionary[shells[j].id].Add(path);

                usedGroupId.Add(path.groupId);
            }
        }
        
        // ����
        for (int i = 0; i < shellCount; i++)
        {
            shells[i].Reload(pathesDictionary[i]);
        }
    }
    public void ShuffleStart()
    {
        foreach (Shell s in shells)
        {
            s.Launch();
        }
    }
    public void ShuffleEnd()
    {
        for (int i = 0; i < shellCount; i++)
        {
            if (shells[i].state == Shell.State.Shuffle)
            {
                return;
            }
        }

        ctrl.Checkpoint();
    }
    private Dictionary<(int, int), List<PathPointToPoint>> CreatePathes(Dictionary<int, Vector3> pointDictionary)
    {
        Dictionary<(int, int), List<PathPointToPoint>> dictionary = new Dictionary<(int, int), List<PathPointToPoint>>();

        int groupId = 0;
        for (int i = 0; i < shellCount; i++)
        {
            int fromId = shells[i].id;
            Vector3 from = pointDictionary[fromId];

            List<PathPointToPoint> stops = new List<PathPointToPoint>();
            stops.Add(new PathPointToPoint(groupId++, fromId, fromId, true, from, from, from));

            dictionary.Add((i, i), stops);

            for (int j = i + 1; j < shellCount; j++)
            {
                int groupId1 = groupId++;
                int groupId2 = groupId++;

                int toId = shells[j].id;
                Vector3 to = pointDictionary[toId];

                //
                List<Vector3> extras = GetExtraPoints(from, to);

                PathPointToPoint path1 = new PathPointToPoint(groupId1, fromId, toId, true, from, to, extras[0]);
                PathPointToPoint reverse1 = new PathPointToPoint(groupId1, toId, fromId, true, to, from, extras[0]);

                PathPointToPoint path2 = new PathPointToPoint(groupId2, fromId, toId, false, from, to, extras[1]);
                PathPointToPoint reverse2 = new PathPointToPoint(groupId2, toId, fromId, false, to, from, extras[1]);

                List<PathPointToPoint> pathes = new List<PathPointToPoint>();
                pathes.Add(path1);
                pathes.Add(path2);

                dictionary.Add((i, j), pathes);

                //
                List<PathPointToPoint> reverses = new List<PathPointToPoint>();
                reverses.Add(reverse1);
                reverses.Add(reverse2);

                dictionary.Add((j, i), reverses);
            }
        }

        return dictionary;
    }
    private List<Vector3> GetExtraPoints(Vector3 p1, Vector3 p2)
    {
        Vector3 center = (p1 + p2) * 0.5f;

        Vector3 diff = new Vector3(p2.x - p1.x, 0, p2.z - p1.z);
        float dist = diff.magnitude * 0.5f; // �߽ɿ��� ���ʱ��� �Ÿ�

        Vector3 dir;
        if (Mathf.Approximately(diff.x, 0))
        {
            dir = Vector3.right;
        }
        else if (Mathf.Approximately(diff.z, 0))
        {
            dir = Vector3.forward;
        }
        else
        {
            dir = new Vector3(-diff.z, 0, diff.x).normalized;
        }

        List<Vector3> points = new List<Vector3>();
        points.Add(center + dir * dist);
        points.Add(center - dir * dist);

        return points;
    }
    private Vector3[] GetCirclePoints(int count, int radius)
    {
        Vector3[] points = new Vector3[count];

        Vector3 margin = new Vector3(0, 0, 1.5f);

        for (int i = 0; i < count; i++)
        {
            // �� ���� ���� (���� ����)
            float angle = i * Mathf.PI * 2f / count;

            // �� �� ��ǥ ���
            points[i] = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * radius - margin;
        }

        return points;
    }
}
