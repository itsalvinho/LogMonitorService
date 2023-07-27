using System.Text;

namespace LogMonitorService.Utilities
{
    /// <summary>
    /// Class <c>ReverseStreamReader</c> for reverse reading a stream line by line.
    /// </summary>
    public class ReverseStreamReader : IDisposable
    {
        private const byte LF_BYTE = 10;
        private const byte CR_BYTE = 13;

        private readonly Stream _stream;
        private readonly Encoding _encoding;
        private readonly int _bufferSize = 8192;

        private Queue<string> _parsedLines;     // Holds a queue of already parsed lines in memory for faster access
        private byte[] _prevBuffer;             // Holds un-processed bytes that were remaining from a buffer due to these bytes being piece of the next chunk-to-be-read's line

        public ReverseStreamReader(Stream stream, Encoding encoding, int bufferSize = 8192)
        {
            ValidateStream(stream);

            _stream = stream;
            _encoding = encoding;
            _parsedLines = new Queue<string>(_bufferSize/2); // Worst case scenario, every line in the buffer to read from contains 2 bytes: a character and Line Feed. _bufferSize/2 should be enough.
            _prevBuffer = new byte[0];
            _stream.Seek(0, SeekOrigin.End);
        }

        private void ValidateStream(Stream stream)
        {
            if (stream == null)
                throw new Exception("Stream cannot be null");
            if (!stream.CanRead) 
                throw new Exception("Cannot read stream");  // Possibly read permission issues or locked file?
            if (!stream.CanSeek) 
                throw new Exception("Cannot seek stream");  // This can happen if stream originates from the cloud (eg. Azure Blob Storage, S3 bucket)
        }

        /// <summary>
        /// Asynchronously reads the stream data backwards and returns the last line read as a string.
        /// 
        /// The process is as follows:
        ///     1. Check if there's a valid parsed line in the queue, return if exists, otherwise go to step 2.
        ///     2. Reading a chunk of data into a buffer (byte array with size defined based on the bufferSize provided in the constructor).
        ///     3. Iterate the buffer until it reaches a line feed (LF) aka \n.
        ///         a. Keep track of offsets in buffer to queue a line into memory
        ///         b. Keep track of whether the line contains the search pattern provided (skip if not provided)
        ///     4. Queue found lines into memory
        ///     5. Keep track of unread bytes that belongs to the next chunk-to-read's line in the _prevBuffer object so that we won't have to read it again in the next iteration.
        ///     5. If nothing in queue, repeat from step 1.
        /// 
        /// </summary>
        /// <param name="stream">The stream to write into.</param>
        /// <param name="logPath">The full path to the log file to read from.</param>
        /// <param name="encoding">The encoding of the log file.</param>
        /// <param name="searchText">The text to search in the logs for, it will only write the lines containing the substring.</param>
        /// <param name="maxLinesToReturn">The maximum number of lines to return in the stream.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation early.</param>
        /// <returns>
        /// The next line of text in the stream (that matches the search pattern if it was provided).
        /// Returns null if there are no longer lines to return.
        /// </returns>
        public async Task<string> ReadLineAsync(byte[]? searchPattern = null)
        {
            while (_stream.Position > 0 && _parsedLines.Count == 0)
            {
                long offsetToSeek = _stream.Position > _bufferSize ?
                    _bufferSize :
                    _stream.Position;

                // Seek backwards by the offset (calculated by buffersize)
                _stream.Seek(-offsetToSeek, SeekOrigin.Current);

                // Read into buffer
                byte[] currBuffer = new byte[offsetToSeek + _prevBuffer.Length];
                await _stream.ReadAsync(currBuffer, 0, (int)offsetToSeek); // Note that this is casted to INT but it would never actually be more because _bufferSize is int

                // Seek it back again
                _stream.Seek(-offsetToSeek, SeekOrigin.Current);

                // Put the old buffer back
                Buffer.BlockCopy(_prevBuffer, 0, currBuffer, (int)offsetToSeek, _prevBuffer.Length);

                int endOfLineIdx = currBuffer.Length - 1;
                bool lineHasCarriageReturn = currBuffer[endOfLineIdx - 1] == CR_BYTE;

                // Vars for pattern matching
                bool checkForPattern = searchPattern != null && searchPattern.Length > 0;
                bool lineHasPatternMatch = false;
                int searchPatternLastIdx = searchPattern?.Length - 1 ?? 0;
                int searchPatternCurrIdx = searchPatternLastIdx;
                for (int i = currBuffer.Length - 2; i >= 0; i--)
                {
                    byte currByte = currBuffer[i];
                    bool isFinalLineAndByte = i == 0 && _stream.Position == 0;

                    // Checks each byte on wether the current line being read contains the search pattern
                    // Pattern search could technically be done outside of this function but for performance reasons,
                    // if we're going to iterate through each byte anyway, we can check for matches here.
                    if (checkForPattern && !lineHasPatternMatch)
                    {
                        // If the current byte matches, iterate searchPatternIdx until it's 0 meaning the pattern is a full match
                        if (currByte == searchPattern[searchPatternCurrIdx])
                        {
                            if (searchPatternCurrIdx == 0)
                                lineHasPatternMatch = true;
                            else
                                searchPatternCurrIdx--;
                        }
                        // If it doesn't match and we were midway through a match, reset the currLineHasSearchPattern flag and index
                        else if (searchPatternCurrIdx < searchPatternLastIdx)
                        {
                            lineHasPatternMatch = false;
                            searchPatternCurrIdx = searchPatternLastIdx;
                        }
                    }

                    // Check for line feed or if it's the end of the stream
                    if (currByte == LF_BYTE || isFinalLineAndByte)
                    {
                        // Queue the string with either CR and LF removed (if CR exists) or just LF 
                        int lineLength = lineHasCarriageReturn ?
                            endOfLineIdx - i - 2 :
                            endOfLineIdx - i - 1;

                        // Only queue if we searched for a pattern and if the pattern actually matched
                        if (!checkForPattern || lineHasPatternMatch)
                            _parsedLines.Enqueue(_encoding.GetString(currBuffer, i + 1, lineLength));

                        endOfLineIdx = i;
                        lineHasCarriageReturn = endOfLineIdx > 0 && currBuffer[endOfLineIdx - 1] == CR_BYTE;

                        // Reset pattern matching flags and indices
                        if (checkForPattern)
                        {
                            lineHasPatternMatch = false;
                            searchPatternCurrIdx = searchPatternLastIdx;
                        }
                    }
                }

                // Hold any remaining bytes in the _prevBuffer so we have it ready for the next chunk of bytes to read
                _prevBuffer = new byte[endOfLineIdx + 1];
                Buffer.BlockCopy(currBuffer, 0, _prevBuffer, 0, endOfLineIdx + 1);
            }

            return _parsedLines.Count > 0 ?
                    _parsedLines.Dequeue() :
                    null;
        }

        /// <summary>
        /// Checks if at end of file (technically beginning since this class reads in reverse).
        /// </summary>
        public bool IsAtEnd()
        {
            return _stream.Position == 0;
        }

        public void Dispose()
        {
            _prevBuffer = null;
            _parsedLines = null;
        }
    }
}
