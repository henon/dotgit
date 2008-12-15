﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotGit.Exceptions
{
	public class PackFileException : Exception
	{
		public PackFileException(string message, string packFile)
			:base(message)
		{
			PackFile = packFile;
		}

    public PackFileException(string message, string packFile, Exception innerException)
      :base(message, innerException)
    {
      PackFile = packFile;
    }

		public string PackFile
		{
			get;
			private set;
		}
	}
}
