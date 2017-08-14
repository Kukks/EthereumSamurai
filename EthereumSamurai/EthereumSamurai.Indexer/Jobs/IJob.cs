﻿using System.Threading;
using System.Threading.Tasks;

namespace EthereumSamurai.Indexer.Jobs
{
    public interface IJob
    {
        string Id { get; }

        int Version { get; }



        Task RunAsync();

        Task RunAsync(CancellationToken cancellationToken);
    }
}