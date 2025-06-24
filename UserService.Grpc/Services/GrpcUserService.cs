using AutoMapper;
using Grpc.Core;
using UserService.Domain.Interfaces.Service;

namespace UserService.Grpc.Services;

public class GrpcUserService(IGetUserService userService, IMapper mapper) : UserService.UserServiceBase
{
    public override async Task<GrpcUser> GetUserWithRolesById(GetUserByIdRequest request, ServerCallContext context)
    {
        var result = await userService.GetByIdWithRolesAsync(request.UserId);

        if (!result.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, result.ErrorMessage!),
                new Metadata { { nameof(result.ErrorCode), result.ErrorCode?.ToString() ?? string.Empty } });

        return mapper.Map<GrpcUser>(result.Data);
    }

    public override async Task<GetUsersByIdsResponse> GetUsersByIds(GetUsersByIdsRequest request,
        ServerCallContext context)
    {
        var result = await userService.GetByIdsAsync(request.UserIds);

        if (!result.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, result.ErrorMessage!),
                new Metadata { { nameof(result.ErrorCode), result.ErrorCode?.ToString() ?? string.Empty } });

        var response = new GetUsersByIdsResponse();
        response.Users.AddRange(result.Data.Select(mapper.Map<GrpcUser>));

        return response;
    }
}