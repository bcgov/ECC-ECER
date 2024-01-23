using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System.Text;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace ECER.Utilities.DataverseSdk.Model;

public static class ExtensionMethods
{
    public static void AddLink([NotNull] this EcerContext context, Entity source, string relationshipName, Entity target)
    {
        context.AddLink(source, new Relationship(relationshipName), target);
    }

    public static void AddRelatedObject([NotNull] this EcerContext context, Entity source, string relationshipName, Entity target)
    {
        context.AddRelatedObject(source, new Relationship(relationshipName), target);
    }

    private const int FileBlockSize = 4 * 1024 * 1024; // 4 MB

    public static async Task<string?> UploadFile([NotNull] this EcerContext context, [NotNull] Entity entity, string fileFieldName, [NotNull] FileContainer file, CancellationToken ct = default)
    {
        var response = (InitializeFileBlocksUploadResponse)context.Execute(new InitializeFileBlocksUploadRequest
        {
            Target = new EntityReference(entity.LogicalName, entity.Id),
            FileAttributeName = fileFieldName,
            FileName = file.FileName
        });

        var fileContinuationToken = response.FileContinuationToken;
        int blockNumber = 0;

        var slices = new List<ReadOnlyMemory<byte>>();
        int sliceIndex = 0;
        while (sliceIndex < file.Content.Length)
        {
            var left = file.Content.Length - sliceIndex;
            var slice = file.Content.Slice(sliceIndex, left > FileBlockSize ? FileBlockSize : left);
            slices.Add(slice);
            blockNumber++;
            sliceIndex += slice.Length;
        }

        var blockIds = new ConcurrentBag<string>();

        try
        {
            Parallel.ForEach(slices, new ParallelOptions { CancellationToken = ct }, slice =>
            {
                string blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));

                blockIds.Add(blockId);

                context.Execute(new UploadBlockRequest()
                {
                    BlockData = slice.ToArray(),
                    BlockId = blockId,
                    FileContinuationToken = fileContinuationToken,
                });
            });
        }
        catch (OperationCanceledException)
        {
            return null;
        }

        var commitFileBlocksUploadResponse = (CommitFileBlocksUploadResponse)context.Execute(new CommitFileBlocksUploadRequest
        {
            BlockList = blockIds.ToArray(),
            FileContinuationToken = fileContinuationToken,
            FileName = file.FileName,
            MimeType = file.MimeType
        });

        return await Task.FromResult(commitFileBlocksUploadResponse.FileId.ToString());
    }

    private static RetrieveAttributeResponse GetAttribute([NotNull] this EcerContext context, [NotNull] Entity entity, string fileFieldName)
    {
        return (RetrieveAttributeResponse)context.Execute(new RetrieveAttributeRequest
        {
            EntityLogicalName = entity.LogicalName,
            LogicalName = fileFieldName,
        });
    }

    public static async Task<FileContainer?> DownloadFile([NotNull] this EcerContext context, [NotNull] Entity entity, string fileFieldName, CancellationToken ct = default)
    {
        InitializeFileBlocksDownloadResponse response;
        try
        {
            response = (InitializeFileBlocksDownloadResponse)context.Execute(new InitializeFileBlocksDownloadRequest

            {
                Target = new EntityReference(entity.LogicalName, entity.Id),
                FileAttributeName = fileFieldName,
            });
        }
        catch (FaultException<OrganizationServiceFault> ex) when (ex.Message.StartsWith("No file attachment found"))
        {
            throw new FileNotFoundException($"File not found", ex);
        }

        var offset = 0;

        using var ms = new MemoryStream();
        while (offset < response.FileSizeInBytes)
        {
            if (ct.IsCancellationRequested) return null;
            var dlResponse = (DownloadBlockResponse)context.Execute(new DownloadBlockRequest
            {
                FileContinuationToken = response.FileContinuationToken,
                BlockLength = FileBlockSize,
                Offset = offset
            });
            ms.Write(dlResponse.Data);
            offset += dlResponse.Data.Length;
        }

        return await Task.FromResult(new FileContainer(response.FileName, string.Empty, new ReadOnlyMemory<byte>(ms.ToArray())));
    }

    public static async Task DeleteFile([NotNull] this EcerContext context, [NotNull] Entity entity, string fileFieldName, CancellationToken ct = default)
    {
        await Task.CompletedTask;

        var isImage = context.GetAttribute(entity, fileFieldName).AttributeMetadata is ImageAttributeMetadata;

        if (isImage)
        {
            entity.Attributes[fileFieldName] = null;
            context.UpdateObject(entity);
            context.SaveChanges();
        }
        else
        {
            if (!Guid.TryParse(entity[fileFieldName]?.ToString() ?? string.Empty, out var fileId)) throw new InvalidOperationException($"Cannot find file id in entity {entity.LogicalName}.{fileId} with id {entity.Id}");

            DeleteFileRequest deleteFileRequest = new()
            {
                FileId = fileId
            };

            context.Execute(deleteFileRequest);
        }
    }
}

public record FileContainer(string FileName, string MimeType, ReadOnlyMemory<byte> Content);