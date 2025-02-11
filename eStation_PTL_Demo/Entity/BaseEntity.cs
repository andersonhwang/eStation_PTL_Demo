﻿namespace eStation_PTL_Demo.Entity
{
    /// <summary>
    /// Entity base
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// Code
        /// </summary>
        public int Code { get; set; } = 0;
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; } = string.Empty;
    }

    /// <summary>
    /// Sequence entity base
    /// </summary>
    public class SequenceEntity : BaseEntity
    {
        /// <summary>
        /// Sequence
        /// </summary>
        public int Sequence { get; set; } = 0;
    }

    /// <summary>
    /// Interface of PTL entity
    /// </summary>
    public interface ITag
    {
        /// <summary>
        /// Get length
        /// </summary>
        /// <returns>Length</returns>
        public byte Length();
    }

    /// <summary>
    /// Interface of PTLItem entity
    /// </summary>
    public interface ITagItem
    {
        /// <summary>
        /// Get bytes
        /// </summary>
        /// <returns></returns>
        public byte[] Bytes();
    }
}
