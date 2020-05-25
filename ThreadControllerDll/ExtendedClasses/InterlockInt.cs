using System.Threading;

namespace ThreadControllerDll.OtherComponents.ExtendedClasses
{
    /// <summary>
    /// Adds Interlock class to int
    /// </summary>
    public sealed class InterlockInt
    {
        private int count;

        /// <summary>
        /// Automatically sets value to 0
        /// </summary>
        public InterlockInt()
        {
            count = 0;
        }

        public InterlockInt(int initialValue)
        {
            count = initialValue;
        }

        /// <summary>
        /// Uses interlocked to increment value
        /// </summary>
        /// <returns>Incremented Value</returns>
        public int Increment()
        {
            return Interlocked.Increment(ref count);
        }

        /// <summary>
        /// Uses interlocked to Decrement value
        /// </summary>
        /// <returns>Decrement Value</returns>
        public int Decrement()
        {
            return Interlocked.Decrement(ref count);
        }

        public int Get()
        {
            //The Read method is unnecessary on 64-bit systems, because 64-bit read operations are already atomic. On 32-bit systems, 64-bit read operations are not atomic unless performed using Read.
            return count;
        }

        public int Set(int value)
        {
            Interlocked.Exchange(ref count, value);
            return count;
        }
    }
}