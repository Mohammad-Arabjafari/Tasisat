
using Frosh.Core.Common;
using Frosh.Core.Entities;
using Frosh.Core.Interface;
namespace Frosh.Data.InMemori.Repository;

public class ProjectRepository : IProjectRepository
{
    void IRepository<Project, Guid>.Add(Project entity)
    {
        throw new NotImplementedException();
    }

    bool IRepository<Project, Guid>.Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    IEnumerable<Project> IRepository<Project, Guid>.GetAll()
    {
        throw new NotImplementedException();
    }

    Project? IRepository<Project, Guid>.GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    bool IRepository<Project, Guid>.Update(Project entity)
    {
        throw new NotImplementedException();
    }
}

