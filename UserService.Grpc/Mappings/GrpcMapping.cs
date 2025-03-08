using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace UserService.Grpc.Mappings;

public class GrpcMapping : Profile
{
    public GrpcMapping()
    {
        CreateMap<DateTime, Timestamp>()
            .ConvertUsing(x =>
                Timestamp.FromDateTime(DateTime.SpecifyKind(x, DateTimeKind.Utc)));

        CreateMap<Timestamp, DateTime>().ConvertUsing(x => x.ToDateTime());
    }
}