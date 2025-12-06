using AutoMapper;
using Grpc.Core;
using UserService.Domain.Interfaces.Service;

namespace UserService.Grpc.Services;

public class GrpcUserService(IGetUserService userService, IGetRoleService roleService, IMapper mapper)
    : UserService.UserServiceBase
{
    public override async Task<GrpcUser> GetUserWithRolesById(GetUserByIdRequest request, ServerCallContext context)
    {
        var userResult = await userService.GetByIdsAsync([request.UserId], context.CancellationToken);
        if (!userResult.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, userResult.ErrorMessage!),
                new Metadata { { nameof(userResult.ErrorCode), userResult.ErrorCode?.ToString() ?? string.Empty } });

        var rolesResult = await roleService.GetUsersRolesAsync([request.UserId], context.CancellationToken);
        if (!rolesResult.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, rolesResult.ErrorMessage!),
                new Metadata { { nameof(rolesResult.ErrorCode), rolesResult.ErrorCode?.ToString() ?? string.Empty } });

        var currentReputation =
            await userService.GetCurrentReputationsAsync([request.UserId], context.CancellationToken);
        if (!currentReputation.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, currentReputation.ErrorMessage!),
                new Metadata
                {
                    { nameof(currentReputation.ErrorCode), currentReputation.ErrorCode?.ToString() ?? string.Empty }
                });

        var grpcUser = mapper.Map<GrpcUser>(userResult.Data.Single());
        grpcUser.Roles.AddRange(rolesResult.Data.Single().Value.Select(mapper.Map<GrpcRole>));
        grpcUser.Reputation = currentReputation.Data.Single().Value;

        return grpcUser;
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