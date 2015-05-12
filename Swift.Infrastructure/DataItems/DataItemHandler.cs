using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Swift.Extensibility;
using Swift.Extensibility.Input;
using Swift.Extensibility.Services;

namespace Swift.Infrastructure.BaseModules.DataItems
{
    /// <summary>
    /// Provides DataItems.
    /// </summary>
    [Export(typeof(IDataItemHandler))]
    public class DataItemHandler : IDataItemHandler, IPluginServiceUser, IInitializationAware
    {
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private IPluginServices _pluginServices;
        private IEnumerable<IDataItemSource> _sources;

        #region IDataItemHandler Members

        /// <summary>
        /// Gets the best match asynchronously.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// The best matching DataItem for the given input.
        /// </returns>
        public async Task<DataItem> GetBestMatchAsync(IInput input)
        {
            IList<DataItem> list = new List<DataItem>();
            await GetMatchingItemsAsync(input, ref list, new CancellationToken());
            return list.OrderByDescending(_ => GetRating(_, input)).First();
        }

        /// <summary>
        /// Gets the matching items asynchronously. Subsequent calls to this method will cancel the currently running call and start new.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="items">The items.</param>
        /// <param name="cancellationToken">The cancellation token. Abort any operation if this cancellation is requested.</param>
        /// <returns></returns>
        public Task GetMatchingItemsAsync(IInput input, ref IList<DataItem> items, CancellationToken cancellationToken)
        {
            try
            {
                _tokenSource.Cancel();
                if (_sources == null || _sources.Count() == 0)
                    return Task.Delay(0); // TODO log error
                _tokenSource = new CancellationTokenSource();
                items.Clear();

                var list = items;
                var po = new ParallelOptions() { CancellationToken = _tokenSource.Token };
                return Task.Run(() =>
                {
                    try
                    {
                        var plr = Parallel.ForEach(_sources, po, _ =>
                        {
                            var dataitemsprovided = _.GetMatchingItems(input);
                            foreach (var di in dataitemsprovided)
                            {
                                po.CancellationToken.ThrowIfCancellationRequested();
                                list.Add(di);
                            }
                        });
                    }
                    catch (OperationCanceledException) { }
                }, po.CancellationToken);
            }
            catch
            {
                return Task.Delay(0);
            }
        }

        /// <summary>
        /// Gets the rating.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private int GetRating(DataItem item, IInput input)
        {
            if (Rater != null)
            {
                return Rater.Rate(item, input);
            }
            else if (Matcher != null)
            {
                return Matcher.GetCertainty(item, input);
            }
            else
            {
                return 0; // TODO improve default rating
            }
        }

        public IDataItemMatcher Matcher { get; private set; }

        public IDataItemRater Rater { get; private set; }

        #endregion

        #region IPluginServiceUser Members

        /// <summary>
        /// Provides an implementation of plugin services.
        /// </summary>
        /// <param name="pluginServices">The plugin services.</param>
        public void SetPluginServices(IPluginServices pluginServices)
        {
            _pluginServices = pluginServices;
        }

        #endregion

        #region IInitializationAware Members

        /// <summary>
        /// Gets the initialization priority. Higher values lead to later initialization.
        /// </summary>
        /// <value>
        /// The initialization priority.
        /// </value>
        public int InitializationPriority
        {
            get { return 0; }
        }

        /// <summary>
        /// Handles the <see cref="E:Initialization" /> event.
        /// </summary>
        /// <param name="args">The <see cref="T:Swift.Extensibility.Services.InitializationEventArgs" /> instance containing the event data.</param>
        public void OnInitialization(InitializationEventArgs args)
        {
            _sources = _pluginServices.GetServices<IDataItemSource>();
            try
            {
                Matcher = _pluginServices.GetService<IDataItemMatcher>();
            }
            catch { } // TODO log
            try
            {
                Rater = _pluginServices.GetService<IDataItemRater>();
            }
            catch { } // TODO log
        }

        #endregion
    }
}
