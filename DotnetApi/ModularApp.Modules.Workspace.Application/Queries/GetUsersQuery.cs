using MediatR;
using Microsoft.EntityFrameworkCore;
using ModularApp.Modules.Workspace.Application.Common;
using ModularApp.Modules.Workspace.Application.Models;
using ModularApp.Modules.Workspace.Domain.Entities;

namespace ModularApp.Modules.Workspace.Application.Queries;

public record GetUsersQuery : IRequest<UserModel2>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, UserModel2>
{
    private readonly IRepository _repository;
    // private readonly IMapper _mapper;


    public GetUsersQueryHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserModel2> Handle(GetUsersQuery request, CancellationToken ct)
    {
        var users = await _repository
            .SecureWithNoTracking<User>()
            .ToListAsync(ct);

        return users.Select(x => new UserModel2(x.UserName)).FirstOrDefault();
    }
}