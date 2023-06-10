using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;


[TaskDescription("Patrol around the specified waypoints using the Unity NavMesh.")]
[TaskCategory("Movement")]
[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}PatrolIcon.png")]
public class CustomPatrol : NavMeshMovement
{
    [UnityEngine.Tooltip("Should the agent patrol the waypoints randomly?")]
    public SharedBool randomPatrol = false;

    [UnityEngine.Tooltip("The length of time that the agent should pause when arriving at a waypoint")]
    public SharedFloat waypointPauseDuration = 0;

    [UnityEngine.Tooltip("The waypoints to move to")]
    public SharedGameObjectList waypoints;

    public string BlendTreeParamName;
    // The current index that we are heading towards within the waypoints array
    private int waypointIndex;
    private float waypointReachedTime;

    public override void OnStart()
    {
        base.OnStart();
        
        if (waypoints.Value.Count == 0 || waypoints.Value[0] == null)
        {
            return;
        }

        // initially move towards the closest waypoint
        float distance = Mathf.Infinity;
        float localDistance;
        for (int i = 0; i < waypoints.Value.Count; ++i)
        {
            if ((localDistance = Vector3.Magnitude(transform.position - waypoints.Value[i].transform.position)) <
                distance)
            {
                distance = localDistance;
                waypointIndex = i;
            }
        }

        waypointReachedTime = -1;
        SetDestination(Target());
    }

    // Patrol around the different waypoints specified in the waypoint array. Always return a task status of running. 
    public override TaskStatus OnUpdate()
    {
        if (waypoints.Value.Count == 0 || waypoints.Value[0] == null)
        {
            return TaskStatus.Success;
        }

        var _animator = GetComponent<Animator>();
        _animator.SetFloat(BlendTreeParamName, navMeshAgent.velocity.magnitude);
        

        if (HasArrived())
        {
            if (waypointReachedTime == -1)
            {
                waypointReachedTime = Time.time;
            }

            // wait the required duration before switching waypoints.
            if (waypointReachedTime + waypointPauseDuration.Value <= Time.time)
            {
                if (randomPatrol.Value)
                {
                    if (waypoints.Value.Count == 1)
                    {
                        waypointIndex = 0;
                    }
                    else
                    {
                        // prevent the same waypoint from being selected
                        var newWaypointIndex = waypointIndex;
                        while (newWaypointIndex == waypointIndex)
                        {
                            newWaypointIndex = Random.Range(0, waypoints.Value.Count);
                        }

                        waypointIndex = newWaypointIndex;
                    }
                }
                else
                {
                    waypointIndex = (waypointIndex + 1) % waypoints.Value.Count;
                }

                SetDestination(Target());
                waypointReachedTime = -1;
            }
        }

        return TaskStatus.Running;
    }

    // Return the current waypoint index position
    private Vector3 Target()
    {
        if (waypointIndex >= waypoints.Value.Count)
        {
            return transform.position;
        }

        return waypoints.Value[waypointIndex].transform.position;
    }

    // Reset the public variables
    public override void OnReset()
    {
        base.OnReset();

        randomPatrol = false;
        waypointPauseDuration = 0;
        waypoints = null;
    }

    // Draw a gizmo indicating a patrol 
    public override void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (waypoints == null || waypoints.Value == null)
        {
            return;
        }

        var oldColor = UnityEditor.Handles.color;
        UnityEditor.Handles.color = Color.yellow;
        for (int i = 0; i < waypoints.Value.Count; ++i)
        {
            if (waypoints.Value[i] != null)
            {
                UnityEditor.Handles.SphereHandleCap(0, waypoints.Value[i].transform.position,
                    waypoints.Value[i].transform.rotation, 1, EventType.Repaint);
            }
        }

        UnityEditor.Handles.color = oldColor;
#endif
    }
}