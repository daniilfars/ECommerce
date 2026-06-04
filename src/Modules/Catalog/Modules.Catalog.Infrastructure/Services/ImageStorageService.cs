using Minio;
using Minio.DataModel.Args;
using Modules.Catalog.Application.Interfaces;

namespace Modules.Catalog.Infrastructure.Services;

public class ImageStorageService : IImageStorageService
{
    private readonly IMinioClient _minio;
    private readonly string _bucketName;
    private readonly string _endpoint;

    public ImageStorageService(IMinioClient minio, string bucketName, string endpoint)
    {
        _minio = minio;
        _bucketName = bucketName;
        _endpoint = endpoint;
    }

    public async Task DeleteAsync(string objectName, CancellationToken cancellationToken = default)
    {
        var removeObjectArgs = new RemoveObjectArgs().WithBucket(_bucketName).WithObject(objectName);

        await _minio.RemoveObjectAsync(removeObjectArgs, cancellationToken);
    }

    public string GetPublicUrl(string objectName)
    {
        return $"{_endpoint}/{_bucketName}/{objectName}";
    }

    public async Task<string> UploadAsync(string objectName, Stream data, string contentType, CancellationToken cancellationToken = default)
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(_bucketName);
        bool found = await _minio.BucketExistsAsync(bucketExistsArgs, cancellationToken);

        if(!found)
        {
            var makeBucketArgs = new MakeBucketArgs().WithBucket(_bucketName);
            await _minio.MakeBucketAsync(makeBucketArgs, cancellationToken);
        }

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithStreamData(data)
            .WithObjectSize(data.Length)
            .WithContentType(contentType);

        await _minio.PutObjectAsync(putObjectArgs, cancellationToken);

        return GetPublicUrl(objectName);
    }
}