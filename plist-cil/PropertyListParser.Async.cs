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
using System.Text;
using System.Threading.Tasks;

namespace Claunia.PropertyList
{
    /// <summary>
    ///     This class provides methods to parse property lists. It can handle files, input streams and byte arrays. All
    ///     known property list formats are supported. This class also provides methods to save and convert property lists.
    /// </summary>
    /// @author Daniel Dreibrodt
    /// @author Natalia Portillo
    public static partial class PropertyListParser
    {
        /// <summary>Reads all bytes from an Stream and stores them in an array, up to a maximum count.</summary>
        /// <param name="fs">The Stream pointing to the data that should be stored in the array.</param>
        internal static async Task<byte[]> ReadAllAsync(Stream fs)
        {
            using var outputStream = new MemoryStream();

            await fs.CopyToAsync(outputStream);

            return outputStream.ToArray();
        }

        /// <summary>Parses a property list from a file.</summary>
        /// <param name="filePath">Path to the property list file.</param>
        /// <returns>The root object in the property list. This is usually a NSDictionary but can also be a NSArray.</returns>
        public static async Task<NSObject> ParseAsync(string filePath) => await ParseAsync(new FileInfo(filePath)).ConfigureAwait(false);

        /// <summary>Parses a property list from a file.</summary>
        /// <param name="f">The property list file.</param>
        /// <returns>The root object in the property list. This is usually a NSDictionary but can also be a NSArray.</returns>
        public static async Task<NSObject> ParseAsync(FileInfo f)
        {
            using FileStream fis = f.OpenRead();

            return await ParseAsync(fis).ConfigureAwait(false);
        }

        /// <summary>Parses a property list from an Stream.</summary>
        /// <param name="fs">The Stream delivering the property list data.</param>
        /// <returns>The root object of the property list. This is usually a NSDictionary but can also be a NSArray.</returns>
        public static async Task<NSObject> ParseAsync(Stream fs) => Parse(await ReadAllAsync(fs).ConfigureAwait(false));

        /// <summary>Saves a property list with the given object as root into a XML file.</summary>
        /// <param name="root">The root object.</param>
        /// <param name="outFile">The output file.</param>
        /// <exception cref="IOException">When an error occurs during the writing process.</exception>
        public static async Task SaveAsXmlAsync(NSObject root, FileInfo outFile)
        {
            string parent = outFile.DirectoryName;

            if(!Directory.Exists(parent))
                Directory.CreateDirectory(parent);

            // Use Create here -- to make sure that when the updated file is shorter than
            // the original file, no "obsolete" data is left at the end.
            using Stream fous = outFile.Open(FileMode.Create, FileAccess.ReadWrite);

            await SaveAsXmlAsync(root, fous).ConfigureAwait(false);
        }

        /// <summary>Saves a property list with the given object as root in XML format into an output stream.</summary>
        /// <param name="root">The root object.</param>
        /// <param name="outStream">The output stream.</param>
        /// <exception cref="IOException">When an error occurs during the writing process.</exception>
        public static async Task SaveAsXmlAsync(NSObject root, Stream outStream)
        {
            using var w = new StreamWriter(outStream, Encoding.UTF8, 1024, true);

            await w.WriteAsync(root.ToXmlPropertyList());
        }

        /// <summary>Converts a given property list file into the OS X and iOS XML format.</summary>
        /// <param name="inFile">The source file.</param>
        /// <param name="outFile">The target file.</param>
        public static async Task ConvertToXmlAsync(FileInfo inFile, FileInfo outFile)
        {
            NSObject root = await ParseAsync(inFile).ConfigureAwait(false);
            await SaveAsXmlAsync(root, outFile).ConfigureAwait(false);
        }

        /// <summary>Saves a property list with the given object as root into a binary file.</summary>
        /// <param name="root">The root object.</param>
        /// <param name="outFile">The output file.</param>
        /// <exception cref="IOException">When an error occurs during the writing process.</exception>
        public static async Task SaveAsBinaryAsync(NSObject root, FileInfo outFile)
        {
            string parent = outFile.DirectoryName;

            if(!Directory.Exists(parent))
                Directory.CreateDirectory(parent);

            await BinaryPropertyListWriter.WriteAsync(outFile, root).ConfigureAwait(false);
        }

        /// <summary>Saves a property list with the given object as root in binary format into an output stream.</summary>
        /// <param name="root">The root object.</param>
        /// <param name="outStream">The output stream.</param>
        /// <exception cref="IOException">When an error occurs during the writing process.</exception>
        public static async Task SaveAsBinaryAsync(NSObject root, Stream outStream) =>
            await BinaryPropertyListWriter.WriteAsync(outStream, root).ConfigureAwait(false);

