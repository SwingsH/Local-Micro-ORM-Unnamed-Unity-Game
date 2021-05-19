using System;
using FlatBuffers;

namespace Tizsoft.Utils
{
	/// <summary>
    /// 提供 FlatBuffers 擴充方法。
    /// </summary>
	public static class FlatBuffersExtensions
	{
        #region FlatBufferBuilder

		/// <summary>
        /// 直接取出 FlatBufferBuilder 的 ArraySegment，此方法不會額外消耗空間。
        /// </summary>
        /// <param name="fbb"></param>
        /// <returns></returns>
		public static ArraySegment<byte> ToArraySegment(this FlatBufferBuilder fbb)
		{
            ExceptionUtils.VerifyArgumentNull(fbb, "fbb", "Argument is null.");
            return fbb.DataBuffer.ToArraySegment();
		}

        #endregion FlatBufferBuilder

        #region ByteBuffer

        /// <summary>
        /// 直接取出 ByteBuffer 的 ArraySegment，此方法不會額外消耗空間。
        /// </summary>
        /// <param name="bb"></param>
        /// <returns></returns>
        public static ArraySegment<byte> ToArraySegment(this ByteBuffer bb)
		{
            ExceptionUtils.VerifyArgumentNull(bb, "bb", "Argument is null.");
			ExceptionUtils.VerifyArgumentNull(bb.Data, "bb.Data", "Invalid argument. bb.Data is null.");
            return new ArraySegment<byte>(bb.Data, bb.Position, bb.Data.Length - bb.Position);
        }

        #endregion ByteBuffer
    }
}