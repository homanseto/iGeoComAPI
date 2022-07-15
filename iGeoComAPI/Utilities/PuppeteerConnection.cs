using PuppeteerSharp;


namespace iGeoComAPI.Utilities
{
    public class PuppeteerConnection
    {
        public async Task<T> PuppeteerGrabber<T>(string? url, string? infoCode, string? waitSelector, Dictionary<string, string>? cookies = null)
        {
            BrowserFetcher browserFetcher = new BrowserFetcher();
            var lanchOptions = new LaunchOptions();
            await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);

            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false,
                IgnoreHTTPSErrors = true,
            }))
            using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync(url);
                if (cookies != null)
                {
                    foreach (var cookie in cookies)
                    {
                        await page.SetCookieAsync(new CookieParam
                        {
                            Name = cookie.Key,
                            Value = cookie.Value,
                        });
                    }
                    await page.GoToAsync(url);
                    await page.WaitForSelectorAsync(waitSelector);
                    //await page.GetCookiesAsync(url);
                }
                else
                {
                    await page.WaitForSelectorAsync(waitSelector);
                }
                return await page.EvaluateFunctionAsync<T>(infoCode);
            }
        }

        public async Task<string> GetUrl(string url)
        {
            try
            {
                var timeout = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
                var options = new NavigationOptions { Timeout = timeout };
                BrowserFetcher browserFetcher = new BrowserFetcher();
                await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
                using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = false,
                    IgnoreHTTPSErrors = true
                })) using (var page = await browser.NewPageAsync())
                {
                    new NavigationOptions().WaitUntil = new[]
                   {
                    WaitUntilNavigation.Load,
                    WaitUntilNavigation.DOMContentLoaded,
                    WaitUntilNavigation.Networkidle0,
                    WaitUntilNavigation.Networkidle2
                };
                    await page.GoToAsync(url);
                    await page.WaitForTimeoutAsync(timeout);
                    var link = page.Url;
                    return link;
                }
            }catch(Exception ex)
            {        
                return string.Empty;
            }

        }
    }
}
