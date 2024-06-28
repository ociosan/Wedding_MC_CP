using Data.Entities;
using Data;
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
        private IGenericRepository<Email>? _email;
        private IGenericRepository<WhatsApp>? _whatsApp;

        public WeddingDbUow(WeddingDBContext weddingDBContext)
        {
            _weddingDBContext = weddingDBContext;
        }

        public IGenericRepository<Family> Family => _family ??= new GenericRepository<Family>(_weddingDBContext);
        public IGenericRepository<FamilyMember> FamilyMember => _familyMember ??= new GenericRepository<FamilyMember>(_weddingDBContext);
        public IGenericRepository<Email> Email => _email ??= new GenericRepository<Email>(_weddingDBContext);
        public IGenericRepository<WhatsApp> WhatsApp => _whatsApp ??= new GenericRepository<WhatsApp>(_weddingDBContext);

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
