﻿#if !AppStore
using Autofac;
using Foundation;
using GitTrends.Mobile.Shared;

namespace GitTrends.iOS
{
    public partial class AppDelegate
    {
        public AppDelegate() => Xamarin.Calabash.Start();

        UITestBackdoorService? _uiTestBackdoorService;
        UITestBackdoorService UITestBackdoorService => _uiTestBackdoorService ??= ContainerService.Container.BeginLifetimeScope().Resolve<UITestBackdoorService>();

        [Preserve, Export(BackdoorMethodConstants.SetGitHubUser + ":")]
        public async void SetGitHubUser(NSString accessToken) =>
            await UITestBackdoorService.SetGitHubUser(accessToken.ToString()).ConfigureAwait(false);

        [Preserve, Export(BackdoorMethodConstants.TriggerPullToRefresh + ":")]
        public async void TriggerRepositoriesPullToRefresh(NSString noValue) =>
            await UITestBackdoorService.TriggerPullToRefresh().ConfigureAwait(false);

        [Preserve, Export(BackdoorMethodConstants.GetVisibleCollection + ":")]
        public NSString GetVisibleRepositoryList(NSString noValue) =>
            SerializeObject(UITestBackdoorService.GetVisibleCollection());

        [Preserve, Export(BackdoorMethodConstants.GetCurrentTrendsChartOption + ":")]
        public NSString GetCurrentTrendsChartOption(NSString noValue) =>
            SerializeObject(UITestBackdoorService.GetCurrentTrendsChartOption());

        [Preserve, Export(BackdoorMethodConstants.IsTrendsSeriesVisible + ":")]
        public NSString IsTrendsSeriesVisible(NSString seriesLabel) =>
            SerializeObject(UITestBackdoorService.IsTrendsSeriesVisible(seriesLabel.ToString()));

        [Preserve, Export(BackdoorMethodConstants.GetCurrentOnboardingPageNumber + ":")]
        public NSString GetCurrentOnboardingPageNumber(NSString noValue) =>
            SerializeObject(UITestBackdoorService.GetCurrentOnboardingPageNumber());

        [Preserve, Export(BackdoorMethodConstants.PopPage + ":")]
        public async void PopPage(NSString noValue) =>
            await UITestBackdoorService.PopPage().ConfigureAwait(false);

        [Preserve, Export(BackdoorMethodConstants.AreNotificationsEnabled + ":")]
        public NSString AreNotificationsEnabled(NSString noValue) =>
            SerializeObject(UITestBackdoorService.AreNotificationsEnabled());

        [Preserve, Export(BackdoorMethodConstants.GetPreferredTheme + ":")]
        public NSString GetPreferredTheme(NSString noValue) =>
            SerializeObject(UITestBackdoorService.GetPreferredTheme());

        static NSString SerializeObject<T>(T value) => new NSString(Newtonsoft.Json.JsonConvert.SerializeObject(value));
    }
}
#endif