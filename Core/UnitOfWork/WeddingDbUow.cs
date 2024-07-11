using Data.Entities;
using Data;
using Core.Interfaces.Repository;
using Core.Repository;
using Core.Interfaces.UnitOfWork;
using Core.Interfaces.Helper;

namespace Core.UnitOfWork
{
    public class WeddingDbUow : IWeddingDbUow
    {
        private readonly IDapperDbHelper _dapperDbHelper;

        private IGenericRepository<Family>? _family;
        private IGenericRepository<FamilyMember>? _familyMember;
        private IGenericRepository<Email>? _email;
        private IGenericRepository<WhatsApp>? _whatsApp;

        public WeddingDbUow(IDapperDbHelper dapperDbHelper)
        {
            _dapperDbHelper = dapperDbHelper;
        }

        public IGenericRepository<Family> Family => _family ??= new GenericRepository<Family>(_dapperDbHelper);
        public IGenericRepository<FamilyMember> FamilyMember => _familyMember ??= new GenericRepository<FamilyMember>(_dapperDbHelper);
        public IGenericRepository<Email> Email => _email ??= new GenericRepository<Email>(_dapperDbHelper);
        public IGenericRepository<WhatsApp> WhatsApp => _whatsApp ??= new GenericRepository<WhatsApp>(_dapperDbHelper);



    }
}
