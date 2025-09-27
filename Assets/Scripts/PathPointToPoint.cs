using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PathPointToPoint
{
    public PathPointToPoint(
        int groupId,
        int fromId,
        int toId,
        bool direction,
        Vector3 from,
        Vector3 to,
        Vector3 extra
    )
    {
        this.groupId = groupId;
        this.fromId = fromId;
        this.toId = toId;
        this.direction = direction;
        this.from = from;
        this.to = to;
        this.extra = extra;
    }
    public int groupId { get; private set; }
    public int fromId { get; private set; }
    public int toId { get; private set; }
    public bool direction { get; private set; }
    public Vector3 from { get; private set; }
    public Vector3 to { get; private set; }
    public Vector3 extra { get; private set; }
    public override string ToString()
    {
        return $"Path(groupId={groupId}, fromId={fromId}, toId={toId}, direction={direction}, from={from}, to={to}, extra={extra})";
    }
}
