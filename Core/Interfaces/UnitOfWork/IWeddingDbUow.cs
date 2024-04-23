using Core.Interfaces.Repository;
using Data.Entities;

namespace Core.Interfaces.UnitOfWork
{
    public interface IWeddingDbUow : IDisposable
    {
        public IGenericRepository<Family> Family { get; }
        public IGenericRepository<FamilyMember> FamilyMember { get; }
        Task SaveAsync();
        void Save();
    }
}
