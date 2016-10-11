using DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class UploadRepository<T> : IUploadRepository<T> where T : BaseEntity
    {
        private readonly IDbContext _context;
        private IDbSet<T> _entities;

        public UploadRepository(IDbContext context)
        {
            this._context = context;
        }

        public void Upload(List<T> entities)
        {
            foreach (var entity in entities)
            {
                Entities.Add(entity);
            }
            _context.SaveChanges();
        }

        private IDbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = _context.Set<T>();
                }
                return _entities;
            }
        }

    }
}
