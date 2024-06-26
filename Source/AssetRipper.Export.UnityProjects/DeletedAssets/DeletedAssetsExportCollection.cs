﻿using AssetRipper.Assets;
using AssetRipper.Assets.Collections;
using AssetRipper.Assets.Export;
using AssetRipper.Assets.Metadata;
using AssetRipper.IO.Files.SerializedFiles;
using AssetRipper.Processing;

namespace AssetRipper.Export.UnityProjects.DeletedAssets;

public sealed record DeletedAssetsExportCollection(DeletedAssetsInformation Asset) : IExportCollection
{
	bool IExportCollection.Exportable => false;

	AssetCollection IExportCollection.File => Asset.Collection;

	TransferInstructionFlags IExportCollection.Flags => Asset.Collection.Flags;

	IEnumerable<IUnityObjectBase> IExportCollection.Assets => Asset.DeletedAssets.Append(Asset);

	string IExportCollection.Name => nameof(DeletedAssetsExportCollection);

	bool IExportCollection.Contains(IUnityObjectBase asset)
	{
		return ReferenceEquals(Asset, asset) || Asset.DeletedAssets.Contains(asset);
	}

	MetaPtr IExportCollection.CreateExportPointer(IExportContainer container, IUnityObjectBase asset, bool isLocal)
	{
		throw new NotSupportedException();
	}

	bool IExportCollection.Export(IExportContainer container, string projectDirectory)
	{
		throw new NotSupportedException();
	}

	long IExportCollection.GetExportID(IExportContainer container, IUnityObjectBase asset)
	{
		throw new NotSupportedException();
	}
}
