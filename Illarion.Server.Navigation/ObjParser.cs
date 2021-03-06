// Copyright (c) 2013, 2015 Robert Rouhani <robert.rouhani@gmail.com> and other contributors (see CONTRIBUTORS file).
// Licensed under the MIT License - https://raw.github.com/Robmaister/SharpNav/master/LICENSE
// Edited by r-lodahl for Illarion Photon Server

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SharpNav;
using SharpNav.Geometry;

namespace Illarion.Server.Navigation
{
	/// <summary>
	/// Parses a model in .obj format.
	/// </summary>
	public class ObjParser
	{
		private static readonly char[] LineSplitChars = { ' ' };

		private readonly List<Triangle3> _tris;

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjParser"/> class.
		/// </summary>
		/// <param name="path">The path of the .obj file to parse.</param>
		public ObjParser(string path)
		{
			_tris = new List<Triangle3>();
			var tempVerts = new List<Vector3>();

			using (var reader = new StreamReader(path))
			{
				var file = reader.ReadToEnd();
				foreach (var line in file.Split('\n'))
				{
					//trim any extras
					var trimedLine = line;
					var commentStart = line.IndexOf("#", StringComparison.Ordinal);
					if (commentStart != -1) trimedLine = trimedLine.Substring(0, commentStart);
					trimedLine = trimedLine.Trim();

					var splitLine = trimedLine.Split(LineSplitChars, StringSplitOptions.RemoveEmptyEntries);
					if (splitLine.Length == 0) continue;

					switch (splitLine[0])
					{
						case "v":
							if (splitLine.Length < 4) continue;

						    if (!TryParseVector(splitLine, 1, 2, 3, out Vector3 v)) continue;
							tempVerts.Add(v);
							break;
						case "f":
							if (splitLine.Length < 4) continue;
							else if (splitLine.Length == 4)
							{
							    if (!int.TryParse(splitLine[1].Split('/')[0], out var v0)) continue;
								if (!int.TryParse(splitLine[2].Split('/')[0], out var v1)) continue;
								if (!int.TryParse(splitLine[3].Split('/')[0], out var v2)) continue;

								v0 -= 1;
								v1 -= 1;
								v2 -= 1;

								_tris.Add(new Triangle3(tempVerts[v0], tempVerts[v1], tempVerts[v2]));
							}
							break;
					}
				}
			}
		}

        /// <summary>
        /// Gets an array of the triangles in this model.
        /// </summary>
        /// <returns></returns>
        public Triangle3[] GetTriangles() => _tris.ToArray();

	    private static bool TryParseVector(string[] values, int x, int y, int z, out Vector3 v)
		{
			v = Vector3.Zero;

			if (!float.TryParse(values[x], NumberStyles.Any, CultureInfo.InvariantCulture, out v.X)) return false;
			if (!float.TryParse(values[y], NumberStyles.Any, CultureInfo.InvariantCulture, out v.Y)) return false;
			if (!float.TryParse(values[z], NumberStyles.Any, CultureInfo.InvariantCulture, out v.Z)) return false;

			return true;
		}
	}
}