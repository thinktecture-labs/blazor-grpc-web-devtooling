using ProtoBuf.Grpc;
using System.ServiceModel;

namespace Blazor.GrpcDevTools.Shared.Services;

[ServiceContract]
public interface ITimeService
{
    [OperationContract]
    IAsyncEnumerable<string> SubscribeAsync(CallContext context = default);
}