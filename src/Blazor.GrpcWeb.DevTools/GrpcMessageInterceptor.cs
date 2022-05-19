using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.JSInterop;

namespace Blazor.GrpcWeb.DevTools;

public record GrpcDevToolsCall<TRequest, TRepsonse>(string type, string method, string methodType, TRequest request, TRepsonse response);
public record GrpcDevToolsServerRequest<TRequest>(string type, string method, string methodType, TRequest request);
public record GrpcDevToolsServerResponse<TRepsonse>(string type, string method, string methodType, TRepsonse response);

public partial class GrpcMessageInterceptor : Interceptor
{
    private readonly IJSRuntime _jsRuntime;
    public GrpcMessageInterceptor(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        Console.WriteLine($"Starting call. Type: {context.Method.Type}. " +
                          $"Method: {context.Method.Name}.");

        var call = continuation(request, context);

        return new AsyncUnaryCall<TResponse>(
            HandleUnaryCall(context.Method.Name, context.Method.Type.ToString(), request, call.ResponseAsync),
            call.ResponseHeadersAsync,
            call.GetStatus,
            call.GetTrailers,
            call.Dispose);
    }

    public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        var streamingCall = base.AsyncServerStreamingCall(request, context, continuation);

        var response = new AsyncServerStreamingCall<TResponse>(
            new AsyncStreamReaderWrapper<TResponse>(streamingCall.ResponseStream, context.Method.Name, _jsRuntime),
            HandleServerStreamRequest(streamingCall.ResponseHeadersAsync, request, context.Method.Name),
            streamingCall.GetStatus, streamingCall.GetTrailers, streamingCall.Dispose);

        return response;
    }

    private async Task<Metadata> HandleServerStreamRequest<TRequest>(Task<Metadata> metaData, TRequest request, string method)
    {
        try
        {
            var result = await metaData;
            await _jsRuntime.HandleGrpcServerStreamRequest(method, request);
            return result;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Custom error", ex);
        }
    }

    private async Task<TResponse> HandleUnaryCall<TResponse, TRequest>(string method, string methodType, TRequest request, Task<TResponse> inner)
    {
        try
        {
            var result = await inner;
            await _jsRuntime.HandleGrpcRequest(method, methodType, request, result);
            return result;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Custom error", ex);
        }
    }
}