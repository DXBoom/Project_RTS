using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;

public class A_Manager : MonoBehaviour
{
    public static A_Manager Instance;
    
    private List<A_Node> _grid = new List<A_Node>();
    
    [Header("Grid Settings")] [Space]
    [SerializeField] private Vector3 worldPosition;
    [SerializeField] private Vector3 worldSize;
    [SerializeField] private LayerMask unWalkableLayers;

    [Header("A* Settings")] [Space]
    [SerializeField] private bool multiThreading = false;
    [SerializeField] private float nodeYPos;

    [Header("Player Data")] [Space] 
    public Vector3 mainPlayerPos;
    public bool mainPlayerMove;
    
    private float _biggerBorderX;
    private float _smallerBorderX;
    private float _biggerBorderZ;
    private float _smallerBorderZ;

    private readonly Queue<PathResponse> _results = new Queue<PathResponse>();   //Store The Response Result
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
   
    private void Start()
    {
        CalculateWorldMapBorders();
        GenerateGrid();
    }

    private void Update()
    {
        CallBackTheResult();
    }

    #region Manager
    public void FindingPath(PathRequest request ,Action<PathResponse> callBack)
    {
        if (_grid == null)
            return ;
      
        A_Heap<A_Node> openList = new A_Heap<A_Node>();
        List<A_Node> closeList = new List<A_Node>();

        A_Node agentNode = GetPointNodeFromGridByPosition(request.startNode);
        A_Node targetNode = GetPointNodeFromGridByPosition(request.targetNode);

        if (targetNode==null)
            return;

        openList.Add(agentNode);
        
        while (openList.Count > 0)
        {
            A_Node currentNode = openList.Pop();

            if (currentNode == null)
                return;

            //Check If Reach The Goal 
            if (CheckIfPointsLinkedTogether(currentNode, targetNode))
            {
                targetNode.parent = currentNode;
                break;
            }

            closeList.Add(currentNode);
            List<A_Node> neighboursNode = currentNode.neighbors;
          
            for (int i=0; i < neighboursNode.Count ; i++)
            {
                if (closeList.Contains(neighboursNode[i]))
                    continue;
                else
                {
                    float gCost = currentNode.gCost + CalculateMovementCostToTargetNode(currentNode, neighboursNode[i]);
                    float hCost = CalculateMovementCostToTargetNode(neighboursNode[i], targetNode);

                    if (openList.Contains(neighboursNode[i]) == false)
                    {
                        //Set fValue
                        neighboursNode[i].fCost = hCost + gCost;
                        neighboursNode[i].parent = currentNode;
                        openList.Add(neighboursNode[i]);
                    }
                    else
                    {
                        float new_fCost = gCost+hCost;

                        //Check if new fvalue is best than the older 
                        if (neighboursNode[i].fCost > new_fCost)
                        {
                            //Update fValue 
                            neighboursNode[i].fCost = new_fCost;
                            neighboursNode[i].parent = currentNode;
                        }
                    }
                }
            }
        }

        // Create The Path 
        Vector3[] path = CreatePath(agentNode, targetNode);

        //Create Response To Call Back it 
        PathResponse pathResponse = new PathResponse(path, true, request.callBack);

        //Call Back 
        callBack(pathResponse);
        return;
    }

    //Calculate The Cost from current node  To target node 
    float CalculateMovementCostToTargetNode(A_Node _node, A_Node _targetNode)
    {
        return Vector3.Distance(_node.position, _targetNode.position);
    }
    
    private Vector3[] CreatePath(A_Node _agentNode, A_Node _targetNode)
    {
        List<A_Node> pathNode = new List<A_Node>();
        A_Node currentNode = _targetNode;
       
        while (currentNode != _agentNode)
        {
            pathNode.Add(currentNode);
            currentNode = currentNode.parent;
        }

        pathNode.Reverse();
        Vector3[] path = new Vector3[pathNode.Count ];

        for(int i=0; i< pathNode.Count  ;i++)
        {
            pathNode[i].position.y = nodeYPos;
            path[i] = pathNode[i].position;
        }
        return path;
    }
    #endregion

    #region Grid
    private void CalculateWorldMapBorders()
    {
        _biggerBorderX = worldPosition.x + worldSize.x / 2;
        _smallerBorderX = worldPosition.x - worldSize.x / 2;
        _biggerBorderZ = worldPosition.z + worldSize.z / 2;
        _smallerBorderZ = worldPosition.z - worldSize.z / 2;
    }

    public void GenerateGrid()
    {
        _grid = new List<A_Node>();
        GameObject[] Objects = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];

