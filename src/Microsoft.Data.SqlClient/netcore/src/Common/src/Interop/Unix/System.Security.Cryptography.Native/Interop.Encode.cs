// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

internal static partial class Interop
{
    internal static partial class Crypto
    {
        internal delegate int GetEncodedSizeFunc<in THandle>(THandle handle);

        internal delegate int EncodeFunc<in THandle>(THandle handle, byte[] buf);

        internal static byte[] OpenSslEncode<THandle>(GetEncodedSizeFunc<THandle> getSize, EncodeFunc<THandle> encode, THandle handle)
            where THandle : SafeHandle
        {
            int size = getSize(handle);

            if (size < 1)
            {
                throw Crypto.CreateOpenSslCryptographicException();
            }

            byte[] data = new byte[size];

            int size2 = encode(handle, data);
            if (size2 < 1)
            {
                Debug.Fail(
                    $"{nameof(OpenSslEncode)}: {nameof(getSize)} succeeded ({size}) and {nameof(encode)} failed ({size2})");

                // If it ever happens, ensure the error queue gets cleared.
                // And since it didn't write the data, reporting an exception is good too.
                throw CreateOpenSslCryptographicException();
            }

            Debug.Assert(size == size2);
            
            return data;
        }
    }
}
