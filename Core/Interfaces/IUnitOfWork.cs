using Core.Interfaces.Repository;
using Data.Entities;

namespace Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IGenericRepository<Family> Family { get; }
        public IGenericRepository<FamilyMember> FamilyMember { get; }
        Task SaveAsync();
        void Save();
    }
}
