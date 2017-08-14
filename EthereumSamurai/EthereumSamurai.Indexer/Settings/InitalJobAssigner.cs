﻿using System.Collections.Generic;
using EthereumSamurai.Core.Models;
using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Core.Settings;
using EthereumSamurai.Indexer.Jobs;

namespace EthereumSamurai.Indexer.Settings
{
    public interface IInitalJobAssigner
    {
        IEnumerable<IJob> GetJobs();
    }

    public class InitalJobAssigner : IInitalJobAssigner
    {
        private readonly IErc20BalanceIndexingJobFactory _erc20BalanceIndexingJobFactory;
        private readonly IBlockIndexingJobFactory        _blockIndexingFactory;
        private readonly IBlockRepository                _blockRepository;
        private readonly IIndexerInstanceSettings        _indexerInstanceSettings;
        private readonly ILog                            _logger;
        private readonly IRpcBlockReader                 _rpcBlockReader;

        public InitalJobAssigner(
            IBlockIndexingJobFactory        blockIndexingFactory,
            IBlockRepository                blockRepository,
            IErc20BalanceIndexingJobFactory erc20BalanceIndexingJobFactory,
            IIndexerInstanceSettings        indexerInstanceSettings,
            ILog                            logger,
            IRpcBlockReader                 rpcBlockReader)
        {
            _blockIndexingFactory        = blockIndexingFactory;
            _blockRepository             = blockRepository;
            _erc20BalanceIndexingJobFactory = erc20BalanceIndexingJobFactory;
            _indexerInstanceSettings     = indexerInstanceSettings;
            _logger                      = logger;
            _rpcBlockReader              = rpcBlockReader;
        }

        public IEnumerable<IJob> GetJobs()
        {
            var jobs = new List<IJob>();
            
            var lastRpcBlock = (ulong) _rpcBlockReader.GetBlockCount().Result;
            var from         = _indexerInstanceSettings.StartBlock;
            var to           = _indexerInstanceSettings.StopBlock ?? lastRpcBlock;
            var partSize     = (to - from) / (ulong) _indexerInstanceSettings.ThreadAmount;

            ulong? toBlock = from;
            
            // Blocks indexers
            for (var i = 0; i < _indexerInstanceSettings.ThreadAmount; i++)
            {
                var fromBlock = (ulong) toBlock + 1;

                toBlock = fromBlock + partSize;
                toBlock = toBlock < to ? toBlock : _indexerInstanceSettings.StopBlock;

                var indexerId = $"{_indexerInstanceSettings.IndexerId}_thread_{i}";
                var job       = _blockIndexingFactory.GetJob(new IndexingSettings
                {
                    IndexerId = indexerId,
                    From      = fromBlock,
                    To        = toBlock
                });

                jobs.Add(job);
            }

            // Balances indexer
            jobs.Add(_erc20BalanceIndexingJobFactory.GetJob(_indexerInstanceSettings.BalancesStartBlock));

            return jobs;
        }
    }
}