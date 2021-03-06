/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2019 iText Group NV
    Authors: iText Software.

This program is free software; you can redistribute it and/or modify it under the terms of the GNU Affero General Public License version 3 as published by the Free Software Foundation with the addition of the following permission added to Section 15 as permitted in Section 7(a): FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY iText Group NV, iText Group NV DISCLAIMS THE WARRANTY OF NON INFRINGEMENT OF THIRD PARTY RIGHTS.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License along with this program; if not, see http://www.gnu.org/licenses or write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA, 02110-1301 USA, or download the license from the following URL:

http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions of this program must display Appropriate Legal Notices, as required under Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License, a covered work must retain the producer line in every PDF that is created or manipulated using iText.

You can be released from the requirements of the license by purchasing a commercial license. Buying such a license is mandatory as soon as you develop commercial activities involving the iText software without disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP, serving PDFs on the fly in a web application, shipping iText with a closed source product.

For more information, please contact iText Software Corp. at this address: sales@itextpdf.com */
using System;

namespace Org.BouncyCastle.Crypto.Prng
{
	/// <remarks>
	/// Takes bytes generated by an underling RandomGenerator and reverses the order in
	/// each small window (of configurable size).
	/// <p>
	/// Access to internals is synchronized so a single one of these can be shared.
	/// </p>
	/// </remarks>
	public class ReversedWindowGenerator
		: IRandomGenerator
	{
		private readonly IRandomGenerator generator;

		private byte[] window;
		private int windowCount;

		public ReversedWindowGenerator(
			IRandomGenerator	generator,
			int					windowSize)
		{
			if (generator == null)
				throw new ArgumentNullException("generator");
			if (windowSize < 2)
				throw new ArgumentException("Window size must be at least 2", "windowSize");

			this.generator = generator;
			this.window = new byte[windowSize];
		}

		/// <summary>Add more seed material to the generator.</summary>
		/// <param name="seed">A byte array to be mixed into the generator's state.</param>
		public virtual void AddSeedMaterial(
			byte[] seed)
		{
			lock (this)
			{
				windowCount = 0;
				generator.AddSeedMaterial(seed);
			}
		}

		/// <summary>Add more seed material to the generator.</summary>
		/// <param name="seed">A long value to be mixed into the generator's state.</param>
		public virtual void AddSeedMaterial(
			long seed)
		{
			lock (this)
			{
				windowCount = 0;
				generator.AddSeedMaterial(seed);
			}
		}

		/// <summary>Fill byte array with random values.</summary>
		/// <param name="bytes">Array to be filled.</param>
		public virtual void NextBytes(
			byte[] bytes)
		{
			doNextBytes(bytes, 0, bytes.Length);
		}

		/// <summary>Fill byte array with random values.</summary>
		/// <param name="bytes">Array to receive bytes.</param>
		/// <param name="start">Index to start filling at.</param>
		/// <param name="len">Length of segment to fill.</param>
		public virtual void NextBytes(
			byte[]	bytes,
			int		start,
			int		len)
		{
			doNextBytes(bytes, start, len);
		}

		private void doNextBytes(
			byte[]	bytes,
			int		start,
			int		len)
		{
			lock (this)
			{
				int done = 0;
				while (done < len)
				{
					if (windowCount < 1)
					{
						generator.NextBytes(window, 0, window.Length);
						windowCount = window.Length;
					}

					bytes[start + done++] = window[--windowCount];
				}
			}
		}
	}
}
