using System;
using System.Collections.Generic;
using SharpNav;
using SharpNav.Geometry;
using SharpNav.Pathfinding;

namespace Illarion.Server.Navigation
{
    internal class LocationInvestigator : ILocationInvestigator
    {
        internal const float DistancePerSecond = 1f;
        internal const int MaxPolys = 256;
        internal readonly Vector3 MaxExtent = new Vector3(0.5f, 0.2f, 0.5f);
        private readonly NavMesh _navMesh;

        internal LocationInvestigator()
        {
            var parser = new ObjParser("Map\\navmesh.obj");
            NavMeshGenerationSettings settings = NavMeshGenerationSettings.Default;
            settings.AgentHeight = 1.7f;
            settings.AgentRadius = 0.6f;

            _navMesh = NavMesh.Generate(parser.GetTriangles(), settings);
        }

        /// <summary>
        /// For a given character location and a wanted new location, checks if the new location was reachable for the client. If not a correction will be returned.
        /// </summary>
        /// <param name="location">The current location of the character on the server.</param>
        /// <param name="updatedLocation">The updated character location from the client.</param>
        /// <param name="deltaTime">The time between setting the server location and receiving the client location.</param>
        /// <returns>The new server location for the character to be used.</returns>
        public System.Numerics.Vector3 InvestigateUpdatedLocation(System.Numerics.Vector3 location, System.Numerics.Vector3 updatedLocation, float deltaTime)
        {
            if (deltaTime <= 0f) return location;

            var query = new NavMeshQuery(_navMesh, 2048);

            var startPoint = new Vector3(location.X, location.Y, location.Z);
            var endPoint = new Vector3(updatedLocation.X, updatedLocation.Y, updatedLocation.Z);
            var extent = new Vector3(5f, 5f, 5f);

            NavPoint startPoly = query.FindNearestPoly(startPoint, extent);
            NavPoint endPoly = query.FindNearestPoly(endPoint, extent);

            var startNavPoint = new NavPoint(startPoly.Polygon, startPoint);
            var endNavPoint = new NavPoint(endPoly.Polygon, endPoint);

            var path = new List<int>(MaxPolys);
            var isReachable = query.FindPath(ref startNavPoint, ref endNavPoint, path);

            // Case 1: Position not reachable: Do not move the character
            // The client should only send such an illegal move command
            // if is map is broken or he tries to cheat
            if (!isReachable) return location;

            // Get smoothed path from the polygon-only path
            List<Vector3> smoothPath = PathfindingSmoothPath(query, startPoly, endPoly, path);

            // Check and correct position
            var timeNeeded = 0f;
            for (var i = 0; i < smoothPath.Count - 1; i++)
            {
                Vector3 start = smoothPath[i];
                var diff = Vector3.Subtract(smoothPath[i + 1], start);
                var distance = (float)Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y + diff.Z * diff.Z);
                var timeForDistance = distance / DistancePerSecond;

                timeNeeded += timeForDistance;
                if (timeNeeded <= deltaTime) continue;

                // Case 2: With speed factor for maximum speed, target is still not reachable
                // Use the last reachable point and move it closer to the endpoint without breaking the time constraint
                var legalFactor = ( timeForDistance - ( timeNeeded - deltaTime ) ) / timeForDistance;
                var legalPos = Vector3.Add(start, Vector3.Multiply(diff, legalFactor));

                return new System.Numerics.Vector3(legalPos.X, legalPos.Y, legalPos.Z);
            }

            // Case 3: updatedLocation is fine
            // If server navmesh targetPosition is too different from updatedLocation use it instead of updatedLocation itself
            Vector3 pathEndpoint = smoothPath[smoothPath.Count - 1];
            var pathEndDiff = Vector3.Subtract(endPoint, pathEndpoint);
            if (pathEndDiff.X <= MaxExtent.X && pathEndDiff.Y <= MaxExtent.Y && pathEndDiff.Z <= MaxExtent.Z
                && pathEndDiff.X >= -MaxExtent.X && pathEndDiff.Y >= -MaxExtent.Y && pathEndDiff.Z >= -MaxExtent.Z)
            {
                return updatedLocation;
            }
            return new System.Numerics.Vector3(pathEndpoint.X, pathEndpoint.Y, pathEndpoint.Z);
        }

