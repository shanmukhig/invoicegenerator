using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using InvoiceGenerator.Entities;

namespace InvoiceGenerator.Data
{
  public interface IRepository<T> where T : BaseEntity
  {
    Task<IEnumerable<T>> GetAll();
    Task<T> GetById(string id);
    Task<T> AddOrUpdate(string id, T customer);
    Task<bool> Delete(string id);
    Task<string> UploadFileAsync(string id, string fileName, Stream stream = null, byte[] buffer = null);
    Task<byte[]> DownloadFileAsync(string id);
  }
}