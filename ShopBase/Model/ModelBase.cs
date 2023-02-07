using System;
using ShopBase.Persistence;

namespace ShopBase.Model
{
    public class ModelBase<T> : IEquatable<T?>
        where T : ModelBase<T>, new()
    {
        public int Id { get; set; }

        public bool IsInDb
        {
            get { return this.Id > 0; }
        }

        public static T? TryGetById(int id)
        {
            return DBAccess<T>.Instance.TryGetById(id);
        }

        public static T GetById(int id)
        {
            return DBAccess<T>.Instance.GetById(id);
        }

        public static bool operator !=(ModelBase<T>? left, ModelBase<T>? right)
        {
            return !(left == right);
        }

        public static bool operator ==(ModelBase<T>? left, ModelBase<T>? right)
        {
            return EqualityComparer<ModelBase<T>>.Default.Equals(left, right);
        }

        public static IQueryable<T> ReadAll()
        {
            return DBAccess<T>.Instance.ReadAll();
        }

        public void Create()
        {
            DBAccess<T>.Instance.Create((T)this);
        }

        public void Delete()
        {
            DBAccess<T>.Instance.Delete((T)this);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as T);
        }

        public bool Equals(T? other)
        {
            return other is not null &&
                   this.Id > 0 &&
                   other.Id > 0 &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public void Update()
        {
            if (this.IsInDb)
            {
                DBAccess<T>.Instance.Update((T)this);
            }
            else
            {
                DBAccess<T>.Instance.Create((T)this);
            }
        }
    }
}