        private static List<Vector3> PathfindingSmoothPath(NavMeshQuery query, NavPoint startPoly, NavPoint endPoly, List<int> path)
        {
            // Make a smooth path from the list of polygons
            // Following code taken from SharpNav-Example-Repository
            var polyNumber = path.Count;
            var polys = path.ToArray();
            var iterationVector = new Vector3();
            var targetVector = new Vector3();
            query.ClosestPointOnPoly(startPoly.Polygon, startPoly.Position, ref iterationVector);
            query.ClosestPointOnPoly(polys[polyNumber - 1], endPoly.Position, ref targetVector);

            var smoothPath = new List<Vector3>(2048) { iterationVector };

            const float stepSize = 0.5f;
            const float slop = 0.01f;
            while (polyNumber > 0 && smoothPath.Count < smoothPath.Capacity)
            {
                //find location to steer towards
                var steerVector = new Vector3();
                var steerVectorFlag = 0;
                var steerVectorRef = 0;

                if (!GetSteerTarget(query, iterationVector, targetVector, slop, polys, polyNumber, ref steerVector, ref steerVectorFlag, ref steerVectorRef)) break;

                var endOfPath = (steerVectorFlag & PathfindingCommon.STRAIGHTPATH_END) != 0;
                var offMeshConnection = (steerVectorFlag & PathfindingCommon.STRAIGHTPATH_OFFMESH_CONNECTION) != 0;

                //find movement delta
                Vector3 delta = steerVector - iterationVector;
                var length = delta.Length();

                //if steer target is at end of path or off-mesh link
                //don't move past location
                if ((endOfPath || offMeshConnection) && length < stepSize) length = 1;
                else length = stepSize / length;

                var moveTarget = new Vector3();
                VMad(ref moveTarget, iterationVector, delta, length);

                //move
                var result = new Vector3();
                var visited = new List<int>(16);
                query.MoveAlongSurface(new NavPoint(polys[0], iterationVector), moveTarget, ref result, visited);
                polyNumber = FixupCorridor(polys, polyNumber, MaxPolys, visited);
                float height = 0;
                query.GetPolyHeight(polys[0], result, ref height);
                result.Y = height;
                iterationVector = result;

                //handle end of path when close enough
                if (endOfPath && InRange(iterationVector, steerVector, slop, 1.0f))
                {
                    //reached end of path
                    iterationVector = targetVector;
                    if (smoothPath.Count < smoothPath.Capacity) smoothPath.Add(iterationVector);

                    break;
                }

                //store results
                if (smoothPath.Count < smoothPath.Capacity) smoothPath.Add(iterationVector);
            }

            return smoothPath;
        }

        private static void VMad(ref Vector3 dest, Vector3 v1, Vector3 v2, float s)
        {
            dest.X = v1.X + v2.X * s;
            dest.Y = v1.Y + v2.Y * s;
            dest.Z = v1.Z + v2.Z * s;
        }

        private static bool GetSteerTarget(NavMeshQuery navMeshQuery, Vector3 startPos, Vector3 endPos, float minTargetDist, int[] path, int pathSize, ref Vector3 steerPos, ref int steerPosFlag, ref int steerPosRef)
        {
            const int maxSteerPoints = 3;
            var steerPath = new Vector3[maxSteerPoints];
            var steerPathFlags = new int[maxSteerPoints];
            var steerPathPolys = new int[maxSteerPoints];
            var nsteerPath = 0;
            navMeshQuery.FindStraightPath(startPos, endPos, path, pathSize, steerPath, steerPathFlags, steerPathPolys, ref nsteerPath, maxSteerPoints, 0);

            if (nsteerPath == 0) return false;

            //find vertex far enough to steer to
            var ns = 0;
            while (ns < nsteerPath)
            {
                if ((steerPathFlags[ns] & PathfindingCommon.STRAIGHTPATH_OFFMESH_CONNECTION) != 0 ||
                    !InRange(steerPath[ns], startPos, minTargetDist, 1000.0f))
                    break;

                ns++;
            }

            //failed to find good point to steer to
            if (ns >= nsteerPath) return false;

            steerPos = steerPath[ns];
            steerPos.Y = startPos.Y;
            steerPosFlag = steerPathFlags[ns];
            steerPosRef = steerPathPolys[ns];

            return true;
        }

        private static bool InRange(Vector3 v1, Vector3 v2, float r, float h)
        {
            var dx = v2.X - v1.X;
            var dy = v2.Y - v1.Y;
            var dz = v2.Z - v1.Z;
            return (dx * dx + dz * dz) < (r * r) && Math.Abs(dy) < h;
        }

        private static int FixupCorridor(int[] path, int npath, int maxPath, List<int> visited)
        {
            var furthestPath = -1;
            var furthestVisited = -1;

            //find furhtest common polygon
            for (var i = npath - 1; i >= 0; i--)
            {
                var found = false;
                for (var j = visited.Count - 1; j >= 0; j--)
                {
                    if (path[i] != visited[j]) continue;
                    furthestPath = i;
                    furthestVisited = j;
                    found = true;
                }

                if (found) break;
            }

            //if no intersection found, return current path
            if (furthestPath == -1 || furthestVisited == -1) return npath;

            //concatenate paths
            //adjust beginning of the buffer to include the visited
            var req = visited.Count - furthestVisited;
            var orig = Math.Min(furthestPath + 1, npath);
            var size = Math.Max(0, npath - orig);
            if (req + size > maxPath) size = maxPath - req;
            for (var i = 0; i < size; i++) path[req + i] = path[orig + i];

            //store visited
            for (var i = 0; i < req; i++) path[i] = visited[(visited.Count - 1) - i];

            return req + size;
        }
    }
}
