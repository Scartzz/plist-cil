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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Claunia.PropertyList
{
    /// <summary>Parses XML property lists.</summary>
    /// @author Daniel Dreibrodt
    /// @author Natalia Portillo
    public static partial class XmlPropertyListParser
    {
        /// <summary>Parses a XML property list file.</summary>
        /// <param name="f">The XML property list file.</param>
        /// <returns>The root object of the property list. This is usually a NSDictionary but can also be a NSArray.</returns>
        public static async Task<NSObject> ParseAsync(FileInfo f)
        {
            var doc = new XmlDocument();

            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore
            };

            using(Stream stream = f.OpenRead())
                using(var tempStream = await LoadStreamAsync(stream).ConfigureAwait(false))
                    using(var reader = XmlReader.Create(tempStream, settings))
                        doc.Load(reader);

            return ParseDocument(doc);
        }

        /// <summary>Parses a XML property list from an input stream.</summary>
        /// <param name="str">The input stream pointing to the property list's data.</param>
        /// <returns>The root object of the property list. This is usually a NSDictionary but can also be a NSArray.</returns>
        public static async Task<NSObject> ParseAsync(Stream str)
        {
            var doc = new XmlDocument();

            var settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;

            using(var tempStream = await LoadStreamAsync(str).ConfigureAwait(false))
                using(var reader = XmlReader.Create(tempStream, settings))
                    doc.Load(reader);

            return ParseDocument(doc);
        }

        static async Task<MemoryStream> LoadStreamAsync(Stream s)
        {
            MemoryStream ms = new MemoryStream();
            await s.CopyToAsync(ms).ConfigureAwait(false);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}