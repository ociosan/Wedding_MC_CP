using Data.Entities;
using Data;
using System;
using Core.Interfaces.Repository;
using Core.Repository;
using Core.Interfaces.UnitOfWork;

namespace Core.UnitOfWork
{
    public class WeddingDbUow : IWeddingDbUow
    {
        private readonly WeddingDBContext _weddingDBContext;
        private IGenericRepository<Family>? _family;
        private IGenericRepository<FamilyMember>? _familyMember;

        public WeddingDbUow(WeddingDBContext weddingDBContext)
        {
            _weddingDBContext = weddingDBContext;
        }

        public IGenericRepository<Family> Family => _family ??= new GenericRepository<Family>(_weddingDBContext);
        public IGenericRepository<FamilyMember> FamilyMember => _familyMember ??= new GenericRepository<FamilyMember>(_weddingDBContext);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool dispose)
        {
            if (dispose)
                _weddingDBContext.Dispose();
        }

        public async Task SaveAsync()
        {
            await _weddingDBContext.SaveChangesAsync();
        }

        public void Save()
        {
            _weddingDBContext.SaveChanges();
        }

    }
}
