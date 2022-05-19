const postType = "__GRPCWEB_DEVTOOLS__";

export function sendGrpcCall(method, methodType, request, response) {
    window.postMessage({
        type: postType,
        method,
        methodType: "unary",
        request,
        response
    });
}