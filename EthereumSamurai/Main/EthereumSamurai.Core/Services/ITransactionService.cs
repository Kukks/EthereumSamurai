﻿using System.Collections.Generic;
using System.Threading.Tasks;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;

namespace EthereumSamurai.Core.Services
{
    public interface ITransactionService
    {
        Task<TransactionModel> GetAsync(string transactionHash);

        Task<TransactionFullInfoModel> GetFullInfoAsync(string transactionHash);

        Task<IEnumerable<TransactionModel>> GetAsync(TransactionQuery transactionQuery);

        Task<IEnumerable<TransactionModel>> GetForBlockHashAsync(string blockHash);

        Task<IEnumerable<TransactionModel>> GetForBlockNumberAsync(ulong blockNumber);
    }
}