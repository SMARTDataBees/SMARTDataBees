// #StartHeader# ==============================================================
//
// This file is a part of the SMARTDataBees open source project.
// 
// Copyright (C) 2007 by
//        G.E.M. Team Solutions GbR
//        CAD-Development
//
// SMARTDataBees is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SMARTDataBees is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with SMARTDataBees.  If not, see <http://www.gnu.org/licenses/>.
//
// #EndHeader# ================================================================

using System;
using System.Security.Cryptography;
using System.Text;

namespace SDBees.Security
{
	public class SecurityManager
	{
	    /// <summary>
		/// Returns the Hash for the given String
		/// </summary>
		/// <param name="sInput"></param>
		/// <returns></returns>
		public string GetMD5Hash(string sInput)
		{
			byte[] hashedBytes = null;
			string sReturn = null;
			try
			{
				//Encrypt the string
				var md5Hasher = new MD5CryptoServiceProvider();
				var encoder = new UTF8Encoding();
				hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(sInput));

#if true
                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                var sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (var i = 0; i < hashedBytes.Length; i++)
                {
                    sBuilder.Append(hashedBytes[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                sReturn = sBuilder.ToString();
#else
			    char[] hashedChars = null;
				hashedChars = encoder.GetChars(hashedBytes);
				foreach (char c in hashedChars)
				{
					sReturn += c;
				}
#endif
            }
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			return sReturn;
		}
	}
}
