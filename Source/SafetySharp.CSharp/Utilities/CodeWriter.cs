﻿// The MIT License (MIT)
// 
// Copyright (c) 2014, Institute for Software & Systems Engineering
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
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace SafetySharp.Utilities
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;

	/// <summary>
	///     Generates code in an in-memory buffer.
	/// </summary>
	internal class CodeWriter
	{
		/// <summary>
		///     The buffer that contains the written code.
		/// </summary>
		private readonly StringBuilder _buffer = new StringBuilder(4096);

		/// <summary>
		///     Indicates whether the writer is currently at the beginning of a new line.
		/// </summary>
		private bool _atBeginningOfLine = true;

		/// <summary>
		///     The number of tabs that are placed at the beginning of the next line.
		/// </summary>
		private int _indent;

		/// <summary>
		///     Initializes a new instance of the <see cref="CodeWriter" /> type.
		/// </summary>
		public CodeWriter()
		{
			WriterHeader();
		}

        /// <summary>
		///     Initializes a new instance of the <see cref="CodeWriter" /> type and use a custom writeHeaderAction.
		/// </summary>
		public CodeWriter(Action writeHeaderAction)
        {
            if (writeHeaderAction!=null)
                writeHeaderAction();
        }

        /// <summary>
        ///     Appends the given format string to the current line.
        /// </summary>
        /// <param name="format">The format string that should be appended.</param>
        /// <param name="arguments">The arguments that should be copied into the format string.</param>
        [StringFormatMethod("format")]
		public void Append(string format, params object[] arguments)
		{
			Requires.NotNull(format, () => format);

			AddIndentation();
			_buffer.AppendFormat(format, arguments);
		}

		/// <summary>
		///     Appends the given format string to the current line and starts a new line.
		/// </summary>
		/// <param name="format">The format string that should be appended.</param>
		/// <param name="arguments">The arguments that should be copied into the format string.</param>
		[StringFormatMethod("format")]
		public void AppendLine(string format, params object[] arguments)
		{
			Requires.NotNull(format, () => format);

			AddIndentation();
			_buffer.AppendFormat(format, arguments);
			NewLine();
		}

		/// <summary>
		///     Appends a new line to the buffer.
		/// </summary>
		public void NewLine()
		{
			_buffer.AppendLine();
			_atBeginningOfLine = true;
		}

		/// <summary>
		///     Ensures that the subsequent write operation is performed on a new line.
		/// </summary>
		public void EnsureNewLine()
		{
			if (!_atBeginningOfLine)
				NewLine();
		}

		/// <summary>
		///     If the writer is currently at the beginning of a new line, adds the necessary number of tabs to the current line in
		///     order to get the desired indentation level.
		/// </summary>
		private void AddIndentation()
		{
			if (!_atBeginningOfLine)
				return;

			_atBeginningOfLine = false;
			for (var i = 0; i < _indent; ++i)
				_buffer.Append("\t");
		}

		/// <summary>
		///     Increases the indent of the next line that is generated.
		/// </summary>
		public void IncreaseIndent()
		{
			++_indent;
		}

		/// <summary>
		///     Decreases the indent of the next line that is generated.
		/// </summary>
		public void DecreaseIndent()
		{
			--_indent;
		}

		/// <summary>
		///     Appends a block statement to the buffer. The given content is generated within a pair of curly braces.
		/// </summary>
		/// <param name="content">
		///     Generates the content that should be placed within the block statement by calling Append methods
		///     of this code writer instance.
		/// </param>
		public void AppendBlockStatement(Action content)
		{
			Requires.NotNull(content, () => content);

			EnsureNewLine();
			AppendLine("{{");
			IncreaseIndent();

			content();

			EnsureNewLine();
			DecreaseIndent();
			Append("}}");

			NewLine();
		}

		/// <summary>
		///     Appends a list of values to the current line, with each value being separated by the given separator.
		/// </summary>
		/// <param name="values">The source values for each of which the content is generated.</param>
		/// <param name="separator">The separator that separates two successive values.</param>
		/// <param name="content">
		///     Generates the content that should be appended for each value in source by calling Append methods
		///     of this code writer instance.
		/// </param>
		public void AppendSeparated<T>(IEnumerable<T> values, string separator, Action<T> content)
		{
			Requires.NotNull(values, () => values);
			Requires.NotNull(separator, () => separator);
			Requires.NotNull(content, () => content);

			AppendSeparated(values, () => Append(separator), content);
		}

		/// <summary>
		///     Appends a list of values to the current line, with each value being separated by the given separator.
		/// </summary>
		/// <param name="source">The source values, for each of which the content is generated.</param>
		/// <param name="separator">
		///     Generates the separator that should be appended between two consecutive values by calling Append methods
		///     of this code writer instance.
		/// </param>
		/// <param name="content">
		///     Generates the content that should be appended for each value in source by calling Append methods
		///     of this code writer instance.
		/// </param>
		public void AppendSeparated<T>(IEnumerable<T> source, Action separator, Action<T> content)
		{
			Requires.NotNull(source, () => source);
			Requires.NotNull(separator, () => separator);
			Requires.NotNull(content, () => content);

			var count = source.Count();
			var i = 0;
			foreach (var value in source)
			{
				content(value);
				if (i < count - 1)
					separator();

				++i;
			}
		}

		/// <summary>
		///     Writes a header that indicates that the file has been generated by a tool.
		/// </summary>
		private void WriterHeader()
		{
			AppendLine("// ------------------------------------------------------------------------------");
			AppendLine("//  <auto-generated>");
			AppendLine("//      Generated by the Safety Sharp Compiler.");
			AppendLine("//      {0}, {1}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
			AppendLine("// ");
			AppendLine("//      Changes to this file may cause incorrect behavior and will be lost if");
			AppendLine("//      the code is regenerated.");
			AppendLine("//  </auto-generated>");
			AppendLine("// ------------------------------------------------------------------------------");
			NewLine();
		}

		/// <summary>
		///     Writes the generated code to the file at the given path.
		/// </summary>
		/// <param name="path">The path of the file that should be generated.</param>
		public void WriteToFile(string path)
		{
			Requires.NotNullOrWhitespace(path, () => path);
			File.WriteAllText(path, _buffer.ToString());
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return _buffer.ToString();
		}
	}
}