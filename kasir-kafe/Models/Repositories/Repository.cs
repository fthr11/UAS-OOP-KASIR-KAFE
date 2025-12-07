using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using kasirkafe.Models;               // <- sesuaikan dengan namespace CafeDbContext kamu
using kasirkafe.Models.Interfaces;
using kasirkafe.Data;

namespace kasirkafe.Models.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private CafeDbContext _context;
    private DbSet<T> _dbSet;

    public Repository(CafeDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IEnumerable<T> GetAll()
    {
        return _dbSet.ToList();
    }

    public T? GetById(int id)
    {
        return _dbSet.Find(id);
    }

    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public void Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(int id)
    {
        var entity = _dbSet.Find(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public void Save()
    {
        _context.SaveChanges();
    }
}