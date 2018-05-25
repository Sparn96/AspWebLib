using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MitchellMarshClassLibrary.Database.Services
{
    public interface IUserService<TDataModel, TUserKey>
    {
        /// <summary>
        /// EF Core Add To Database
        /// </summary>
        /// <param name="add">Entity to add</param>
        /// <param name="SaveAfterExecute">Calls SaveChanges() on the current DB context when true</param>
        /// <returns></returns>
        TDataModel Add(TDataModel add, TUserKey UserId, bool SaveAfterExecute = true);
        /// <summary>
        /// Add a range of entites to the database at once
        /// </summary>
        /// <param name="addRange">range of entities</param>
        /// <param name="SaveAfterExecute">Calls SaveChanges() on the current DB context when true</param>
        void Add(List<TDataModel> addRange, TUserKey UserId, bool SaveAfterExecute = true);
        /// <summary>
        /// EF Core Update in Databse
        /// </summary>
        /// <param name="update">entity to update</param>
        /// <param name="SaveAfterExecute">Calls SaveChanges() on the current DB context when true</param>
        /// <returns></returns>
        TDataModel Update(TDataModel update, TUserKey UserId, bool SaveAfterExecute = true);
        /// <summary>
        /// Update a range of entities in database
        /// </summary>
        /// <param name="updateRange">list of entities to update</param>
        /// <param name="SaveAfterExecute">Calls SaveChanges() on the current DB context when true</param>
        void Update(List<TDataModel> updateRange, TUserKey UserId, bool SaveAfterExecute = true);



        TDataModel Activate(TDataModel activate, TUserKey UserId, bool SaveAfterExecute = true);
        void Activate(List<TDataModel> activateRange, TUserKey UserId, bool SaveAfterExecute = true);

        TDataModel Deactivate(TDataModel deactivate, TUserKey UserId, bool SaveAfterExecute = true);
        void Deactivate(List<TDataModel> deactivateRange, TUserKey UserId, bool SaveAfterExecute = true);

        /// <summary>
        /// Will flag the delete flag as true. Does not remove records from the Database
        /// </summary>
        /// <param name="delete"></param>
        /// <param name="SaveAfterExecute">Calls SaveChanges() on the current DB context when true</param>
        /// <returns></returns>
        TDataModel Delete(TDataModel delete, TUserKey UserId, bool SaveAfterExecute = true);
        void Delete(List<TDataModel> deleteRange, TUserKey UserId, bool SaveAfterExecute = true);
    }

    public interface IBasicService<TDataModel>
    {
        /// <summary>
        /// EF Core Add To Database
        /// </summary>
        /// <param name="add">Entity to add</param>
        /// <param name="SaveAfterExecute">Calls SaveChanges() on the current DB context when true</param>
        /// <returns></returns>
        TDataModel Add(TDataModel add, bool SaveAfterExecute = true);
        /// <summary>
        /// Add a range of entites to the database at once
        /// </summary>
        /// <param name="addRange">range of entities</param>
        /// <param name="SaveAfterExecute">Calls SaveChanges() on the current DB context when true</param>
        void Add(List<TDataModel> addRange, bool SaveAfterExecute = true);
        /// <summary>
        /// EF Core Update in Databse
        /// </summary>
        /// <param name="update">entity to update</param>
        /// <param name="SaveAfterExecute">Calls SaveChanges() on the current DB context when true</param>
        /// <returns></returns>
        TDataModel Update(TDataModel update, bool SaveAfterExecute = true);
        /// <summary>
        /// Update a range of entities in database
        /// </summary>
        /// <param name="updateRange">list of entities to update</param>
        /// <param name="SaveAfterExecute">Calls SaveChanges() on the current DB context when true</param>
        void Update(List<TDataModel> updateRange, bool SaveAfterExecute = true);

        TDataModel Save(TDataModel save, bool SaveAfterExecute = true);
        List<TDataModel> Save(List<TDataModel> save_list, bool SaveAfterExecute = true);

        TDataModel Activate(TDataModel activate, bool SaveAfterExecute = true);
        void Activate(List<TDataModel> activateRange, bool SaveAfterExecute = true);

        TDataModel Deactivate(TDataModel deactivate, bool SaveAfterExecute = true);
        void Deactivate(List<TDataModel> deactivateRange, bool SaveAfterExecute = true);

        /// <summary>
        /// Will flag the delete flag as true. Does not remove records from the Database
        /// </summary>
        /// <param name="delete"></param>
        /// <param name="SaveAfterExecute">Calls SaveChanges() on the current DB context when true</param>
        /// <returns></returns>
        TDataModel Delete(TDataModel delete, bool SaveAfterExecute = true);
        void Delete(List<TDataModel> deleteRange, bool SaveAfterExecute = true);


        int SaveChanges();
        /// <summary>
        /// Specialized saving changes to large sets of records
        /// </summary>
        /// <returns></returns>
        int BulkSaveChanges();
        void UndoChangesDbContext();
        void UndoChangesEntity(TDataModel entity);

        TDataModel Get(params object[] primary_key);

        IEnumerable<TDataModel> GetAll();
        IEnumerable<TDataModel> GetAll(Expression<Func<TDataModel, bool>> predicate);
        IEnumerable<TDataModel> GetAllActive();

    }

    public class BasicServiceImplementation<TDataModel, TDbContext> :
        IBasicService<TDataModel>
        where TDataModel : DataRowProperties
        where TDbContext : DbContext
    {
        TDbContext _context;

        public BasicServiceImplementation(TDbContext ctx)
        {
            _context = ctx;
        }

        public TDataModel Activate(TDataModel activate, bool SaveAfterExecute = true)
        {
            activate.IsActive = true;
            activate.DateInactivatedUtc = null;

            return Update(activate, SaveAfterExecute);
        }

        public void Activate(List<TDataModel> activateRange, bool SaveAfterExecute = true)
        {
            activateRange.ForEach(e =>
            {
                e.IsActive = true;
                e.DateInactivatedUtc = null;
            });
            Update(activateRange, SaveAfterExecute);
            return;
        }

        public TDataModel Add(TDataModel add, bool SaveAfterExecute = true)
        {
            add.DateCreatedUtc = DateTime.UtcNow;
            add.DateLastModifiedUtc = add.DateCreatedUtc;
            add.IsActive = true;
            add.IsDeleted = false;

            var Ent = _context.Add(add);
            if (SaveAfterExecute) SaveChanges();
            return Ent.Entity;
        }

        public void Add(List<TDataModel> addRange, bool SaveAfterExecute = true)
        {
            DateTime UTC = DateTime.UtcNow;
            addRange.ForEach(e =>
            {
                e.DateCreatedUtc = UTC;
                e.DateLastModifiedUtc = UTC;
                e.IsActive = true;
                e.IsDeleted = true;
            });
            _context.AddRange(addRange);
            if (SaveAfterExecute) SaveChanges();
            return;
        }

        public int BulkSaveChanges()
        {

            throw new NotImplementedException();
        }

        public TDataModel Deactivate(TDataModel deactivate, bool SaveAfterExecute = true)
        {
            deactivate.IsActive = false;
            deactivate.DateInactivatedUtc = DateTime.UtcNow;

            return Update(deactivate, SaveAfterExecute);
        }

        public void Deactivate(List<TDataModel> deactivateRange, bool SaveAfterExecute = true)
        {
            deactivateRange.ForEach(e =>
            {
                e.IsActive = false;
                e.DateInactivatedUtc = DateTime.UtcNow;
            });

            Update(deactivateRange, SaveAfterExecute);
            return;
        }

        public TDataModel Delete(TDataModel delete, bool SaveAfterExecute = true)
        {
            delete.IsDeleted = true;
            delete.DateFlaggedForDeletionUtc = DateTime.UtcNow;

            return Update(delete, SaveAfterExecute);
        }

        public void Delete(List<TDataModel> deleteRange, bool SaveAfterExecute = true)
        {
            deleteRange.ForEach(e =>
            {
                e.IsDeleted = true;
                e.DateFlaggedForDeletionUtc = DateTime.UtcNow;
            });

            Update(deleteRange, SaveAfterExecute);
            return;
        }

        public TDataModel Get(params object[] primary_key)
        {
            return _context.Find<TDataModel>(primary_key);
        }

        public IEnumerable<TDataModel> GetAll()
        {
            return _context.Set<TDataModel>();
        }

        public IEnumerable<TDataModel> GetAll(Expression<Func<TDataModel, bool>> predicate)
        {
            var set = _context.Set<TDataModel>();
            return set.Where(predicate);
        }

        public IEnumerable<TDataModel> GetAllActive()
        {
            return _context.Set<TDataModel>().Where(e => e.IsActive);
        }
        /// <summary>
        /// Will Add / Update the entry and return a tracked entity
        /// </summary>
        /// <param name="save"></param>
        /// <param name="SaveAfterExecute"></param>
        /// <returns></returns>
        public TDataModel Save(TDataModel save, bool SaveAfterExecute = true)
        {
            TDataModel result;
            
            try { result = Update(save, SaveAfterExecute); }
            catch { result =  Add(save, SaveAfterExecute); }
            return result;
        }
        /// <summary>
        /// Will save each item induvidually and track each saved entity. Will return a list of all saved objects.
        /// DO NOT USE FOR LARGE QTY OF ENTRIES
        /// </summary>
        /// <param name="save_list"></param>
        /// <param name="SaveAfterExecute"></param>
        /// <returns></returns>
        public List<TDataModel> Save(List<TDataModel> save_list, bool SaveAfterExecute = true)
        {
            List<TDataModel> result = new List<TDataModel>();
            save_list.ForEach(e => result.Add(Save(e, SaveAfterExecute)));
            return result;
        }

        /// <summary>
        /// Will only save changes of the calling type
        /// </summary>
        /// <returns></returns>
        public int SaveChanges()
        {
            //Save All objects not of the type you want to save
            var originalDataset = _context.ChangeTracker.Entries()
                    .Where(e => !typeof(TDataModel).IsAssignableFrom(e.Entity.GetType()) && e.State != EntityState.Unchanged)
                    .GroupBy(e => e.State)
                    .ToList();
            //change all object that arent your type to unchanged
            foreach (var entry in _context.ChangeTracker.Entries().Where(f => !typeof(TDataModel).IsAssignableFrom(f.Entity.GetType())))
            {
                entry.State = EntityState.Unchanged;
            }
            //save all remaining (of your type)
            int result = _context.SaveChanges();
            //return all objects not your type to its original state
            foreach (var state in originalDataset)
            {
                foreach (var entry in state)
                {
                    entry.State = state.Key;
                }
            }

            return result;
        }
        /// <summary>
        /// Undo all chanegs made any any entity of this type in the current context
        /// </summary>
        public void UndoChangesDbContext()
        {
            foreach (EntityEntry entry in _context.ChangeTracker.Entries().Where(f => typeof(TDataModel).IsAssignableFrom(f.Entity.GetType())))
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    default: break;
                }
            }
        }

        /// <summary>
        /// Undo all changes made to the given entity
        /// </summary>
        /// <param name="entity"></param>
        public void UndoChangesEntity(TDataModel entity)
        {
            EntityEntry entry = _context.Entry(entity);
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.State = EntityState.Unchanged;
                    break;
                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    break;
                case EntityState.Deleted:
                    entry.Reload();
                    break;
                default: break;
            }
        }

        public TDataModel Update(TDataModel update, bool SaveAfterExecute = true)
        {
            update.DateLastModifiedUtc = DateTime.UtcNow;

            var Endn = _context.Update(update);
            if (SaveAfterExecute) SaveChanges();
            return Endn.Entity;
        }

        public void Update(List<TDataModel> updateRange, bool SaveAfterExecute = true)
        {
            DateTime UTC = DateTime.UtcNow;
            updateRange.ForEach(e => e.DateLastModifiedUtc = UTC);

            _context.UpdateRange(updateRange);
            if (SaveAfterExecute) SaveChanges();
            return;
        }
    }
    public class UserServiceImplementation<TDataModel, TDbContext, TUser, TUserKey> :
        BasicServiceImplementation<TDataModel, TDbContext>,
        IUserService<TDataModel, TUserKey>
        where TDbContext : DbContext
        where TDataModel : UserEditableDataRowProperties<TUser, TUserKey>
    {
        public UserServiceImplementation(TDbContext ctx) : base(ctx)
        {

        }

        public TDataModel Activate(TDataModel activate, TUserKey UserId, bool SaveAfterExecute = true)
        {
            activate.LastModifiedById = UserId;
            return Activate(activate, SaveAfterExecute);
        }

        public void Activate(List<TDataModel> activateRange, TUserKey UserId, bool SaveAfterExecute = true)
        {
            activateRange.ForEach(e => e.LastModifiedById = UserId);
            Activate(activateRange, SaveAfterExecute);
            return;
        }

        public TDataModel Add(TDataModel add, TUserKey UserId, bool SaveAfterExecute = true)
        {
            add.CreatedById = UserId;
            add.LastModifiedById = UserId;
            return Add(add, SaveAfterExecute);
        }

        public void Add(List<TDataModel> addRange, TUserKey UserId, bool SaveAfterExecute = true)
        {
            addRange.ForEach(e =>
            {
                e.LastModifiedById = UserId;
                e.CreatedById = UserId;
            });
            Add(addRange, SaveAfterExecute);
            return;
        }

        public TDataModel Deactivate(TDataModel deactivate, TUserKey UserId, bool SaveAfterExecute = true)
        {
            deactivate.LastModifiedById = UserId;
            return Deactivate(deactivate, SaveAfterExecute);
        }

        public void Deactivate(List<TDataModel> deactivateRange, TUserKey UserId, bool SaveAfterExecute = true)
        {
            deactivateRange.ForEach(e => e.LastModifiedById = UserId);
            Deactivate(deactivateRange, SaveAfterExecute);
            return;
        }

        public TDataModel Delete(TDataModel delete, TUserKey UserId, bool SaveAfterExecute = true)
        {
            delete.LastModifiedById = UserId;
            return Delete(delete, SaveAfterExecute);
        }

        public void Delete(List<TDataModel> deleteRange, TUserKey UserId, bool SaveAfterExecute = true)
        {
            deleteRange.ForEach(e => e.LastModifiedById = UserId);
            Delete(deleteRange, SaveAfterExecute);
            return;
        }

        public TDataModel Update(TDataModel update, TUserKey UserId, bool SaveAfterExecute = true)
        {
            update.LastModifiedById = UserId;
            return Update(update, SaveAfterExecute);
        }

        public void Update(List<TDataModel> updateRange, TUserKey UserId, bool SaveAfterExecute = true)
        {
            updateRange.ForEach(e => e.LastModifiedById = UserId);
            Update(updateRange, SaveAfterExecute);
        }
    }
}
