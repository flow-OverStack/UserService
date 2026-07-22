using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Mocks;

namespace UserService.Tests.UnitTests.Sut;

internal class GetReputationRecordServiceSut
{
    private readonly IGetReputationRecordService _getReputationRecordService;

    public readonly IBaseRepository<ReputationRecord> ReputationRecordRepository =
        RepositoryMocks.GetMockReputationRecordRepository().Object;


    public GetReputationRecordServiceSut()
    {
        _getReputationRecordService = new GetReputationRecordService(ReputationRecordRepository);
    }

    public IGetReputationRecordService GetService()
    {
        return _getReputationRecordService;
    }
}
