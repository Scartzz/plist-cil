// plist-cil - An open source library to parse and generate property lists for .NET
// Copyright (C) 2015 Natalia Portillo
//
// This code is based on:
// plist - An open source library to parse and generate property lists
// Copyright (C) 2014 Daniel Dreibrodt
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.IO;
using System.Threading.Tasks;

namespace Claunia.PropertyList
{
    /// <summary>
    ///     <para>A BinaryPropertyListWriter is a helper class for writing out binary property list files.</para>
    ///     <para>
    ///         It contains an output stream and various structures for keeping track of which NSObjects have already been
    ///         serialized, and where they were put in the file.
    ///     </para>
    /// </summary>
    /// @author Keith Randall
    /// @author Natalia Portillo
    public sealed partial class BinaryPropertyListWriter
    {
        /// <summary>Writes a binary plist file with the given object as the root.</summary>
        /// <param name="file">the file to write to</param>
        /// <param name="root">the source of the data to write to the file</param>
        /// <exception cref="IOException"></exception>
        public static async Task WriteAsync(FileInfo file, NSObject root)
        {
            using FileStream fous = file.OpenWrite();
            await WriteAsync(fous, root).ConfigureAwait(false);
        }

        /// <summary>Writes a binary plist serialization of the given object as the root.</summary>
        /// <param name="outStream">the stream to write to</param>
        /// <param name="root">the source of the data to write to the stream</param>
        /// <exception cref="IOException"></exception>
        public static async Task WriteAsync(Stream outStream, NSObject root)
        {
            int minVersion = GetMinimumRequiredVersion(root);

            if(minVersion > VERSION_00)
            {
                string versionString = minVersion == VERSION_10
                                           ? "v1.0"
                                           : minVersion == VERSION_15
                                               ? "v1.5"
                                               : minVersion == VERSION_20
                                                   ? "v2.0"
                                                   : "v0.0";

                throw new IOException("The given property list structure cannot be saved. " +
                                      "The required version of the binary format ("         + versionString +
                                      ") is not yet supported.");
            }

            using var tempStream = new MemoryStream();
            var w = new BinaryPropertyListWriter(tempStream, minVersion);
            w.Write(root);
            tempStream.Seek(0, SeekOrigin.Begin);
            await tempStream.CopyToAsync(outStream).ConfigureAwait(false);
        }
    }
}