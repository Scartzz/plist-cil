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
using System.Threading;
using System.Threading.Tasks;

namespace Claunia.PropertyList
{
    /// <summary>
    ///     <para>
    ///         Parses property lists that are in Apple's binary format. Use this class when you are sure about the format of
    ///         the property list. Otherwise use the PropertyListParser class.
    ///     </para>
    ///     <para>
    ///         Parsing is done by calling the static <see cref="Parse(byte[])" />, <see cref="Parse(FileInfo)" /> and
    ///         <see cref="Parse(Stream)" /> methods.
    ///     </para>
    /// </summary>
    /// @author Daniel Dreibrodt
    /// @author Natalia Portillo
    public sealed partial class BinaryPropertyListParser
    {
        /// <summary>Parses a binary property list from an input stream.</summary>
        /// <param name="fs">The input stream that points to the property list's data.</param>
        /// <returns>The root object of the property list. This is usually a NSDictionary but can also be a NSArray.</returns>
        /// <exception cref="PropertyListFormatException">When the property list's format could not be parsed.</exception>
        public static async Task<NSObject> ParseAsync(Stream fs)
        {
            //Read all bytes into a list
            byte[] buf = await PropertyListParser.ReadAllAsync(fs).ConfigureAwait(false);

            // Don't close the stream - that would be the responisibility of code that class
            // Parse
            return Parse(buf);
        }

        /// <summary>Parses a binary property list file.</summary>
        /// <param name="f">The binary property list file</param>
        /// <returns>The root object of the property list. This is usually a NSDictionary but can also be a NSArray.</returns>
        /// <exception cref="PropertyListFormatException">When the property list's format could not be parsed.</exception>
        public static async Task<NSObject> ParseAsync(FileInfo f) => await ParseAsync(f.OpenRead()).ConfigureAwait(false);
    }
}