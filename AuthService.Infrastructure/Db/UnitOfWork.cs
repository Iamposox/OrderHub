using AuthService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Db;
public class UnitOfWork : IUnitOfWork
{
    private readonly AuthDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;

    private IUserRepository? _userRepository;
    private IRoleRepository? _roleRepository;

    private bool _disposed = false;
    private bool _transactionStarted = false;
    public UnitOfWork(
        AuthDbContext authDbContext,
        ILogger<UnitOfWork> logger
        ) 
    {
        _context = authDbContext;
        _logger = logger;
    }

    public IUserRepository Users => _userRepository ??= new UserRepository(_context);

    public IRoleRepository Roles => _roleRepository ??= new RoleRepository(_context);

    public async Task BeginTransactionAsync()
    {
        if (_transactionStarted)
            throw new InvalidOperationException("Транзакция уже запушена");
        await _context.BeginTransactionAsync();
        _transactionStarted = true;
    }

    public async Task CommitTransactionAsync()
    {
        if (!_transactionStarted)
            throw new InvalidOperationException("Нет активной транзакции для коммита.");
        await _context.SaveChangesAsync();
        await _context.CommitTransactionAsync();
        _transactionStarted = false;
    }
    public async Task RollbackTransactionAsync()
    {
        if (!_transactionStarted)
            throw new InvalidOperationException("Нет активной транзакции для отката.");
        await _context.RollbackTransactionAsync();
        _transactionStarted = false;
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (_transactionStarted)
                {
                    _logger.LogWarning("UnitOfWork был уничтожен, но транзакция не завершена. Выполняется откат.");
                    _context.Database.RollbackTransaction();
                }
                _context.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

}