        /// <summary>Converts a given property list file into the OS X and iOS binary format.</summary>
        /// <param name="inFile">The source file.</param>
        /// <param name="outFile">The target file.</param>
        public static async Task ConvertToBinaryAsync(FileInfo inFile, FileInfo outFile)
        {
            NSObject root = await ParseAsync(inFile).ConfigureAwait(false);
            await SaveAsBinaryAsync(root, outFile).ConfigureAwait(false);
        }

        /// <summary>Saves a property list with the given object as root into a ASCII file.</summary>
        /// <param name="root">The root object.</param>
        /// <param name="outFile">The output file.</param>
        /// <exception cref="IOException">When an error occurs during the writing process.</exception>
        public static async Task SaveAsASCIIAsync(NSDictionary root, FileInfo outFile)
        {
            string parent = outFile.DirectoryName;

            if(!Directory.Exists(parent))
                Directory.CreateDirectory(parent);

            using Stream fous = outFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);

            using var w = new StreamWriter(fous, Encoding.ASCII);

            await w.WriteAsync(root.ToASCIIPropertyList()).ConfigureAwait(false);
        }

        /// <summary>Saves a property list with the given object as root into a ASCII file.</summary>
        /// <param name="root">The root object.</param>
        /// <param name="outFile">The output file.</param>
        /// <exception cref="IOException">When an error occurs during the writing process.</exception>
        public static async Task SaveAsASCIIAsync(NSArray root, FileInfo outFile)
        {
            string parent = outFile.DirectoryName;

            if(!Directory.Exists(parent))
                Directory.CreateDirectory(parent);

            using Stream fous = outFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);

            using var w = new StreamWriter(fous, Encoding.ASCII);

            await w.WriteAsync(root.ToASCIIPropertyList()).ConfigureAwait(false);
        }

        /// <summary>Converts a given property list file into ASCII format.</summary>
        /// <param name="inFile">The source file.</param>
        /// <param name="outFile">The target file.</param>
        public static async Task ConvertToASCIIAsync(FileInfo inFile, FileInfo outFile)
        {
            NSObject root = await ParseAsync(inFile).ConfigureAwait(false);

            if(root is NSDictionary dictionary)
                await SaveAsASCIIAsync(dictionary, outFile).ConfigureAwait(false);
            else if(root is NSArray array)
                await SaveAsASCIIAsync(array, outFile).ConfigureAwait(false);
            else
                throw new PropertyListFormatException("The root of the given input property list " +
                                                      "is neither a Dictionary nor an Array!");
        }

        /// <summary>Saves a property list with the given object as root into a GnuStep ASCII file.</summary>
        /// <param name="root">The root object.</param>
        /// <param name="outFile">The output file.</param>
        /// <exception cref="IOException">When an error occurs during the writing process.</exception>
        public static async Task SaveAsGnuStepASCIIAsync(NSDictionary root, FileInfo outFile)
        {
            string parent = outFile.DirectoryName;

            if(!Directory.Exists(parent))
                Directory.CreateDirectory(parent);

            using Stream fous = outFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);

            using var w = new StreamWriter(fous, Encoding.ASCII);

            await w.WriteAsync(root.ToGnuStepASCIIPropertyList()).ConfigureAwait(false);
        }

        /// <summary>Saves a property list with the given object as root into a GnuStep ASCII file.</summary>
        /// <param name="root">The root object.</param>
        /// <param name="outFile">The output file.</param>
        /// <exception cref="IOException">When an error occurs during the writing process.</exception>
        public static async Task SaveAsGnuStepASCIIAsync(NSArray root, FileInfo outFile)
        {
            string parent = outFile.DirectoryName;

            if(!Directory.Exists(parent))
                Directory.CreateDirectory(parent);

            using Stream fous = outFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);

            using var w = new StreamWriter(fous, Encoding.ASCII);

            await w.WriteAsync(root.ToGnuStepASCIIPropertyList()).ConfigureAwait(false);
        }

        /// <summary>Converts a given property list file into GnuStep ASCII format.</summary>
        /// <param name="inFile">The source file.</param>
        /// <param name="outFile">The target file.</param>
        public static async Task ConvertToGnuStepASCIIAsync(FileInfo inFile, FileInfo outFile)
        {
            NSObject root = await ParseAsync(inFile).ConfigureAwait(false);

            switch(root)
            {
                case NSDictionary dictionary:
                    await SaveAsGnuStepASCIIAsync(dictionary, outFile).ConfigureAwait(false);
                    break;
                case NSArray array:
                    await SaveAsGnuStepASCIIAsync(array, outFile).ConfigureAwait(false);
                    break;
                default:
                    throw new PropertyListFormatException("The root of the given input property list " +
                                                          "is neither a Dictionary nor an Array!");
            }
        }
    }
}