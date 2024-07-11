using Core.Interfaces.Repository;
using Data.Entities;

namespace Core.Interfaces.UnitOfWork
{
    public interface IWeddingDbUow
    {
        public IGenericRepository<Family> Family { get; }
        public IGenericRepository<FamilyMember> FamilyMember { get; }
        public IGenericRepository<Email> Email { get; }
        public IGenericRepository<WhatsApp> WhatsApp { get; }
    }
}
