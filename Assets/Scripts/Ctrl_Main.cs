using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ctrl_Main : MonoBehaviour
{
    [SerializeField] private Shell[] shells;

    private Dictionary<int, Vector3> pointDictionary = new Dictionary<int, Vector3>();
    private Dictionary<(int, int), List<PathPointToPoint>> pathDictionary = new Dictionary<(int, int), List<PathPointToPoint>>();

    private void Start()
    {
        pointDictionary = shells.ToDictionary(key => key.Id, value => value.transform.position);
        pathDictionary = CreatePathes(pointDictionary);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            foreach (Shell s in shells)
            {
                s.Launch();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShufflePathes(10);
        }
    }
    public void StartGame()
    {

    }

    public void Choose()
    {

    }

    public void EndGame()
    {

    }

    private void ShufflePathes(int shuffleCount)
    {
        Dictionary<int, List<PathPointToPoint>> pathesDictionary = new Dictionary<int, List<PathPointToPoint>>();
        List<int> columns = new List<int>();
        int[,] matrix = new int[shells.Length, shuffleCount + 1];

        // Init
        for (int i = 0; i < shells.Length; i++)
        {
            pathesDictionary.Add(i, new List<PathPointToPoint>());
            columns.Add(shells[i].Id);
            matrix[i, 0] = shells[i].Id;
        }

        // 매 턴마다의 위치 확보
        List<int> lastShuffles = columns;
        int c = 0;
        while (c < shuffleCount)
        {
            List<int> newShuffles = Utils.Shuffle(lastShuffles);
            
            if(!lastShuffles.SequenceEqual(newShuffles))
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    matrix[i, c + 1] = newShuffles[i];
                }

                lastShuffles = newShuffles;
                c++;
            }
        }

        // 경로가 겹치지 않도록 분배
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

                pathesDictionary[shells[j].Id].Add(path);

                usedGroupId.Add(path.groupId);
            }
        }

        // 적용
        for (int i = 0; i < shells.Length; i++)
        {
            shells[i].Reload(pathesDictionary[i]);
        }
    }
    
    private Dictionary<(int, int), List<PathPointToPoint>> CreatePathes(Dictionary<int, Vector3> pointDictionary)
    {
        Dictionary<(int, int), List<PathPointToPoint>> dictionary = new Dictionary<(int, int), List<PathPointToPoint>>();

        int groupId = 0;
        for (int i = 0; i < shells.Length; i++)
        {
            int fromId = shells[i].Id;
            Vector3 from = pointDictionary[fromId];

            List<PathPointToPoint> stops = new List<PathPointToPoint>();
            stops.Add(new PathPointToPoint(groupId++, fromId, fromId, true, from, from, from));

            dictionary.Add((i, i), stops);

            for (int j = i + 1; j < shells.Length; j++)
            {
                int groupId1 = groupId++;
                int groupId2 = groupId++;

                int toId = shells[j].Id;
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
        float dist = diff.magnitude * 0.5f; // 중심에서 한쪽까지 거리

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
}
