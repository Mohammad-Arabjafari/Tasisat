using Frosh.Core.Entities;
using Frosh.Core.Interface;
using Frosh.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace Frosh.Data.SqlServer.Repository;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Project> GetAll()
    {
        return _context.Projects
            .AsNoTracking()
            .ToList();
    }

    public Project? GetById(Guid id)
    {
        return _context.Projects
            .AsNoTracking()
            .FirstOrDefault(p => p.Id == id);
    }

    public void Add(Project entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Projects.Add(entity);
        _context.SaveChanges();
    }

    public bool Update(Project entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var existingProject = _context.Projects.Find(entity.Id);

        if (existingProject is null)
            return false;

        _context.Entry(existingProject).CurrentValues.SetValues(entity);

        _context.SaveChanges();

        return true;
    }

    public bool Delete(Guid id)
    {
        var project = _context.Projects.Find(id);

        if (project is null)
            return false;

        _context.Projects.Remove(project);
        _context.SaveChanges();

        return true;
    }
}