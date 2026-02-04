using AutoMapper;
using Grpc.Core;
using UserService.Application.Enums;
using UserService.Application.Resources;
using UserService.Domain.Interfaces.Service;

namespace UserService.GrpcServer.Services;

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
        // Not possible, because roles are fetched for every user that exists
        if (!rolesResult.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, rolesResult.ErrorMessage!),
                new Metadata { { nameof(rolesResult.ErrorCode), rolesResult.ErrorCode?.ToString() ?? string.Empty } });

        var currentReputation =
            await userService.GetCurrentReputationsAsync([request.UserId], context.CancellationToken);
        // Not possible, because reputations are fetched for every user that exists
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
        var users = await userService.GetByIdsAsync(request.UserIds);
        if (!users.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, users.ErrorMessage!),
                new Metadata { { nameof(users.ErrorCode), users.ErrorCode?.ToString() ?? string.Empty } });

        var currentReputations =
            await userService.GetCurrentReputationsAsync(request.UserIds, context.CancellationToken);
        if (!currentReputations.IsSuccess)
            // Not possible, because reputations are fetched for every user that exists
            throw new RpcException(new Status(StatusCode.InvalidArgument, currentReputations.ErrorMessage!),
                new Metadata
                {
                    { nameof(currentReputations.ErrorCode), currentReputations.ErrorCode?.ToString() ?? string.Empty }
                });

        var response = new GetUsersByIdsResponse();
        response.Users.AddRange(users.Data.Select(mapper.Map<GrpcUser>));

        var reputationsDict = currentReputations.Data.ToDictionary(x => x.Key, x => x.Value);
        foreach (var user in response.Users)
            if (reputationsDict.TryGetValue(user.Id, out var reputation)) user.Reputation = reputation;
            // Not possible, because reputations are fetched for every user that exists
            else
                throw new RpcException(new Status(StatusCode.InvalidArgument, ErrorMessage.UsersNotFound),
                    new Metadata { { nameof(currentReputations.ErrorCode), nameof(ErrorCodes.UsersNotFound) } });

        return response;
    }
}