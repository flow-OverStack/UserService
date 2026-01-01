using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Configurations;

namespace UserService.Tests.UnitTests.Factories;

internal class GetReputationRecordServiceFactory
{
    private readonly IGetReputationRecordService _getReputationRecordService;

    public readonly IBaseRepository<ReputationRecord> ReputationRecordRepository =
        MockRepositoriesGetters.GetMockReputationRecordRepository().Object;


    public GetReputationRecordServiceFactory()
    {
        _getReputationRecordService = new GetReputationRecordService(ReputationRecordRepository);
    }

    public IGetReputationRecordService GetService()
    {
        return _getReputationRecordService;
    }
}