        //Build Points Around Obstacles
        foreach(GameObject obj in Objects)
        {
            if (obj.layer == 8)
            {
                BuildPointNode(obj.transform);
            }
        }

        //Calculate Neighbors for each points
        for (int i = 0 ; i < _grid.Count ; i++)
        {
            List<A_Node> neighbors = CalculateNeighbors(_grid[i]);

            _grid[i].AddNeighbors(neighbors);           
        }
    }

    private void BuildPointNode(Transform obstacle)
    {
        float xSize = obstacle.transform.localScale.x / 2 + 0.7f;
        float zSize = obstacle.transform.localScale.z / 2 + 0.7f;

        Vector3 forwardDirection = obstacle.forward * zSize;
        Vector3 rightDirection = obstacle.right * xSize;

        //Build four point around obstacle
        for (int i=-1; i<=1;i=i+2)
        {
            for(int j=-1;j<=1;j=j+2)
            {
                Vector3 pos = i * forwardDirection + j * rightDirection + obstacle.position;
                //Check the new point is Walkable
                if (CheckIsWalk(pos))
                {
                    A_Node pointNode = new A_Node(pos);
                    _grid.Add(pointNode);
                }
            }
        }
    }

    public A_Node GetPointNodeFromGridByPosition(Vector3 positions)
    {
        A_Node pointNode = new A_Node(positions);

        if(CheckIsWalk(positions))
        {
            pointNode.neighbors = CalculateNeighbors(pointNode);
            return pointNode;        
        }

        return null;
    }

    public List<A_Node> CalculateNeighbors(A_Node pointNode)
    {
        List<A_Node> neighbors = new List<A_Node>();

        for (int i = 0; i < _grid.Count; i++)
        {
            if (CheckIfPointsLinkedTogether(pointNode, _grid[i])  && pointNode.position != _grid[i].position)
                neighbors.Add(_grid[i]);
        }
        return neighbors;
    }

    public bool CheckIfPointsLinkedTogether(A_Node point1, A_Node point2)
    {
        if (point1 == null || point2 == null)
            return false;

        if(Physics.Linecast(point1.position,point2.position,unWalkableLayers))
            return false;
        
        return true;
    }

    private bool CheckIsWalk(Vector3 pointNodePosition)
    {
        if (pointNodePosition.x > _biggerBorderX || pointNodePosition.x < _smallerBorderX)
            return false;

        if (pointNodePosition.z > _biggerBorderZ || pointNodePosition.z < _smallerBorderZ)
            return false;

        if (Physics.OverlapSphere(pointNodePosition, 0.2f, unWalkableLayers).Length > 0)
            return false;

        return true;
    }

    #endregion
    
    #region Path Request
    // Call back The Result to Each Player Request
    private void CallBackTheResult()
    {
        if (_results.Count > 0)
        {
            lock (_results)
            {
                for (int i = 0; i < _results.Count; i++)
                {
                    PathResponse pathResponse = _results.Dequeue();
                    pathResponse.callBack(pathResponse.path, pathResponse.succes);
                }
            }
        }
    }

    public void Request(PathRequest pathRequest)
    {
        if(multiThreading)
        {
            //Make New Thread for Finding Path
            ThreadStart threadStart = delegate
            {
                A_Manager.Instance.FindingPath(pathRequest, FinishProcessing);
            };

            threadStart.Invoke();
        }
        else
        {
            A_Manager.Instance.FindingPath(pathRequest, FinishProcessing);
        }
    }

    public void FinishProcessing(PathResponse pathResponse)
    {
        if(multiThreading)
        {
            //Add New Result To Queue Result To Call Back it 
            lock (_results)
            {
                _results.Enqueue(pathResponse);
            }
        }
        else
        {
            pathResponse.callBack(pathResponse.path, pathResponse.succes);
        }
    }
    #endregion
}

public struct PathResponse
{
    public Vector3[] path;
    public bool succes;
    public Action<Vector3[], bool> callBack;

    public PathResponse(Vector3[] path, bool succes, Action<Vector3[], bool> callBack)
    {
        this.path = path;
        this.succes = succes;
        this.callBack = callBack;
    }

}

public struct PathRequest
{
    public Vector3 startNode;
    public Vector3 targetNode;
    public Action<Vector3[], bool> callBack;

    public PathRequest(Vector3 startNode, Vector3 targetNode, Action<Vector3[], bool> callBack)
    {
        this.startNode = startNode;
        this.targetNode = targetNode;
        this.callBack = callBack;
    }
}