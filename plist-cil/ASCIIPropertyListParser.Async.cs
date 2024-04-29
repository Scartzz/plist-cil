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

using System;
using System.IO;
using System.Threading.Tasks;

namespace Claunia.PropertyList
{
    /// <summary>
    ///     <para>
    ///         Parser for ASCII property lists. Supports Apple OS X/iOS and GnuStep/NeXTSTEP format. This parser is based on
    ///         the recursive descent paradigm, but the underlying grammar is not explicitly defined.
    ///     </para>
    ///     <para>Resources on ASCII property list format:</para>
    ///     <para>https://developer.apple.com/library/mac/#documentation/Cocoa/Conceptual/PropertyLists/OldStylePlists/OldStylePLists.html</para>
    ///     <para>Property List Programming Guide - Old-Style ASCII Property Lists</para>
    ///     <para>http://www.gnustep.org/resources/documentation/Developer/Base/Reference/NSPropertyList.html</para>
    ///     <para>GnuStep - NSPropertyListSerialization class documentation</para>
    /// </summary>
    /// @author Daniel Dreibrodt
    /// @author Natalia Portillo
    public sealed partial class ASCIIPropertyListParser
    {
        /// <summary>Parses an ASCII property list file.</summary>
        /// <param name="f">The ASCII property list file..</param>
        /// <returns>The root object of the property list. This is usually a NSDictionary but can also be a NSArray.</returns>
        /// <exception cref="FormatException">When an error occurs during parsing.</exception>
        /// <exception cref="IOException">When an error occured while reading from the input stream.</exception>
        public static async Task<NSObject> ParseAsync(FileInfo f) => await ParseAsync(f.OpenRead()).ConfigureAwait(false);

        /// <summary>Parses an ASCII property list from an input stream.</summary>
        /// <param name="fs">The input stream that points to the property list's data.</param>
        /// <returns>The root object of the property list. This is usually a NSDictionary but can also be a NSArray.</returns>
        /// <exception cref="FormatException">When an error occurs during parsing.</exception>
        /// <exception cref="IOException"></exception>
        public static async Task<NSObject> ParseAsync(Stream fs)
        {
            byte[] buf = await PropertyListParser.ReadAllAsync(fs).ConfigureAwait(false);

            // Don't close the stream - that would be the responsibility of code that class
            // Parse
            return Parse(buf);
        }
    }
}