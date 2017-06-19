﻿using EthereumSamurai.Core.Models;
using EthereumSamurai.Core.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumSamurai.Indexer.Jobs
{
    public interface IBlockIndexingJobFactory
    {
        IJob GetJob(IIndexingSettings settings);
    }

    public class BlockIndexingJobFactory : IBlockIndexingJobFactory
    {
        private readonly IRpcBlockReader _rpcBlockReader;
        private readonly IIndexingService _indexingService;
        private readonly ILogger _logger;

        public BlockIndexingJobFactory(IRpcBlockReader rpcBlockReader, IIndexingService indexingService, ILoggerFactory logger)
        {
            _logger = logger.CreateLogger("BlockIndexingJobFactory");
            _indexingService = indexingService;
            _rpcBlockReader = rpcBlockReader;
        }

        public IJob GetJob(IIndexingSettings settings)
        {
            return new BlockIndexingJob(_rpcBlockReader, _indexingService, settings, _logger);
        }
    }
}