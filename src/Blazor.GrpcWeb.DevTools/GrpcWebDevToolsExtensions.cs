using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using ProtoBuf.Grpc.Client;

namespace Blazor.GrpcWeb.DevTools
{
    public static class GrpcWebDevToolsExtensions
    {
        /// <summary>
        /// Registers a gRPC service in the servicecollection, which passes the requests and responses to the gRPC Web Developer Tools.
        /// </summary>
        /// <typeparam name="TService">gRPC Service Type</typeparam>
        /// <param name="services">IServiceCollection</param>
        public static void AddGrpcServiceWithDevTools<TService>(this IServiceCollection services)
            where TService : class
        {
            services.AddScoped(serviceProvider =>
            {
                var channel = serviceProvider.GetService<GrpcChannel>();
                var jsRuntime = serviceProvider.GetService<IJSRuntime>();
                var invoker = channel.Intercept(new GrpcMessageInterceptor(jsRuntime));
                var grpcService = GrpcClientFactory.CreateGrpcService<TService>(invoker);
                return grpcService;
            });
        }

        internal static async Task HandleGrpcRequest<TRequest, TResponse>(this IJSRuntime jsRuntime, string method, TRequest request, TResponse response)
        {
            Console.WriteLine($"HandleGrpcRequest: {method}");
            await jsRuntime.InvokeVoidAsync("postMessage", new GrpcDevToolsCall<TRequest, TResponse>("__GRPCWEB_DEVTOOLS__", method, "unary", request, response));
        }

        internal static async Task HandleGrpcServerStreamRequest<TRequest>(this IJSRuntime jsRuntime, string method, TRequest request)
        {
            Console.WriteLine($"HandleGrpcServerStreamRequest: {method}");
            await jsRuntime.InvokeVoidAsync("postMessage", new GrpcDevToolsServerRequest<TRequest>("__GRPCWEB_DEVTOOLS__", method, "server_streaming", request));
        }

        internal static async Task HandleGrpcServerStreamResponse<TResponse>(this IJSRuntime jsRuntime, string method, TResponse response)
        {
            Console.WriteLine($"HandleGrpcServerStreamResponse: {method}");
            await jsRuntime.InvokeVoidAsync("postMessage", new GrpcDevToolsServerResponse<TResponse>("__GRPCWEB_DEVTOOLS__", method, "server_streaming", response));
        }

    }
}
