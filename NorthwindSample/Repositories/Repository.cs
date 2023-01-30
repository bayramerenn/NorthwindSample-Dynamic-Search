using Microsoft.EntityFrameworkCore;
using NorthwindSample.Attribute;
using NorthwindSample.Encrypt;
using NorthwindSample.Models;

namespace NorthwindSample.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly NorthwindContext _context;
        private DbSet<T> _entities;
        private readonly IEncryption _encryption;
        public Repository(NorthwindContext context, IEncryption encryption = null)
        {
            _context = context;
            _entities = context.Set<T>();
            _encryption = encryption;
        }

        public virtual T GetById(object id)
        {
            var entity = Entities.Find(id);
            System.ComponentModel.DataAnnotations.MetadataTypeAttribute[] metadataTypes = entity.GetType().GetCustomAttributes(true).OfType<System.ComponentModel.DataAnnotations.MetadataTypeAttribute>().ToArray();
            foreach (System.ComponentModel.DataAnnotations.MetadataTypeAttribute metadata in metadataTypes)
            {
                System.Reflection.PropertyInfo[] properties = metadata.MetadataClassType.GetProperties();
                //Metadata atanmış entity'nin tüm propertyleri tek tek alınır.
                foreach (System.Reflection.PropertyInfo pi in properties)
                {
                    //Eğer ilgili property ait CryptoData flag'i var ise ilgili deger encrypt edilir. 
                    if (System.Attribute.IsDefined(pi, typeof(CryptoDataAttribute)))
                    {
                        _context.Entry(entity).Property(pi.Name).CurrentValue = _encryption.DecryptText(_context.Entry(entity).Property(pi.Name).CurrentValue.ToString());
                    }
                }
            }

            return entity;
        }

        public void Insert(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            //_context.Entry(entity).Property("UsedTime").CurrentValue = DateTime.Now;
            //---------------

            System.ComponentModel.DataAnnotations.MetadataTypeAttribute[] metadataTypes = entity.GetType().GetCustomAttributes(true).OfType<System.ComponentModel.DataAnnotations.MetadataTypeAttribute>().ToArray();
            foreach (System.ComponentModel.DataAnnotations.MetadataTypeAttribute metadata in metadataTypes)
            {
                System.Reflection.PropertyInfo[] properties = metadata.MetadataClassType.GetProperties();
                //Metadata atanmış entity'nin tüm propertyleri tek tek alınır.
                foreach (System.Reflection.PropertyInfo pi in properties)
                {
                    //Eğer ilgili property ait CryptoData flag'i var ise ilgili deger encrypt edilir. 
                    if (System.Attribute.IsDefined(pi, typeof(CryptoDataAttribute)))
                    {
                        _context.Entry(entity).Property(pi.Name).CurrentValue = _encryption.EncryptText(_context.Entry(entity).Property(pi.Name).CurrentValue.ToString());
                    }
                }
            }
            _entities.Add(entity);
            _context.SaveChanges();
        }

        public virtual void Insert(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
                Entities.Add(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            _context.SaveChanges();
        }

        public virtual void Update(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            _context.SaveChanges();
        }

        public void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            Entities.Remove(entity);
            _context.SaveChanges();
        }

        public virtual void Delete(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
                Entities.Remove(entity);
            _context.SaveChanges();
        }

        public IEnumerable<T> GetSql(string sql)
        {
            return Entities.FromSqlRaw(sql);
        }

        public virtual IQueryable<T> Table => Entities;

        public virtual IQueryable<T> TableNoTracking => Entities.AsNoTracking();

        protected virtual DbSet<T> Entities => _entities ?? (_entities = _context.Set<T>());
    }
}
