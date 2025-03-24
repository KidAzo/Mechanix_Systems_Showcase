using FactoryTemplate.Concealer;
using FactoryTemplate.ConveyorCreator;
using FactoryTemplate.EventHandling;
using FactoryTemplate.Machines;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryTemplate.SceneManagement
{
	public class ShaderConfig : MonoBehaviour
	{
		[SerializeField] MachineBuildedEventChannel machineBuildedEC;
		[SerializeField] ConveyorBuildedEventChannel conveyorBuildedEC;
		[SerializeField] MachineRemovedEventChannel machineRemovedEC;
		[SerializeField] Material gridMaterial;
		HashSet<Transform> _machines = new();
		ComputeBuffer machineBuffer;
		ConcealerShaderConfig _concealerConfig;
		float _offset = 0.5f;

		private void Awake()
		{
			_concealerConfig = new ConcealerShaderConfig(gridMaterial);
		}

		public struct ColliderData
		{
			public Vector3 position;
			public Vector3 size;
		}

		void OnEnable()
		{
			machineBuildedEC.OnEventRaised += Add;
			conveyorBuildedEC.OnEventRaised += Add;
			machineRemovedEC.OnEventRaised += Remove;
			Bus<ConcealerCreatedEvent>.OnEvent += Add;
			//Bus<ConcealerCreatedEvent>.OnEvent += Remove;
		}

		void OnDisable()
		{
			machineBuildedEC.OnEventRaised -= Add;
			conveyorBuildedEC.OnEventRaised -= Add;
			machineRemovedEC.OnEventRaised -= Remove;
			Bus<ConcealerCreatedEvent>.OnEvent -= Add;
			//Bus<ConcealerCreatedEvent>.OnEvent -= Remove;
		}

		void Add(MachineBuildedEvent evt) => _machines.Add(evt.machine.transform);
		void Add(ConveyorBuildedEvent evt) => _machines.Add(evt.conveyor.transform);
		void Add(ConcealerCreatedEvent evt) => _concealerConfig.Add(evt.collider);
		void Remove(MachineRemovedEvent evt) => _machines.Remove(evt.machine.transform);
		void Remove(ConcealerCreatedEvent evt) => _concealerConfig.Remove(evt.collider);

		void Update()
		{
			_concealerConfig.Config();

			//if (_machines.Count == 0) return;

			var colliderDataList = new List<ColliderData>();
			foreach (var machine in _machines)
			{
				if (machine.transform.position.y < 0)
				{
					if (machine.TryGetComponent(out ConcealerObject concealer)) continue;

					var colliderDirectors = machine.GetComponent<ColliderDirector>();
					var colliders = colliderDirectors.GetColliders();
					foreach (var collider in colliders)
					{
						Vector3 offsetSize = new Vector3(
						   collider.bounds.size.x + _offset,
						   collider.bounds.size.y,
						   collider.bounds.size.z + _offset
					   );

						ColliderData data = new ColliderData
						{
							position = collider.bounds.center,
							size = offsetSize
						};

						colliderDataList.Add(data);
					}
				}
			}

			foreach (var collider in _concealerConfig.colliderDataList)
			{
				colliderDataList.Add(collider);
			}

			if (machineBuffer != null)
				machineBuffer.Release();

			if (colliderDataList.Count == 0) return;

			machineBuffer = new ComputeBuffer(colliderDataList.Count, sizeof(float) * 6);
			machineBuffer.SetData(colliderDataList);

			gridMaterial.SetBuffer("colliderData", machineBuffer);
			gridMaterial.SetInt("colliderCount", colliderDataList.Count);

		}

		void OnDestroy()
		{
			if (machineBuffer != null)
				machineBuffer.Release();
		}
	}

	public class ConcealerShaderConfig
	{
		HashSet<BoxCollider> _concealers;
		public List<ShaderConfig.ColliderData> colliderDataList = new();

		public ConcealerShaderConfig(Material gridMaterial)
		{
			_concealers = new HashSet<BoxCollider>();
			_gridMaterial = gridMaterial;
		}

		public void Add(BoxCollider c) => _concealers.Add(c);
		public void Remove(BoxCollider c) => _concealers.Remove(c);

		public void Config()
		{
			if (_concealers.Count == 0) return;

			colliderDataList = new List<ShaderConfig.ColliderData>();

			foreach (var collider in _concealers)
			{
				Vector3 bottomCenter = new Vector3(
					collider.bounds.center.x,
					collider.bounds.min.y,
					collider.bounds.center.z
				);

				ShaderConfig.ColliderData data = new ShaderConfig.ColliderData
				{
					position = bottomCenter,
					size = collider.bounds.size
				};

				colliderDataList.Add(data);
			}
		}
	}
}