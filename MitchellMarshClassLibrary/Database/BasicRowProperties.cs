using System;

namespace MitchellMarshClassLibrary.Database
{
    /// <summary>
    /// Basic Data Row Properties that each database record should have. Use this for all non-user defined database models
    /// </summary>
    public class DataRowProperties
    {
        public DateTime DateLastModifiedUtc { get; set; }
        public DateTime DateCreatedUtc { get; set; }
        public DateTime? DateInactivatedUtc { get; set; }
        public DateTime? DateFlaggedForDeletionUtc { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }


    }

    /// <summary>
    /// Basic Data Row Properties that each database record should have. Use this for tracking user touches on records
    /// </summary>
    /// <typeparam name="TUser">Datatype of applicaion user to link</typeparam>
    /// <typeparam name="TKey">Datatype of the user id (key)</typeparam>
    public class UserEditableDataRowProperties<TUser, TKey> : DataRowProperties
    {
        public TUser CreatedBy { get; set; }
        public TKey CreatedById { get; set; }
        public TUser LastModifiedBy { get; set; }
        public TKey LastModifiedById { get; set; }
    }
}
