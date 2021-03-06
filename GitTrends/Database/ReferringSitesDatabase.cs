﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitTrends.Shared;
using SQLite;
using Xamarin.Forms;

namespace GitTrends
{
    public class ReferringSitesDatabase : BaseDatabase
    {
        protected override TimeSpan Expiration { get; } = TimeSpan.FromDays(30);

        public async Task DeleteExpiredData()
        {
            var databaseConnection = await GetDatabaseConnection<MobileReferringSitesDatabaseModel>().ConfigureAwait(false);

            var referringSites = await databaseConnection.Table<MobileReferringSitesDatabaseModel>().ToListAsync();
            var expiredReferringSites = referringSites.Where(x => IsExpired(x.DownloadedAt));

            foreach (var expiredReferringSite in expiredReferringSites)
                await databaseConnection.DeleteAsync(expiredReferringSite).ConfigureAwait(false);
        }

        public async Task<MobileReferringSiteModel?> GetReferringSite(string repositoryUrl, Uri? referrerUri)
        {
            var databaseConnection = await GetDatabaseConnection<MobileReferringSitesDatabaseModel>().ConfigureAwait(false);
            var referringSitesDatabaseModelList = await databaseConnection.Table<MobileReferringSitesDatabaseModel>().Where(x => x.RepositoryUrl == repositoryUrl && x.ReferrerUri == referrerUri).ToListAsync().ConfigureAwait(false);

            var newestReferringSiteModel = referringSitesDatabaseModelList.OrderByDescending(x => x.DownloadedAt).FirstOrDefault();

            return newestReferringSiteModel is null ? null : MobileReferringSitesDatabaseModel.ToReferringSitesModel(newestReferringSiteModel);
        }

        public async Task<List<MobileReferringSiteModel>> GetReferringSites(string repositoryUrl)
        {
            var databaseConnection = await GetDatabaseConnection<MobileReferringSitesDatabaseModel>().ConfigureAwait(false);
            var referringSitesDatabaseModelList = await databaseConnection.Table<MobileReferringSitesDatabaseModel>().Where(x => x.RepositoryUrl == repositoryUrl).ToListAsync().ConfigureAwait(false);

            return referringSitesDatabaseModelList.Select(x => MobileReferringSitesDatabaseModel.ToReferringSitesModel(x)).ToList();
        }

        public async Task<int> SaveReferringSite(MobileReferringSiteModel referringSiteModel, string repositorUrl)
        {
            var databaseConnection = await GetDatabaseConnection<MobileReferringSitesDatabaseModel>().ConfigureAwait(false);
            var referringSitesDatabaseModel = MobileReferringSitesDatabaseModel.ToReferringSitesDatabaseModel(referringSiteModel, repositorUrl);

            return await databaseConnection.InsertOrReplaceAsync(referringSitesDatabaseModel).ConfigureAwait(false);
        }

        class MobileReferringSitesDatabaseModel : IMobileReferringSiteModel
        {
            [Indexed]
            public string RepositoryUrl { get; set; } = string.Empty;

            public string FavIconImageUrl { get; set; } = string.Empty;

            public DateTimeOffset DownloadedAt { get; set; }

            public string Referrer { get; set; } = string.Empty;

            public bool IsReferrerUriValid { get; set; }

            public Uri? ReferrerUri { get; set; }

            public long TotalCount { get; set; }

            public long TotalUniqueCount { get; set; }

            public static MobileReferringSiteModel ToReferringSitesModel(MobileReferringSitesDatabaseModel referringSitesDatabaseModel)
            {
                var referringSiteModel = new ReferringSiteModel(referringSitesDatabaseModel.TotalCount, referringSitesDatabaseModel.TotalUniqueCount, referringSitesDatabaseModel.Referrer, referringSitesDatabaseModel.DownloadedAt);

                return new MobileReferringSiteModel(referringSiteModel, referringSitesDatabaseModel.FavIconImageUrl is null ? null : ImageSource.FromUri(new Uri(referringSitesDatabaseModel.FavIconImageUrl)));
            }

            public static MobileReferringSitesDatabaseModel ToReferringSitesDatabaseModel(MobileReferringSiteModel referringSiteModel, string repositoryUrl)
            {
                return new MobileReferringSitesDatabaseModel
                {
                    FavIconImageUrl = referringSiteModel.FavIconImageUrl,
                    DownloadedAt = referringSiteModel.DownloadedAt,
                    RepositoryUrl = repositoryUrl,
                    Referrer = referringSiteModel.Referrer,
                    TotalUniqueCount = referringSiteModel.TotalUniqueCount,
                    IsReferrerUriValid = referringSiteModel.IsReferrerUriValid,
                    ReferrerUri = referringSiteModel.ReferrerUri,
                    TotalCount = referringSiteModel.TotalCount
                };
            }
        }
    }
}
