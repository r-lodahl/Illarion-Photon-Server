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
        private IServiceProvider ServiceProvider { get; }
        private readonly NavMesh _navMesh;

        internal LocationInvestigator(IServiceProvider provider)
        {
            ServiceProvider = provider ?? throw new ArgumentNullException(nameof(provider));

            var parser = new ObjParser("Map\\navmesh.obj");
            var settings = NavMeshGenerationSettings.Default;
            settings.AgentHeight = 1.7f;
            settings.AgentRadius = 0.6f;

            _navMesh = NavMesh.Generate(parser.GetTriangles(), settings);
        }

        /// <summary>
        /// For a given character location and a wanted new location, checks if the new location was reachable for the client. If not a correction will be returned.
        /// </summary>
        /// <param name="location">The current location of the character on the server.</param>
        /// <param name="updatedLocation">The updated character location from the client.</param>
        /// <param name="velocity">The current velocity of the character.</param>
        /// <param name="deltaTime">The time between setting the server location and receiving the client location.</param>
        /// <returns>The new server location for the character to be used.</returns>
        public System.Numerics.Vector3 InvestigateUpdatedLocation(System.Numerics.Vector3 location, System.Numerics.Vector3 updatedLocation, float deltaTime)
        {
            var query = new NavMeshQuery(_navMesh, 2048);

            var startPoint = new Vector3(location.X, location.Y, location.Z);
            var endPoint = new Vector3(updatedLocation.X, updatedLocation.Y, updatedLocation.Z);
            var extent = new Vector3(5f, 5f, 5f);

            var startPoly = query.FindNearestPoly(startPoint, extent);
            var endPoly = query.FindNearestPoly(endPoint, extent);

            var startNavPoint = new NavPoint(startPoly.Polygon, startPoint);
            var endNavPoint = new NavPoint(endPoly.Polygon, endPoint);

            var maxPolys = 256;
            var path = new List<int>(maxPolys);
            var IsReachable = query.FindPath(ref startNavPoint, ref endNavPoint, path);

            if (!IsReachable)
            {
                // TODO: This should only happen if the user is cheating
                return location;
            }

            // Make a smooth path from the list of polygons
            // Following code taken from SharpNav-Example-Repository

            //find a smooth path over the mesh surface
            var npolys = path.Count;
            var polys = path.ToArray();
            var iterPos = new Vector3();
            var targetPos = new Vector3();
            query.ClosestPointOnPoly(startPoly.Polygon, startPoly.Position, ref iterPos);
            query.ClosestPointOnPoly(polys[npolys - 1], endPoly.Position, ref targetPos);

            var smoothPath = new List<Vector3>(2048) {iterPos};
            var smoothPathDistance = 0f;

            const float stepSize = 0.5f;
            const float slop = 0.01f;
            while (npolys > 0 && smoothPath.Count < smoothPath.Capacity)
            {
                //find location to steer towards
                var steerPos = new Vector3();
                var steerPosFlag = 0;
                var steerPosRef = 0;

                if (!GetSteerTarget(query, iterPos, targetPos, slop, polys, npolys, ref steerPos, ref steerPosFlag, ref steerPosRef)) break;

                var endOfPath = ( steerPosFlag & PathfindingCommon.STRAIGHTPATH_END ) != 0;
                var offMeshConnection = ( steerPosFlag & PathfindingCommon.STRAIGHTPATH_OFFMESH_CONNECTION ) != 0;

                //find movement delta
                var delta = steerPos - iterPos;
                var len = (float) Math.Sqrt(Vector3.Dot(delta, delta));

                //if steer target is at end of path or off-mesh link
                //don't move past location
                if (( endOfPath || offMeshConnection ) && len < stepSize) len = 1;
                else len = stepSize / len;

                var moveTgt = new Vector3();
                VMad(ref moveTgt, iterPos, delta, len);

                // Sum up distance
                smoothPathDistance += len;

                //move
                var result = new Vector3();
                var visited = new List<int>(16);
                query.MoveAlongSurface(new NavPoint(polys[0], iterPos), moveTgt, ref result, visited);
                npolys = FixupCorridor(polys, npolys, maxPolys, visited);
                float h = 0;
                query.GetPolyHeight(polys[0], result, ref h);
                result.Y = h;
                iterPos = result;

                //handle end of path when close enough
                if (endOfPath && InRange(iterPos, steerPos, slop, 1.0f))
                {
                    //reached end of path
                    iterPos = targetPos;
                    if (smoothPath.Count < smoothPath.Capacity) smoothPath.Add(iterPos);

                    break;
                }

                //store results
                if (smoothPath.Count < smoothPath.Capacity) smoothPath.Add(iterPos);
            }

            // Set correct position

            var timeNeeded = smoothPathDistance / DistancePerSecond;

            if (timeNeeded > deltaTime)
            {
                // TODO: Put player back on calculated path for him
                return System.Numerics.Vector3.One;
            }

            var pathEndpoint = smoothPath[smoothPath.Count - 1];
            var differenceSquared = Vector3.Subtract(endPoint, pathEndpoint).LengthSquared();
            if (differenceSquared > 5f)
            {
                // TODO: Define correct threshold when server response should be used instead of client reported location
                return new System.Numerics.Vector3(pathEndpoint.X, pathEndpoint.Y, pathEndpoint.Z);
            }

            // Client update is fine (should mostly the case)
            return updatedLocation;
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
