using System.Collections.Generic;
using System.IO;
using FactoryTemplate.Concealer;
using FactoryTemplate.Machines;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Utility class for exporting Unity GameObject hierarchies to OBJ format, handling mesh filtering,
/// transformation to world space, and preserving object structure through named groups.
/// That is part of my biggest project you can make this code more abstract and use it in your project
/// </summary>

public static class OBJExporter
{
	public static void Export(GameObject parent, string filePath, string nameOfExport)
	{
		List<MeshFilter> meshFilters = new List<MeshFilter>();
		var childFilters = parent.GetComponentsInChildren<MeshFilter>();

		foreach (var filter in childFilters)
		{
			if (filter.GetComponent<MeshRenderer>() != null && filter.sharedMesh != null)
			{
				MeshRenderer renderer = filter.GetComponent<MeshRenderer>();
				bool hasNonOutlineMaterial = false;

				if (renderer.sharedMaterials == null || renderer.sharedMaterials.Length == 0) continue;

				foreach (Material mat in renderer.sharedMaterials)
				{

					if (!mat.name.Contains("Outline"))
					{
						hasNonOutlineMaterial = true;
						break;
					}
				}

				if (hasNonOutlineMaterial)
				{
					meshFilters.Add(filter);
				}
			}
		}

		if (meshFilters.Count == 0)
		{
			Debug.LogError("No mesh!");
			return;
		}

		using (StreamWriter writer = new StreamWriter(filePath))
		{
			int vertexOffset = 0;

			writer.WriteLine($"o {nameOfExport}");

			foreach (MeshFilter mf in meshFilters)
			{
				Mesh mesh = mf.sharedMesh;
				Transform t = mf.transform;

				var machine = t.GetComponent<Machine>() ?? t.GetComponentInParent<Machine>();

				if (machine != null)
				{
					var selectedObject = machine.Selected();

					string objectName;

					objectName = $"{selectedObject.name}_{selectedObject.GetInstanceID()}";

					writer.WriteLine($"g {objectName}");
				}
				else if (t.TryGetComponent(out ConcealerObject cc))
				{
					string objectName = $"{cc.gameObject.name}_{cc.GetInstanceID()}";
					writer.WriteLine($"g {objectName}");
				}

				foreach (Vector3 vertex in mesh.vertices)
				{
					Vector3 worldVertex = t.TransformPoint(vertex);
					writer.WriteLine($"v {worldVertex.x.ToString("F6", System.Globalization.CultureInfo.InvariantCulture)} " +
									 $"{worldVertex.y.ToString("F6", System.Globalization.CultureInfo.InvariantCulture)} " +
									 $"{(worldVertex.z).ToString("F6", System.Globalization.CultureInfo.InvariantCulture)}");
				}

				if (mesh.normals.Length > 0)
				{
					foreach (Vector3 normal in mesh.normals)
					{
						Vector3 worldNormal = t.TransformDirection(normal).normalized;
						writer.WriteLine($"vn {worldNormal.x.ToString("F6", System.Globalization.CultureInfo.InvariantCulture)} " +
										 $"{worldNormal.y.ToString("F6", System.Globalization.CultureInfo.InvariantCulture)} " +
										 $"{worldNormal.z.ToString("F6", System.Globalization.CultureInfo.InvariantCulture)}");
					}
				}

				ProcessMeshUVAndTriangles(mesh, writer, vertexOffset);

				vertexOffset += mesh.vertexCount;
			}
		}

		Debug.Log($"OBJ Export process has been finished: {filePath}");
	}

	private static void ProcessMeshUVAndTriangles(Mesh mesh, StreamWriter writer, int vertexOffset)
	{
		for (int subMesh = 0; subMesh < mesh.subMeshCount; subMesh++)
		{
			int[] triangles = mesh.GetTriangles(subMesh);
			if (triangles.Length == 0) continue;

			for (int i = 0; i < triangles.Length; i += 3)
			{
				writer.WriteLine($"f {triangles[i] + 1 + vertexOffset}/{triangles[i] + 1 + vertexOffset}/{triangles[i] + 1 + vertexOffset} " +
								 $"{triangles[i + 1] + 1 + vertexOffset}/{triangles[i + 1] + 1 + vertexOffset}/{triangles[i + 1] + 1 + vertexOffset} " +
								 $"{triangles[i + 2] + 1 + vertexOffset}/{triangles[i + 2] + 1 + vertexOffset}/{triangles[i + 2] + 1 + vertexOffset}");
			}
		}
	}
}
