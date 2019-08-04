using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node {

    public float X { get; set; }
    public float Y { get; set; }
    

    public float fScore { get; set; }
    public float gScore { get; set; }
    public Node CameFrom { get; set; }

    public virtual Vector3 GetVector3() { return new Vector3(X, 0, Y); }

    public Node(float x, float y) {
        X = x;
        Y = y;
    }
    public Node(Vector2 v) : this(v.x, v.y) { }
    public Node(Vector3 v) : this(v.x, v.z) { }
    public Node(Obj o) : this(o.X, o.Y) { }

    public T[,] CreateArrayOfSize<T>(){

        T[,] array = new T[(int)X, (int)Y];
        return array;

    }

    public override bool Equals(System.Object obj) {
        Node n = obj as Node;

        return X == n.X && Y == n.Y;
    }

    public override int GetHashCode() {
        return new { X, Y }.GetHashCode();
    }

    public float DistanceTo(Node n, bool diagonal) {

        if (diagonal) {
            float dx = Mathf.Abs(X - n.X);
            float dy = Mathf.Abs(Y - n.Y);
            return (dx + dy) + (sqrt2 - 2) * Mathf.Min(dx, dy);
        }

        return Mathf.Abs(X - n.X) + Mathf.Abs(Y - n.Y);
    }

    public float DistanceTo(Node n) {
        return DistanceTo(n, false);
    }

    public override string ToString() {
        return X + "_" + Y;
    }



    public float sqrt2 { get { return 1.41421356237f; } }
}

[System.Serializable]
public class Node3d : Node {

    public float Z { get; set; }

    public Node3d(float x, float y, float z) : base(x, y) {

        Z = z;

    }

    public Node3d(Vector3 v) : this(v.x, v.y, v.z) { }

    public override bool Equals(System.Object obj) {
        Node3d n = obj as Node3d;

        return X == n.X && Y == n.Y && Z == n.Z;
    }

    public override int GetHashCode() {
        return new { X, Y, Z }.GetHashCode();
    }

    public override Vector3 GetVector3() { return new Vector3(X, Y, Z); }

}