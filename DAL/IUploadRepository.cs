using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IUploadRepository<T> where T : BaseEntity
    {
        void Upload(List<T> entities);
    }
}
