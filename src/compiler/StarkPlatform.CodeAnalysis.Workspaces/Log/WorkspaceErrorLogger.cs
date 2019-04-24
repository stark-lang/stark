// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Composition;
using StarkPlatform.CodeAnalysis.Host.Mef;
using StarkPlatform.CodeAnalysis.Internal.Log;

namespace StarkPlatform.CodeAnalysis.ErrorLogger
{
    [ExportWorkspaceService(typeof(IErrorLoggerService)), Export(typeof(IErrorLoggerService)), Shared]
    internal class WorkspaceErrorLogger : IErrorLoggerService
    {
        public void LogException(object source, Exception exception)
        {
            Logger.GetLogger()?.Log(FunctionId.Extension_Exception, LogMessage.Create(source.GetType().Name + " : " + ToLogFormat(exception)));
        }

        private static string ToLogFormat(Exception exception)
        {
            return exception.Message + Environment.NewLine + exception.StackTrace;
        }
    }
}

