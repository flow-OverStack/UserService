using AutoMapper;
using Grpc.Core;
using UserService.Domain.Interfaces.Services;

namespace UserService.Grpc.Services;

public class GrpcUserService(IGetUserService userService, IMapper mapper) : UserService.UserServiceBase
{
    public override async Task<GrpcUser> GetUserById(GetUserByIdRequest request, ServerCallContext context)
    {
        var result = await userService.GetByIdAsync(request.UserId);

        if (!result.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, result.ErrorMessage!),
                new Metadata { { nameof(result.ErrorCode), result.ErrorCode.ToString()! } });

        return mapper.Map<GrpcUser>(result.Data);
    }
}