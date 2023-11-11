using System;
using System.Collections.Generic;
using System.Linq;
using Extension;
using Graph_and_Map;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.Events;

namespace TheRevenantsAge
{
    public partial class GroupMovementLogic : MonoBehaviour
    { 
        [SerializeField, Min(0)] private float speed = 1;

        private const float DELTA = 0.1f;

        private Node currentNode;
        private Node targetNode;
        private float progress;
        private List<Node> path = new();
        private Queue<Node> way = new();

        private StateMachine stateMachine;
        private Sleeping sleeping;
        private WaitingTarget waitingTarget;
        private Walking walking;
        private Group group;

        private bool needStop;
        private List<LineRenderer> turnObj;

        public bool CanMove { get; set; } = true;

        public int WayLength => way.Count;

        public bool IsCanLoot => stateMachine.CurrentState.Equals(sleeping);

        public Node CurrentNode
        {
            get
            {
                if (currentNode != null) return currentNode;

                currentNode = DotGraph.Instance.GetNearestNode(transform.position.To2D());
                transform.position = currentNode.transform.position;
                targetNode = currentNode;

                return currentNode;
            }
            private set
            {
                var oldHardNode = currentNode.GetComponent<HardNode>();
                currentNode = value;
                var hardNode = currentNode.GetComponent<HardNode>();
                if (hardNode is null)
                {
                    if (oldHardNode is not null)
                    {
                        GroupNodeChangedToHard?.Invoke(oldHardNode, null);
                    }
                }
                else
                {
                    GroupNodeChangedToHard?.Invoke(null, hardNode);
                }

                LocationChanged?.Invoke(currentNode.Location);
            }
        }

        public event Action<Location> LocationChanged;
        public event Action<HardNode, HardNode> GroupNodeChangedToHard;

        private void CreateWay()
        {
            way = new Queue<Node>(path.Skip(1));
        }

        private void Move()
        {
            var newGroupPos = Vector3.Lerp(CurrentNode.transform.position, targetNode.transform.position, progress);
            transform.position = newGroupPos;

            progress += Time.deltaTime * speed;
            if (IsNearly())
            {
                CurrentNode = targetNode;
                group.Move();

                progress = 0;
                if (needStop)
                {
                    needStop = false;
                    stateMachine.ChangeState(sleeping);
                }
                else if (way.Count == 0 || group.CurrentOnGlobalMapGroupEndurance == 0)
                {
                    stateMachine.ChangeState(sleeping);
                    group.EndMovingOnThisTurn();
                    CanMove = false;
                }
                else
                {
                    targetNode = way.Dequeue();
                }
            }

            bool IsNearly()
            {
                var curPos = transform.position.To2D();
                var targetPos = targetNode.PositionIn2D;
                return Vector2.Distance(curPos, targetPos) <= DELTA;
            }
        }

        private void DrawPath()
        {
            path = PathFinder.FindShortestWay(CurrentNode, DotGraph.Instance.GetNearestNodeToMouse());
            if (path.Count <= 1)
                return;

            var currentObjIndex = 0;
            var endPartIndex = group.CurrentOnGlobalMapGroupEndurance;
            var pointsList = new List<Vector3>(group.MaxOnGlobalMapGroupEndurance);
            for (var i = 0; i < path.Count; i++)
            {
                var node = path[i];
                if (i <= endPartIndex)
                {
                    pointsList.Add(node.transform.position);
                }
                else
                {
                    DrawLineRenderer(turnObj[currentObjIndex], pointsList);
                    var lastPoint = pointsList.Last();
                    pointsList.Clear();
                    pointsList.Add(lastPoint);
                    currentObjIndex++;

                    if (currentObjIndex == turnObj.Count - 1)
                    {
                        endPartIndex = int.MaxValue;
                    }
                    else
                    {
                        endPartIndex += group.MaxOnGlobalMapGroupEndurance;
                    }
                }
            }

            DrawLineRenderer(turnObj[currentObjIndex], pointsList);

            for (var i = currentObjIndex + 1; i < turnObj.Count; i++)
            {
                DrawLineRenderer(turnObj[i], Array.Empty<Vector3>());
            }

            void DrawLineRenderer(LineRenderer lineRenderer, IReadOnlyCollection<Vector3> points)
            {
                if (points.Count > 0)
                {
                    lineRenderer.gameObject.SetActive(true);
                    lineRenderer.positionCount = points.Count;
                    lineRenderer.SetPositions(points.Select(x => new Vector3(x.x, 0.05f, x.z)).ToArray());
                    lineRenderer.transform.position = points.Last();
                }
                else
                {
                    lineRenderer.gameObject.gameObject.SetActive(false);
                    lineRenderer.positionCount = 0;
                }
            }
        }

        private void ClearWay()
        {
            path.Clear();
            way.Clear();
            foreach (var o in turnObj)
            {
                o.positionCount = 0;
                o.gameObject.SetActive(false);
            }
        }

        public void PreparingToMove()
        {
            if (stateMachine.CurrentState == sleeping && CanMove)
                stateMachine.ChangeState(waitingTarget);
        }

        public void StopMove()
        {
            if (stateMachine.CurrentState == walking)
                needStop = true;
        }

        public void OnTurnEnd()
        {
            CanMove = true;
        }

        #region MonoBehaviourCallBack

        private void Awake()
        {
            turnObj = TurnController.Instance.TurnObj.ToList();
            if (turnObj.Count == 0)
                throw new Exception("Нет объектов хода");
            stateMachine = new StateMachine();
            sleeping = new Sleeping(this, stateMachine);
            waitingTarget = new WaitingTarget(this, stateMachine);
            walking = new Walking(this, stateMachine);

            group = GetComponent<Group>();
            stateMachine.Initialize(sleeping);
        }

        private void OnEnable()
        {
            GameEventsController.Instance.EventActivated += OnEventActivated;
        }

        private void OnDisable()
        {
            GameEventsController.Instance.EventActivated -= OnEventActivated;
        }

        private void OnEventActivated(IGameEvent obj)
        {
            StopMove();
        }

        private void Update()
        {
            stateMachine.CurrentState.Update();
        }

        private void FixedUpdate()
        {
            stateMachine.CurrentState.FixedUpdate();
        }

        #endregion
    }
}