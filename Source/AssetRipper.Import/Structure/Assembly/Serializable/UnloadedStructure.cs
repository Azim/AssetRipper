﻿using AssetRipper.Assets;
using AssetRipper.Assets.Cloning;
using AssetRipper.Assets.Export;
using AssetRipper.Assets.Generics;
using AssetRipper.Assets.IO.Writing;
using AssetRipper.Assets.Metadata;
using AssetRipper.Assets.Traversal;
using AssetRipper.Import.Structure.Assembly.Managers;
using AssetRipper.IO.Endian;
using AssetRipper.SourceGenerated.Classes.ClassID_114;
using AssetRipper.Yaml;

namespace AssetRipper.Import.Structure.Assembly.Serializable;

/// <summary>
/// This is a placeholder asset that lazily reads the actual structure sometime after all the assets have been loaded.
/// This allows MonoBehaviours to be loaded before their referenced MonoScript.
/// </summary>
public sealed class UnloadedStructure : UnityAssetBase
{
	/// <summary>
	/// The <see cref="IMonoBehaviour"/> that <see langword="this"/> is the <see cref="IMonoBehaviour.Structure"/> for.
	/// </summary>
	private readonly IMonoBehaviour monoBehaviour;

	private readonly IAssemblyManager assemblyManager;

	/// <summary>
	/// The segment of data for this structure.
	/// </summary>
	private readonly ReadOnlyArraySegment<byte> structureData;

	public UnloadedStructure(IMonoBehaviour monoBehaviour, IAssemblyManager assemblyManager, ReadOnlyArraySegment<byte> structureData)
	{
		this.monoBehaviour = monoBehaviour;
		this.assemblyManager = assemblyManager;
		this.structureData = structureData;
	}

	private void ThrowIfNotStructure()
	{
		if (!ReferenceEquals(monoBehaviour.Structure, this))
		{
			throw new InvalidOperationException("The MonoBehaviour structure has already been loaded.");
		}
	}

	public SerializableStructure? LoadStructure()
	{
		ThrowIfNotStructure();
		SerializableStructure? structure = monoBehaviour.ScriptP?.GetBehaviourType(assemblyManager)?.CreateSerializableStructure();
		if (structure is not null)
		{
			EndianSpanReader reader = new EndianSpanReader(structureData, monoBehaviour.Collection.EndianType);
			if (structure.TryRead(ref reader, monoBehaviour.Collection.Version, monoBehaviour.Collection.Flags))
			{
				monoBehaviour.Structure = structure;
				return structure;
			}
		}

		monoBehaviour.Structure = null;
		return null;
	}

	#region UnityAssetBase Overrides
	public override bool FlowMappedInYaml => LoadStructure()?.FlowMappedInYaml ?? base.FlowMappedInYaml;

	public override int SerializedVersion => LoadStructure()?.SerializedVersion ?? base.SerializedVersion;

	public override YamlMappingNode ExportYamlEditor(IExportContainer container) => LoadStructure()?.ExportYamlEditor(container) ?? new();

	public override YamlMappingNode ExportYamlRelease(IExportContainer container) => LoadStructure()?.ExportYamlRelease(container) ?? new();

	public override void WalkEditor(AssetWalker walker) => LoadStructure()?.WalkEditor(walker);

	public override void WalkRelease(AssetWalker walker) => LoadStructure()?.WalkRelease(walker);

	public override void WalkStandard(AssetWalker walker) => LoadStructure()?.WalkStandard(walker);

	public override IEnumerable<(string, PPtr)> FetchDependencies()
	{
		return LoadStructure()?.FetchDependencies() ?? Enumerable.Empty<(string, PPtr)>();
	}

	public override void WriteEditor(AssetWriter writer) => LoadStructure()?.WriteEditor(writer);

	public override void WriteRelease(AssetWriter writer) => LoadStructure()?.WriteRelease(writer);

	public override void CopyValues(IUnityAssetBase? source, PPtrConverter converter) => LoadStructure()?.CopyValues(source, converter);

	public override void Reset() => LoadStructure()?.Reset();
	#endregion
}
